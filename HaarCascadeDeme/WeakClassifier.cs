using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MyFloat = System.Single;

namespace HaarCascadeDeme
{
    /// <summary>
    /// 弱分类器
    /// </summary>
    class WeakClassifier : ICloneable
    {
        IFeature _feature;

        internal IFeature Feature
        {
            get { return _feature; }
            //set { _feature = value; }
        }
        MyFloat _threshold;
        bool _posLargeThanThreshold;
        MyFloat _weight;
        MyFloat[] _featureValues;

        /// <summary>
        /// 弱分类器投票权值
        /// </summary>
        public MyFloat Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        public WeakClassifier(IFeature feature)
        {
            _feature = feature;
            _featureValues = null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(200);
            sb.Append(_feature.ToString());
            sb.Append("Threshold=");
            sb.Append(_threshold.ToString("F04"));
            sb.AppendLine(",");
            sb.Append("Sign:");
            sb.AppendLine(_posLargeThanThreshold ? ">," : "<,");
            sb.Append("Weight=");
            sb.AppendLine(_weight.ToString("F02"));
            return sb.ToString();
        }

        public  string GetNumString()
        {
            StringBuilder sb = new StringBuilder(200);
            sb.Append("Threshold=");
            sb.AppendLine(_threshold.ToString("F04"));
            sb.Append(",Sign:");
            sb.AppendLine(_posLargeThanThreshold ? ">" : "<");
            sb.Append(",Weight=");
            sb.AppendLine(_weight.ToString("F02"));
            return sb.ToString();
        }

        public void PreCalcFeatureValue(SampleCollection posSamples, SampleCollection negSamples)
        {
            int numPos = posSamples.Count, numNeg = negSamples.Count;
            int count = numNeg + numPos;
            _featureValues = new MyFloat[count];
            int i, j;
            MyFloat value;
            for (i = 0; i < numPos; i++)
            {
                value = _feature.GetValue(posSamples[i]);
                _featureValues[i] = value;
            }
            j = numPos;
            for (i = 0; i < numNeg; i++)
            {
                value = _feature.GetValue(negSamples[i]);
                _featureValues[j] = value;
                j++;
            }
        }

        /// <summary>
        /// 使用指定的样本集和样本权值训练弱分类器
        /// </summary>
        /// <param name="posSamples"></param>
        /// <param name="negSamples"></param>
        /// <param name="sampleWeight"></param>
        public double Train(SampleCollection posSamples, SampleCollection negSamples, MyFloat[] sampleWeight, List<FeatureValueWithPosFlag> featureValues)
        {
            int numPos = posSamples.Count, numNeg = negSamples.Count;
            int count = numNeg + numPos;
            int i;
            double totalPos = 0, totalNeg = 0;
            featureValues.Clear();
            for (i = 0; i < numPos; i++)
            {
                featureValues.Add(new FeatureValueWithPosFlag(_featureValues[i], sampleWeight[i], true));
                totalPos += sampleWeight[i];
            }
            for (i = numPos; i<count;i++)
            {
                featureValues.Add(new FeatureValueWithPosFlag(_featureValues[i], sampleWeight[i], false));
                totalNeg += sampleWeight[i];
            }

            featureValues.Sort(Cmp);
            double posBefore = 0, negBefore = 0;
            double minErr = double.MaxValue;
            int minIndex = 0;
            bool minSign = true, sign;
            for (i = 0; i < count; i++)
            {
                double err1 = posBefore + (totalNeg - negBefore);
                double err2 = negBefore + (totalPos - posBefore);
                if (err1 < err2)
                {
                    sign = true;
                }
                else
                {
                    sign = false;
                    err1 = err2;
                }
                if (err1 < minErr)
                {
                    minErr = err1;
                    minIndex = i;
                    minSign = sign;
                }
                if (featureValues[i].isPos)
                    posBefore += featureValues[i].weight;
                else
                    negBefore += featureValues[i].weight;
            }
            if (minIndex > 0 && featureValues[minIndex].value != featureValues[minIndex - 1].value)
                _threshold = (featureValues[minIndex].value + featureValues[minIndex - 1].value) / 2;
            else
                _threshold = featureValues[minIndex].value;
            _posLargeThanThreshold = minSign;
            return minErr;
        }

        private int Cmp(FeatureValueWithPosFlag a,FeatureValueWithPosFlag b)
        {
            return Math.Sign(a.value - b.value);
        }

        public bool Predict(ISample sample)
        {
            MyFloat value= _feature.GetValue(sample);
            return (value < _threshold) ^ _posLargeThanThreshold;
        }

        public bool Predict(ISample sample,Point offset)
        {
            MyFloat value = _feature.GetValue(sample,offset);
            return (value < _threshold) ^ _posLargeThanThreshold;
        }

        public bool Predict(int i)
        {
            //MyFloat value = _featureValues[i].value;
            MyFloat value = _featureValues[i];
            return (value < _threshold) ^ _posLargeThanThreshold;
        }
                
        #region ICloneable 成员

        public object Clone()
        {
            WeakClassifier newWeak = (WeakClassifier)this.MemberwiseClone();
            newWeak._featureValues = (MyFloat[])this._featureValues.Clone();
            return newWeak;
        }

        #endregion

        public struct FeatureValueWithPosFlag
        {
            public MyFloat value;
            public MyFloat weight;
            public bool isPos;

            public FeatureValueWithPosFlag(MyFloat v,MyFloat w,bool pos)
            {
                value = v;
                weight = w;
                isPos = pos;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder(20);
                sb.Append("Value=");
                sb.Append(value.ToString("F04"));
                sb.Append(",Weight=");
                sb.Append(weight.ToString("F04"));
                sb.Append(",");
                sb.Append(isPos.ToString());
                return sb.ToString();
            }
        }

        internal void ReleaseTrainData()
        {
            _featureValues = null;
        }


        internal void Save(System.Xml.XmlNode stageNode)
        {
            XmlClass.AddWeakClassifier(stageNode, _threshold, _posLargeThanThreshold,_weight,_feature.ColorType,
                ((HaarFeature)_feature)._rects, ((HaarFeature)_feature)._weights);
        }

        internal static WeakClassifier Load(float threshold, bool posLargeThanThreshold, float weight, System.Drawing.Rectangle[] rects, int[] weights, ColorType colorType)
        {
            HaarFeature feature = new HaarFeature(0, rects, weights, colorType);
            WeakClassifier weak = new WeakClassifier(feature);
            weak._threshold = threshold;
            weak._posLargeThanThreshold = posLargeThanThreshold;
            weak._weight = weight;

            return weak;
        }
    }
}

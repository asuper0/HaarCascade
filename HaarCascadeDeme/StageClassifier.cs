using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;

using MyFloat = System.Single;

namespace HaarCascadeDeme
{
    /// <summary>
    /// 级联分类器中的强分类器
    /// </summary>
    class StageClassifier
    {
        int _id;
        WeakClassifier[] _classifiers;

        /*internal static int ViewId { get; set; }*/
        internal WeakClassifier[] Classifiers
        {
            get { return _classifiers; }
            set { _classifiers = value; }
        }
        double _threshold;
        //MyFloat _hitRate, _falsePositiveRate;

        public int Id
        {
            get { return _id; }
           // set { _id = value; }
        }

        /// <summary>
        /// 误检率
        /// </summary>
//         public MyFloat FalsePositiveRate
//         {
//             get { return _falsePositiveRate; }
//            //set { _falsePositiveRate = value; }
//         }

        public int WeakClassifierCount
        {
            get { return _classifiers.Length; }
        }
        /// <summary>
        /// 检测率
        /// </summary>
//         public MyFloat HitRate
//         {
//             get { return _hitRate; }
//             //set { _hitRate = value; }
//         }

        public StageClassifier(int id)
        {
            _id = id;
        }
        
        private StageClassifier() { }

        const double CV_THRESHOLD_EPS = 0.00001F;
        public bool Predict(ISample sample,Point offset)
        {
            double sum = 0;
            foreach (WeakClassifier weak in _classifiers)
            {
                if (weak.Predict(sample, offset))
                    sum += weak.Weight;
            }
            
            bool result = (sum >= _threshold - CV_THRESHOLD_EPS);
            return result;
        }

        public bool Predict(ISample sample)
        {
            double sum = 0;
            foreach (WeakClassifier weak in _classifiers)
            {
                if (weak.Predict(sample))
                    sum += weak.Weight;
            }
            //MyFloat sum = PredictGetSum(sample);
//             if (_id == StageClassifier.LastId)
//                 _trainPredictValues.Add(sum);
            bool result = (sum >= _threshold - CV_THRESHOLD_EPS);
            return result;
        }

        private MyFloat PredictGetSum(ISample sample)
        {
            double sum = 0;
            foreach (WeakClassifier weak in _classifiers)
            {
                if (weak.Predict(sample))
                    sum += weak.Weight;
            }
            return (MyFloat)sum;
        }

        private bool Predict(int index)
        {
            double sum = PredictGetSum(index);
            //             if (_id == StageClassifier.LastId)
            //                 _trainPredictValues.Add(sum);
            bool result = (sum >= _threshold - CV_THRESHOLD_EPS);
            return result;
        }

        private MyFloat PredictGetSum(int index)
        {
            double sum = 0;
            foreach (WeakClassifier weak in _classifiers)
            {
                if (weak.Predict(index))
                    sum += weak.Weight;
            }
            return (MyFloat)sum;
        }

        /// <summary>
        /// 训练级联分类器中一层的强分类器
        /// </summary>
        /// <param name="posSamples"></param>
        /// <param name="negSamples"></param>
        /// <param name="maxFalsePositiveRate"></param>
        /// <param name="minHitRate"></param>
        /// <returns>训练的统计结果</returns>
        public PredictResult Train(SampleCollection posSamples,
                            SampleCollection negSamples,
                            SampleCollection validateSamples,
                            double maxFalsePositiveRate,
                            double minHitRate)
        {
            List<WeakClassifier> weakClassifiers = new List<WeakClassifier>(10);
            PredictResult result=new PredictResult();
            MyFloat[] sampleWeight = InitWeight(posSamples.Count, negSamples.Count);
            WeakClassifierManager allWeak = WeakClassifierManager.Instance;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            allWeak.PreCalcFeatureValue(posSamples, negSamples);
            watch.Stop();
            if (DebugMsg.Debug)
            {
                string msg = string.Format("所有弱分类器特征值预计算完成，用时：{0}\r\n",
                    watch.Elapsed.ToString());
                DebugMsg.AddMessage(msg, 0);
            }

            int trainTime = 0;
            do
            {
                if (++trainTime != 1)
                    NormalizeWeight(sampleWeight);

                if (DebugMsg.Debug)
                {
                    string msg = string.Format("开始训练第{0}个弱分类器\r\n",
                        trainTime);
                    DebugMsg.AddMessage(msg, 0);
                }
                watch.Reset();
                watch.Start();


                allWeak.Train(posSamples, negSamples, sampleWeight);
                WeakClassifier newBestClassifier = AdaBoost(posSamples.Count, negSamples.Count, sampleWeight);
                //UpdateWeights(newBestClassifier, posSamples.Count, negSamples.Count, sampleWeight);
                weakClassifiers.Add(newBestClassifier);
                _classifiers = weakClassifiers.ToArray();

                result = EvaluateErrorRate(validateSamples, minHitRate, maxFalsePositiveRate);
                watch.Stop();

                if (DebugMsg.Debug)
                {
                    string msg = string.Format("训练完成，花费时间{0}\r\n检测率：\t{1:P5}\t误检率：\t{2:P5}\r\n",
                        watch.Elapsed.ToString(),
                        result.HitRate,
                        result.FalsePositiveRate);
                    DebugMsg.AddMessage(msg, 1);
                }

            } while (result.FalsePositiveRate > maxFalsePositiveRate);

            allWeak.ReleaseTrainData();
            foreach ( WeakClassifier weak in _classifiers)
            {
                weak.ReleaseTrainData();
            }
            return result;
        }

        private void UpdateWeights(WeakClassifier bestClassifier, int numPos ,int  numNeg , float[] sampleWeight)
        {
            int count = numNeg + numPos;
            double err = 0;
            int i;
            bool[] classifyResult = new bool[count];
            bool result;
            for (i = 0; i < numPos; i++)
            {
                result = bestClassifier.Predict(i);
                if (result == false)
                    err += sampleWeight[i];    //累加分类错误的样本权值
                classifyResult[i] = result;
            }
            for (i = numPos; i < count; i++)
            {
                result = bestClassifier.Predict(i);
                if (result == true)
                    err += sampleWeight[i];    //累加分类错误的样本权值
                classifyResult[i] = !result;
            }

            //调整样本权值
            //             logRatio( double val )
            // {
            //     const double eps = 1e-5;
            // 
            //     val = max( val, eps );
            //     val = min( val, 1. - eps );
            //     return log( val/(1. - val) );
            // }
            const double eps = (double)1e-5;
            double val = err;
            val = Math.Max(val, eps);
            val = Math.Min(val, 1 - eps);
            err = val;
            MyFloat factor = (MyFloat)(err / (1 - err));
            for (int indexSample = 0; indexSample < count; indexSample++)
            {
                if (classifyResult[indexSample])
                {
                    sampleWeight[indexSample] *= factor;
                }
            }
            bestClassifier.Weight = -(MyFloat)Math.Log10(factor);  //设定弱分类器权值α
        }        

        private PredictResult EvaluateErrorRate(SampleCollection validateSamples, double minHitRate, double maxFalsePositiveRate)
        {
            const MyFloat FLT_EPSILON = 1.192092896e-07F;        /* smallest such that 1.0+FLT_EPSILON != 1.0 */
            int numPos = validateSamples.PosCount, numNeg = validateSamples.NegCount;
            int count = validateSamples.Count;
            int i, numFalse = 0, numPosTrue = 0;
            List<MyFloat> values = new List<MyFloat>(numPos);

            //统计正样本的特征值
            foreach (ISample sample in validateSamples)
            {
                if (sample.IsPositive)
                {
                    values.Add(this.PredictGetSum(sample));
                }
            }            
            values.Sort();
            int thresholdIdx = (int)((1.0 - minHitRate) * numPos);
            _threshold = values[thresholdIdx];
            numPosTrue = numPos - thresholdIdx;

            for (i = thresholdIdx - 1; i >= 0; i--)
                if (Math.Abs(values[i] - _threshold) < FLT_EPSILON)
                    numPosTrue++;
            double hitRate = ((double)numPosTrue) / ((double)numPos);

            //统计负样本的分类误差
            foreach (ISample sample in validateSamples)
            {
                if (!sample.IsPositive && this.Predict(sample))
                {
                    numFalse++;
                }
            }   
            double falseAlarm = ((double)numFalse) / ((double)numNeg);

            PredictResult result = new PredictResult();
            result.Count = count;
            result.PosCount = numPos;
            result.NegCount = numNeg;
            result.FalsePositiveRate = falseAlarm;
            result.HitRate = hitRate;
            return result;
        }


        /// <summary>
        /// 使用AdaBoost算法训练一次，得到一个最优的弱分类器
        /// </summary>
        /// <param name="posSamples">正样本集</param>
        /// <param name="negSamples">负样本集</param>
        /// <param name="sampleWeight">样本权值，依次包含了正样本和负样本的权值</param>
        /// <returns>分类误差最优的弱分类器</returns>
        private WeakClassifier AdaBoost(int numPos,int numNeg, MyFloat[] sampleWeight)
        {
            int count = numNeg + numPos;
            double minErr = double.MaxValue;
            int minErrIndex = 0, i;
            WeakClassifier[] weakClassifiers = WeakClassifierManager.Instance.WeakClassifiers;
            int classifierCount = weakClassifiers.Length;
            for (int indexClassifier = 0; indexClassifier < classifierCount; indexClassifier++)
            {
                WeakClassifier classifier = weakClassifiers[indexClassifier];
                double errCount = 0;
                bool result;
                for (i = 0; i < numPos; i++)
                {
                    //result=classifier.Predict(posSamples[i]);
                    result = classifier.Predict(i);
                    if (result == false)
                        errCount += sampleWeight[i];    //累加分类错误的样本权值
                }
                for (; i < count; i++)
                {
                    //result = classifier.Predict(negSamples[i]);
                    result = classifier.Predict(i);
                    if (result == true)
                        errCount += sampleWeight[i];    //累加分类错误的样本权值
                }
                if (errCount < minErr)  //记录最佳弱分类器
                {
                    minErr = errCount;
                    minErrIndex = indexClassifier;
                }
            }

            //DebugMsg.AddMessage(minErr.ToString(), 0);
            bool[] classifyResult = new bool[count];
            WeakClassifier bestClassifier = (WeakClassifier)weakClassifiers[minErrIndex].Clone();
            {
                bool result;
                for (i = 0; i < numPos; i++)
                {
                    result = bestClassifier.Predict(i);
                    classifyResult[i] = result;
                }
                for (; i < count; i++)
                {
                    result = bestClassifier.Predict(i);
                    classifyResult[i] = !result;
                }
            }

            //调整样本权值
            //             logRatio( double val )
            // {
            //     const double eps = 1e-5;
            // 
            //     val = max( val, eps );
            //     val = min( val, 1. - eps );
            //     return log( val/(1. - val) );
            // }
            const double eps = (double)1e-5;
            double val = minErr;
            val = Math.Max(val, eps);
            val = Math.Min(val, 1 - eps);
            minErr = val;
            MyFloat factor = (MyFloat)(minErr / (1 - minErr));
            for (int indexSample = 0; indexSample < count; indexSample++)
            {
                if (classifyResult[indexSample])
                {
                    sampleWeight[indexSample] *= factor;
                }
            }
            //WeakClassifier bestClassifier= (WeakClassifier)weakClassifiers[minErrIndex].Clone();
            //bestClassifier.Weight =(MyFloat) Math.Log10(1 / factor);  //设定弱分类器权值α
            bestClassifier.Weight = -(MyFloat)Math.Log10(factor);  //设定弱分类器权值α
            return bestClassifier;
        }

        /// <summary>
        /// 归一化样本权值
        /// </summary>
        /// <param name="sampleWeight">样本权值</param>
        private void NormalizeWeight(MyFloat[] sampleWeight)
        {
            MyFloat sum = sampleWeight.Sum() * 1.00001f;
            for (int i = sampleWeight.Length - 1; i >= 0; i--)
            {
                sampleWeight[i] /= sum;
            }
        }

        /// <summary>
        /// 根据样本数量初始化权重
        /// </summary>
        /// <param name="numPos"></param>
        /// <param name="numNeg"></param>
        /// <returns></returns>
        private MyFloat[] InitWeight(int numPos, int numNeg)
        {
            int count = numNeg + numPos;
            MyFloat[] weights = new MyFloat[count];
            //MyFloat doubleCount = count * 2;
            MyFloat wPos = (MyFloat)1 / (MyFloat)(2 * numPos);
            MyFloat wNeg = (MyFloat)1 / (MyFloat)(2 * numNeg);
            int i;
            for (i = 0; i < numPos; i++)
            {
                weights[i] = wPos;
            }
            for (; i < count; i++)
                weights[i] = wNeg;
            return weights;
        }
        internal void Save()
        {
            System.Xml.XmlNode stageNode = XmlClass.CreateStage(_id, _threshold, _classifiers.Length);
            foreach (WeakClassifier weak in _classifiers)
            {
                weak.Save(stageNode);
            }
        }
        /// <summary>
        /// 计算强分类器的阈值
        /// </summary>
//         private void CalcThreshold()
//         {
//             MyFloat sum = 0;
//             foreach (WeakClassifier weak in _classifiers)
//             {
//                 sum += weak.Weight;
//             }
//             sum /= 2;
//             _threshold = sum;
//         }


        /// <summary>
        /// 调整强分类器的阈值，使得检测率满足要求
        /// </summary>
        /// <param name="result"></param>
        /// <param name="targetHitRate"></param>
//         private void AdjustThreshold(PredictResult result,MyFloat targetHitRate)
//         {
//             _trainPredictValues.Sort();
//             int targetHitCount = (int)(result.Count * targetHitRate + 0.5f);
//             int currentHitCount = (int)(result.Count * result.HitRate);
//             int diff = targetHitCount - currentHitCount;
//             int newIndex = BinarySearch(_trainPredictValues, _threshold)-diff;
//             if (newIndex < 0)
//                 newIndex = 0;
//             else if (newIndex >= _trainPredictValues.Count)
//                 newIndex = _trainPredictValues.Count - 1;
//             _threshold = _trainPredictValues[newIndex];
// 
//         }

        /// <summary>
        /// 二分查找，找到大于等于key且序号最小的元素
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="key">待查找的数值</param>
        /// <returns>找到的序号</returns>
//         int BinarySearch(List<MyFloat> arr, MyFloat key)
//         {
//             int low = 0, high = arr.Count - 1;
//             int mid = (low + high) / 2;
// 
//             while (low <= high)
//             {
//                 mid = (high + low) / 2;
//                 if (arr[mid] == key)
//                 {
//                     break;
//                 }
//                 else if (arr[mid] < key)
//                 {
//                     low = mid + 1;
//                 }
//                 else
//                 {
//                     high = mid - 1;
//                 }
//             }
//             while (mid > 0 && arr[mid - 1] == key)
//                 mid--;
//             if (arr[mid] < key && mid + 1 < arr.Count && arr[mid + 1] > key)
//                 mid++;
//             return mid;
//         }


        internal static StageClassifier Load(int id, double threshold, WeakClassifier[] weaks)
        {
            StageClassifier stage = new StageClassifier();
            stage._id = id;
            stage._threshold = threshold;
            stage._classifiers = weaks;
            return stage;
        }

        public override string ToString()
        {
            return string.Format("Id={0},WeakNum={1},Threshold={2}",
                _id, _classifiers.Length, _threshold);
        }

//         public PredictResult Train1(SampleCollection posSamples,
//                            SampleCollection negSamples,
//                            MyFloat maxFalsePositiveRate,
//                            MyFloat minHitRate)
//         {
//             List<WeakClassifier> weakClassifiers = new List<WeakClassifier>(10);
//             PredictResult predictResult = new PredictResult();
//             MyFloat[] sampleWeight = InitWeight(posSamples.Count, negSamples.Count);
//             WeakClassifierManager allWeak = WeakClassifierManager.Instance;
// 
//             //allWeak.PreCalcFeatureValue(posSamples, negSamples);
//             WeakClassifier classifier = allWeak.WeakClassifiers[StageClassifier.ViewId];
//             List<WeakClassifier.FeatureValueWithPosFlag> trainTmp = new List<WeakClassifier.FeatureValueWithPosFlag>(sampleWeight.Length);
//             classifier.PreCalcFeatureValue(posSamples, negSamples);
//             MyFloat minErrWeak = classifier.Train(posSamples, negSamples, sampleWeight, trainTmp);
// 
//             /*WeakClassifier newBestClassifier = this.AdaBoost(posSamples, negSamples, sampleWeight);*/
//             int numPos = posSamples.Count, numNeg = negSamples.Count;
//             int count = numNeg + numPos,i;
//  
//             MyFloat errCount = 0;
//             bool result;
//             for (i = 0; i < numPos; i++)
//             {
//                 //result=classifier.Predict(posSamples[i]);
//                 result = classifier.Predict(i);
//                 if (result == false)
//                     errCount += sampleWeight[i];    //累加分类错误的样本权值
//                 //classifyResult[i] = result;
//             }
//             int j = numPos;
//             for (i = 0; i < numNeg; i++)
//             {
//                 //result = classifier.Predict(negSamples[i]);
//                 result = classifier.Predict(j);
//                 if (result == true)
//                     errCount += sampleWeight[j];    //累加分类错误的样本权值
//                 //classifyResult[j] = !result;
//                 j++;
//             }
// 
//             _classifiers = new WeakClassifier[] { classifier };
//             DebugMsg.AddMessage(minErrWeak.ToString() + "    " + errCount.ToString(), 0);
//             predictResult = EvaluateErrorRate(posSamples.Count, negSamples.Count, minHitRate, maxFalsePositiveRate);
// 
//             
//             return predictResult;
//         }
    }
}

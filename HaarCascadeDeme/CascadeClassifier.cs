using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MyFloat = System.Single;

namespace HaarCascadeDeme
{
    class CascadeClassifier
    {
        StageClassifier[] _classifiers;

        internal StageClassifier[] Classifiers
        {
            get { return _classifiers; }
            set { _classifiers = value; }
        }
        Size _size;
        public Size Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public int StageCount
        {
            get
            {
                if (_classifiers == null)
                    return 0;
                else
                    return _classifiers.Length;
            }
        }

        public void Save(string filename)
        {
            XmlClass.CreateNewXml(_size, _classifiers.Length);
            foreach (StageClassifier stage in _classifiers)
            {
                stage.Save();
            }
            XmlClass.Save(filename);
        }

        public void Train(SampleCollection posSamples,
                            SampleCollection allNegSamples,
                            SampleCollection validateSamples,
                            Size size,
                            double targetFalsePositiveRate, double maxFalsePositiveRate, double minHitRate)
        {
            const int startCapacity = 20;
            _size = size;
            List<StageClassifier> stageClassifier = new List<StageClassifier>(startCapacity);
            double hitRate = 1;
            double falsePositiveRate = 1;
            int stageCount = 0;
            
            int maxSampleNum;
            SampleCollection negSamples;
            
            {   //计算使用的负样本数量
                double MaxMemoryUse = MemoryInfo.GetFreePhysicalMemory() - 400e6;
                if (MaxMemoryUse < 0)
                    MaxMemoryUse = 1.8e9;
                //MaxMemoryUse = 2.48e9;
                //按照内存大小来选取负样本，在充分利用内存的同时防止使用虚拟内存
                int weakNum = WeakClassifierManager.Instance.WeakClassifiers.Length;
                maxSampleNum = (int)((
                    MaxMemoryUse
                    - 1.5e6f    //预留150M给程序其它部分
                    )
                    / weakNum / sizeof(MyFloat)) - posSamples.Count;
                
                if (DebugMsg.Debug)
                {
                    string msg = string.Format("\r\n单次使用负样本：{0}，特征数量：{1}，预计消耗内存：{2}M\r\n",
                        maxSampleNum, weakNum, MaxMemoryUse / 1024 / 1024);
                    DebugMsg.AddMessage(msg, 0);
                }
            }
            
            //如果已有分类器，计算当前的性能
            if (_classifiers != null && _classifiers.Length > 0)
            {
                PredictResult result = EvaluateErrorRate(validateSamples);
                hitRate = result.HitRate;
                falsePositiveRate = result.FalsePositiveRate;
                stageCount = _classifiers.Length;
                stageClassifier.AddRange(_classifiers);

                negSamples = CreateNextSamples(allNegSamples, falsePositiveRate, maxSampleNum);

                if (DebugMsg.Debug)
                {
                    string msg = string.Format("载入分类器完成，层数：{0}，当前性能为：\r\n检测率：\t{1:P5}，\t误检率：\t{2:P5}\r\n------\r\n",
                        stageCount, hitRate, falsePositiveRate);
                    DebugMsg.AddMessage(msg, 0);
                }
            }
            else
                negSamples = allNegSamples.GetNegSamples(maxSampleNum);

            while (falsePositiveRate > targetFalsePositiveRate && negSamples.Count != 0 && posSamples.Count != 0)
            {
                stageCount++;
                
                if (DebugMsg.Debug)
                {
                    string msg = string.Format("--------------------\r\n开始训练第{0}级分类器，使用的数量为：\r\n正样本：\t{1}\t负样本：\t{2}\r\n目标检测率：\t{3:P5}\t目标误检率：\t{4:P5}\r\n",
                        stageCount, posSamples.Count, negSamples.Count, minHitRate, maxFalsePositiveRate);
                    DebugMsg.AddMessage(msg, 0);
                }

                StageClassifier currentStage = new StageClassifier(stageCount);
                PredictResult result = currentStage.Train(posSamples, negSamples, validateSamples,
                     maxFalsePositiveRate, minHitRate);
                falsePositiveRate *= result.FalsePositiveRate;
                hitRate *= result.HitRate;

                stageClassifier.Add(currentStage);
                _classifiers = stageClassifier.ToArray();
                
                if (DebugMsg.Debug)
                {
                    string msg = string.Format("------\r\n第{0}级分类器训练结束，当前性能为：\r\n检测率：\t{1:P5}，\t误检率：\t{2:P5}\r\n弱分类器数量：{3}\r\n目前训练时间总计：{4}\r\n",
                        stageCount, hitRate, falsePositiveRate, currentStage.WeakClassifierCount, DebugMsg.stopwatch.Elapsed.ToString());
                    DebugMsg.AddMessage(msg, 0);
                }
                if (falsePositiveRate > targetFalsePositiveRate)
                {
                    //posSamples = this.CreateNextSamples(posSamples);
                    validateSamples = this.GetPositivePredictedSamples(validateSamples);
                    negSamples = CreateNextSamples(allNegSamples, falsePositiveRate, maxSampleNum);
                }
                this.Save(string.Format(@"D:\ccc{0}.xml", stageCount.ToString()));
            }

            if (DebugMsg.Debug)
            {
                DebugMsg.stopwatch.Stop();
                string msg = string.Format("\r\n--------------------\r\n全部训练完成，训练时间总计：{0}\r\n",
                    DebugMsg.stopwatch.Elapsed.ToString());
                DebugMsg.AddMessage(msg, 0);
            }

            this.Save(@"D:\ccc.xml");
        }

        private SampleCollection CreateNextSamples(SampleCollection allNegSamples, double falsePositiveRate, int maxSampleNum)
        {
            int selectSampleCount = (int)(maxSampleNum / falsePositiveRate);
            if (selectSampleCount <= 0)
                selectSampleCount = allNegSamples.Count;
            SampleCollection negSamples = allNegSamples.GetNegSamples(selectSampleCount);
            int oldNum = negSamples.Count;
            negSamples = this.GetPositivePredictedSamples(negSamples);
            if (DebugMsg.Debug)
            {
                string msg = string.Format("选取了{0}个负样本，通过了{1}个，误检率为{2:P5}\r\n",
                    oldNum, negSamples.Count, (MyFloat)negSamples.Count / oldNum);
                DebugMsg.AddMessage(msg, 0);
            }
            if (negSamples.Count > maxSampleNum)
                negSamples.TrimExcess(maxSampleNum);
            GC.Collect();
            return negSamples;
        }

        private PredictResult EvaluateErrorRate(SampleCollection validateSamples)
        {        
            //const MyFloat FLT_EPSILON = 1.192092896e-07F;        /* smallest such that 1.0+FLT_EPSILON != 1.0 */
            int numPos = validateSamples.PosCount, numNeg = validateSamples.NegCount;
            int count = validateSamples.Count;
            int numFalse = 0, numPosTrue = 0;
            
            //统计正样本的分类误差
            foreach (ISample sample in validateSamples)
            {
                if (sample.IsPositive )
                {
                    if (Predict(sample))
                        numPosTrue++;
                }
            }
            double hitRate = ((double)numPosTrue) / ((double)numPos);

            //统计负样本的分类误差
            foreach (ISample sample in validateSamples)
            {
                if (!sample.IsPositive)
                {
                    if (Predict(sample))
                    {
                        numFalse++;
                    }
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
        /// 使用当前的级联分类器对负目标negSamples进行分类，返回错分的目标，作为新的负样本集negSamples
        /// </summary>
        /// <param name="negSamples"></param>
        /// <returns></returns>
        private SampleCollection GetPositivePredictedSamples(SampleCollection samplecoll)
        {
            SampleCollection newCollection = new SampleCollection(samplecoll.Count);
            foreach (ISample sample in samplecoll)
            {
                if (true == this.Predict(sample))
                    newCollection.Add(sample);
            }
            newCollection.TrimExcess();
            return newCollection;
        }

        public bool Predict(ISample sample,Point offset)
        {
            int deep = _classifiers.Count();
            for (int i = 0; i < deep;i++ )
            {
                if (_classifiers[i].Predict(sample, offset) == false)
                    return false;
            }
            return true;
        }

        public bool Predict(ISample sample)
        {
            int deep = _classifiers.Count();
            for (int i = 0; i < deep; i++)
            {
                if (_classifiers[i].Predict(sample) == false)
                    return false;
            }
            return true;
        }

        public bool PredictUseOneClassifier(ISample sample,int i)
        {
            int deep = _classifiers.Count();
            if (i >= deep)
                i = deep-1;

            return _classifiers[i].Predict(sample);
        }

        private PredictResult Predict(SampleCollection samples)
        {
            int truePosCount = 0, trueNegCount = 0;
            int falsePosCount = 0, falseNegCount = 0;
            foreach (ISample sample in samples)
            {
                bool isPositive = sample.IsPositive;
                if (this.Predict(sample))
                {
                    if (isPositive)
                        truePosCount++;
                    else
                        falsePosCount++;
                }
                else
                {
                    if (isPositive)
                        falseNegCount++;
                    else
                        trueNegCount++;
                }
            }
            PredictResult result = new PredictResult();
            result.Count = samples.Count;
            result.PosCount = samples.PosCount;
            result.NegCount = samples.NegCount;
            //result.FalseNegativeRate = (MyFloat)falseNegCount / samples.NegCount;
            result.FalsePositiveRate = (double)falsePosCount / (double)samples.PosCount;
            result.HitRate = (double)truePosCount / (double)samples.PosCount;
            //result.TrueNegativeRate = (MyFloat)trueNegCount / samples.NegCount;

            return result;
        }

        internal void LoadFrom(Size size, StageClassifier[] classifiers)
        {
            _size = size;
            _classifiers = classifiers;
        }
    }

    delegate PredictResult PredictCallback(SampleCollection samples);
    struct PredictResult
    {
        public double HitRate;
        public double FalsePositiveRate;
        //public MyFloat FalseNegativeRate;
        //public MyFloat TrueNegativeRate;
        public int Count;
        public int PosCount;
        public int NegCount;
    }
}

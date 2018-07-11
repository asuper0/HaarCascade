using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using MyFloat = System.Single;

namespace HaarCascadeDeme
{
    /// <summary>
    /// 管理所有弱分类器
    /// </summary>
    class WeakClassifierManager
    {
        static WeakClassifierManager _instance = null;
        int _weakId;
        ColorType _colorType;
        bool _gray, _saturation;
        WeakClassifier[] _weakClassifiers;

        public static WeakClassifierManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WeakClassifierManager();
                return WeakClassifierManager._instance;
            }
        }

        private WeakClassifierManager(){}

       
        public WeakClassifier[] WeakClassifiers
        {
            get { return _weakClassifiers; }
            set { _weakClassifiers = value; }
        }

        private bool Save(string filename)
        {
            throw new NotImplementedException();
        }

        private bool Load(string filename)
        {
            throw new NotImplementedException();
        }

        private bool Init(SampleCollection posSamples,
                            SampleCollection negSamples)
        {
            throw new NotImplementedException();
        }

        public void CreateHaarFeatures(int width, int height,ColorType colorType)
        {
            //确定颜色
            int clrCount = 0;
            _gray = _saturation = false;
            if ((colorType & ColorType.Gray) != 0)
            {
                clrCount++;
                _gray = true;
            }
            if ((colorType & ColorType.Saturation) != 0)
            {
                clrCount++;
                _saturation = true;
            }
            if (clrCount == 0)
                throw new ArgumentException("colorType必须包括一种颜色类型", "colorType");
            _colorType = colorType;

            _weakId = 0;
            int weakCount = CalcHaarRectCount(width, height) * clrCount;
            _weakClassifiers = new WeakClassifier[weakCount];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int dx = 1; dx <= width; dx++)
                    {
                        for (int dy = 1; dy <= height; dy++)
                        {
                            // haar_x2
                            if ((x + dx * 2 <= width) && (y + dy <= height))
                            {
                                AddRect(x, y, dx * 2, dy, -1,
                                        x + dx, y, dx, dy, +2);
                            }
                            // haar_y2
                            if ((x + dx <= width) && (y + dy * 2 <= height))
                            {
                                AddRect(x, y, dx, dy * 2, -1,
                                        x, y + dy, dx, dy, +2);
                            }
                            // haar_x3
                            if ((x + dx * 3 <= width) && (y + dy <= height))
                            {
                                AddRect(x, y, dx * 3, dy, -1,
                                        x + dx, y, dx, dy, +3);
                            }
                            // haar_y3
                            if ((x + dx <= width) && (y + dy * 3 <= height))
                            {
                                AddRect(x, y, dx, dy * 3, -1,
                                        x, y + dy, dx, dy, +3);
                            }

                            // x3_y3_middle
                            if ((x + dx * 3 <= width) && (y + dy * 3 <= height))
                            {
                                AddRect(x, y, dx * 3, dy * 3, -1,
                                         x + dx, y + dy, dx, dy, +9);
                            }

                            // x2_y2
                            if ((x + dx * 2 <= width) && (y + dy * 2 <= height))
                            {
                                AddRect(x, y, dx * 2, dy * 2, -1,
                                        x, y, dx, dy, +2,
                                        x + dx, y + dy, dx, dy, +2);
                            }
                        }
                    }
                }
            }
        }

        public void AddSymmetricHaarFeatures(int width, int height, ColorType colorType)
        {
            //确定颜色
            int clrCount = 0;
            _gray = _saturation = false;
            if ((colorType & ColorType.Gray) != 0)
            {
                clrCount++;
                _gray = true;
            }
            if ((colorType & ColorType.Saturation) != 0)
            {
                clrCount++;
                _saturation = true;
            }
            if (clrCount == 0)
                throw new ArgumentException("colorType必须包括一种颜色类型", "colorType");
            _colorType = colorType;

            WeakClassifier[] oldWeakClassifiers = _weakClassifiers;
            _weakId = 0;
            _weakClassifiers = new WeakClassifier[oldWeakClassifiers.Length];
            int mid = (height ) / 2;
            //上下有3格的类型，考虑到取对称轴的位置，赋不同的dy
            int[][] y3_dyList = new int[7][] {  new int[0],
                                                new int[] { 4 }, 
                                                new int[] { 3 },
                                                new int[] { 3 },
                                                new int[] { 2 }, 
                                                new int[] { 1 },
                                                new int[] { 1 } };

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < mid; y++)
                {
                    for (int dx = 1; dx <= width; dx++)
                    {
                        int dy = mid - y;
                        // haar_y2
                        if ((x + dx <= width))
                        {
                            //dy = mid - y;
                            AddRect(x, y, dx, dy * 2, -1,
                                    x, y + dy, dx, dy, +2);
                        }
                        // x2_y2
                        if ((x + dx * 2 <= width) )
                        {
                            //dy = mid - y;
                            AddRect(x, y, dx * 2, dy * 2, -1,
                                    x, y, dx, dy, +2,
                                    x + dx, y + dy, dx, dy, +2);
                        }
                        // haar_y3
                        if ((x + dx <= width) )
                        {
                            foreach (int dy_ in y3_dyList[y])
                            {
                                dy = dy_;
                                AddRect(x, y, dx, dy * 3, -1,
                                       x, y + dy, dx, dy, +3);
                            }
                        }

                        // x3_y3_middle
                        if ((x + dx * 3 <= width))
                        {
                            foreach (int dy_ in y3_dyList[y])
                            {
                                dy = dy_;
                                AddRect(x, y, dx * 3, dy * 3, -1,
                                         x + dx, y + dy, dx, dy, +9);
                            }
                        }
                    }
                }
            }

            List<WeakClassifier> list = new List<WeakClassifier>(oldWeakClassifiers.Length + _weakId);
            list.AddRange(oldWeakClassifiers);
            list.AddRange(_weakClassifiers.ToList().GetRange(0, _weakId));
            _weakClassifiers = list.ToArray();
        }

        private int CalcHaarRectCount( int width,int height)
        {
            int num = 0;
            num += CalcFeatureNum(width, 2) * CalcFeatureNum(height,1);
            num += CalcFeatureNum(width, 1) * CalcFeatureNum(height, 2);
            num += CalcFeatureNum(width, 3) * CalcFeatureNum(height, 1);
            num += CalcFeatureNum(width, 1) * CalcFeatureNum(height, 3);
            num += CalcFeatureNum(width, 3) * CalcFeatureNum(height, 3);
            num += CalcFeatureNum(width, 2) * CalcFeatureNum(height, 2);
            return num;
        }

        private int CalcFeatureNum(int len, int w)
        {
            int num = 0;
            for (int i = w; i <= len; i++)
                num += i / w;
            return num;
        }

        private void AddRect(int x1, int y1, int w1, int h1, int ww1, int x2, int y2, int w2, int h2, int ww2, int x3, int y3, int w3, int h3, int ww3)
        {
            Rectangle[] rects = new Rectangle[]{
                new Rectangle(x1,y1,w1,h1),
                new Rectangle(x2,y2,w2,h2),
                new Rectangle(x3,y3,w3,h3),
            };
            int[] weights = new int[] { ww1, ww2, ww3 };

            if (_gray)
            {
                HaarFeature gray = new HaarFeature(_weakId, rects, weights, ColorType.Gray);
                _weakClassifiers[_weakId] = new WeakClassifier(gray);
                _weakId++;
            }
            if (_saturation)
            {
                HaarFeature saturation = new HaarFeature(_weakId, rects, weights, ColorType.Saturation);
                _weakClassifiers[_weakId] = new WeakClassifier(saturation);
                _weakId++;
            }
        }

        private void AddRect(int x1, int y1, int w1, int h1, int ww1, int x2, int y2, int w2, int h2, int ww2)
        {
            Rectangle[] rects = new Rectangle[]{
                new Rectangle(x1,y1,w1,h1),
                new Rectangle(x2,y2,w2,h2),
            };
            int[] weights = new int[] { ww1, ww2 };
            if (_gray)
            {
                HaarFeature gray = new HaarFeature(_weakId, rects, weights, ColorType.Gray);
                _weakClassifiers[_weakId] = new WeakClassifier(gray);
                _weakId++;
            }
            if (_saturation)
            {
                HaarFeature saturation = new HaarFeature(_weakId, rects, weights, ColorType.Saturation);
                _weakClassifiers[_weakId] = new WeakClassifier(saturation);
                _weakId++;
            }
        }

        /// <summary>
        /// 使用指定的样本集和样本权值训练所有的弱分类器
        /// </summary>
        /// <param name="posSamples"></param>
        /// <param name="negSamples"></param>
        /// <param name="sampleWeight"></param>
        internal void Train(SampleCollection posSamples, SampleCollection negSamples, MyFloat[] sampleWeight)
        {
            //             List<WeakClassifier.FeatureValueWithPosFlag> trainTmp = new List<WeakClassifier.FeatureValueWithPosFlag>(sampleWeight.Length);
            //             foreach (WeakClassifier weak in _weakClassifiers)
            //             {
            //                 weak.Train(posSamples, negSamples, sampleWeight, trainTmp);
            //             }

            ConcurrentStack<List<WeakClassifier.FeatureValueWithPosFlag>> stack = new ConcurrentStack<List<WeakClassifier.FeatureValueWithPosFlag>>();
            Parallel.ForEach(_weakClassifiers,
                () =>
                {
                    List<WeakClassifier.FeatureValueWithPosFlag> trainTmp = null;
                    if (stack.Count > 0)
                    {
                        if (stack.TryPop(out trainTmp) == false)
                            trainTmp = null;
                    } 
                    if(trainTmp==null)
                        trainTmp = new List<WeakClassifier.FeatureValueWithPosFlag>(sampleWeight.Length);
                    return trainTmp;
                },
                (weak, state, trainTmp) =>
                {
                    weak.Train(posSamples, negSamples, sampleWeight, trainTmp);
                    return trainTmp;
                },
                (trainTmp) =>
                {
                    stack.Push(trainTmp);
                }
                );
        }

        internal void PreCalcFeatureValue(SampleCollection posSamples, SampleCollection negSamples)
        {
            //             foreach (WeakClassifier weak in _weakClassifiers)
            //             {
            //                 weak.PreCalcFeatureValue(posSamples, negSamples);
            //             }
            Parallel.ForEach(_weakClassifiers,
                weak =>
                {
                    weak.PreCalcFeatureValue(posSamples, negSamples);
                });
        }

        internal void ReleaseTrainData()
        {
            foreach (WeakClassifier weak in _weakClassifiers)
            {
                weak.ReleaseTrainData();
            }
            GC.Collect();
        }
    }
}

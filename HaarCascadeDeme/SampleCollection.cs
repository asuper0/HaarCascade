using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace HaarCascadeDeme
{
    class SampleCollection:IList<ISample>
    {
        List<ISample> _samples;
        int _posCount, _negCount;
        int[] _negIndex;    //对负样本集，记录每个样本文件包含的样本的 起始下标

        public int NegCount
        {
            get { return _negCount; }
            //set { _negCount = value; }
        }

        public int PosCount
        {
            get { return _posCount; }
            //set { _posCount = value; }
        }
        public int Count { get { return _samples.Count; } }
        public int Capacity
        {
            get { return _samples.Capacity; }
            set { _samples.Capacity = value; }
        }
        public SampleCollection(int capacity)
        {
            _samples = new List<ISample>(capacity);
            _posCount = 0;
            _negCount = 0;
            _negIndex = null;
        }

        public SampleCollection(SampleCollection src)
        {
            _samples = new List<ISample>(src);
            _posCount = src._posCount;
            _negCount = src._negCount;
            _negIndex = null;
        }

        public static SampleCollection LoadPosSamples(string dir, bool isPositive,ColorType colorType)
        {
            if (false == Directory.Exists(dir))
                throw new DirectoryNotFoundException(string.Format("目录\"{0}\"不存在", dir));
            string[] files = ListImageFiles(dir);
            SampleCollection coll = new SampleCollection(files.Length * 4);
            foreach (string filename in files)
            {
                Image<Bgr, Byte> img = new Image<Bgr, Byte>(filename);
                ISample sample;

                sample = new HaarSample(img, isPositive, colorType);
                coll.Add(sample);

                img._Flip(FLIP.HORIZONTAL);
                sample = new HaarSample(img, isPositive, colorType);
                coll.Add(sample);

                img._Flip(FLIP.VERTICAL);
                sample = new HaarSample(img, isPositive, colorType);
                coll.Add(sample);

                img._Flip(FLIP.HORIZONTAL);
                sample = new HaarSample(img, isPositive, colorType);
                coll.Add(sample);
            }
            return coll;
        }


        public static SampleCollection LoadNegSamples(string dir, ColorType colorType,Size windowSize)
        {
            if (false == Directory.Exists(dir))
                throw new DirectoryNotFoundException(string.Format("目录\"{0}\"不存在", dir));
            string[] files = ListImageFiles(dir);
            SampleCollection coll = new SampleCollection(0);
            coll._negIndex=new int[files.Length+1];
            List<HaarSample[]> negList = new List<HaarSample[]>(files.Length);
            int i = 0;
            foreach (string filename in files)
            {
                HaarSample[] samples = HaarSample.LoadNegSample(filename, colorType, windowSize, coll._negIndex[i]);
                negList.Add(samples);
                //if (i + 1 < coll._negIndex.Length)
                    coll._negIndex[i + 1] = coll._negIndex[i] + samples.Length;
                i++;
            }
            coll.Capacity = coll._negIndex[i ];
            foreach (HaarSample[] array in  negList)
            {
                coll._samples.AddRange(array);
            }
            coll._negCount = coll._samples.Count;
            return coll;
        }

        public SampleCollection GetNegSamples(int maxSampleNum)
        {
            int[] indices = RandomList(_samples.Count, maxSampleNum);
            SampleCollection coll = new SampleCollection(indices.Length);
            foreach (int i in indices)
            {
                coll.Add(_samples[i]);
            }
            return coll;
        }

        private int[] RandomList(int sampleCount, int maxSampleNum)
        {
            int[] result;
            int len = sampleCount;
            int[] array = new int[len];
            for (int i = 0; i < len; i++)
            {
                array[i] = i;
            } 
            if (sampleCount <= maxSampleNum)
            {
                result = array;
            }
            else
            {
                Random rand = new Random();
                List<int> list = new List<int>(maxSampleNum);
                for (int i = 0; i < maxSampleNum; i++)
                {
                    int id = rand.Next(len);
                    list.Add(array[id]);
                    array[id] = array[len - 1];
                    len--;
                }
                result = list.ToArray();
            }
            return result;
        }

        private static string[] ListImageFiles(string dir)
        {
            string[] bmp = Directory.GetFiles(dir, "*.bmp");
            string[] jpg = Directory.GetFiles(dir, "*.jp?g");
            string[] png = Directory.GetFiles(dir, "*.png");

            List<string> list = new List<string>(bmp.Length + jpg.Length + png.Length);
            list.AddRange(bmp);
            list.AddRange(jpg);
            list.AddRange(png);
            string[] files = list.ToArray();
            return files;
        }

        #region IList<ISample> 成员

        public int IndexOf(ISample item)
        {
            return _samples.IndexOf(item);
        }

        public void Insert(int index, ISample item)
        {
            _samples.Insert(index, item);
            if (item.IsPositive)
                _posCount++;
            else
                _negCount++;
        }

        public void RemoveAt(int index)
        {
            if (_samples[index].IsPositive)
                _posCount--;
            else
                _negCount--;
            _samples.RemoveAt(index);
        }

        public ISample this[int index]
        {
            get
            {
                if (index < _samples.Count)
                    return _samples[index];
                else
                    return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<ISample> 成员

        
        public void Add(ISample item)
        {
            _samples.Add(item);
            if (item.IsPositive)
                _posCount++;
            else
                _negCount++;
        }

        public void Clear()
        {
            _samples.Clear();
            _posCount = _negCount = 0;
        }

        public bool Contains(ISample item)
        {
            return _samples.Contains(item);
        }

        public void CopyTo(ISample[] array, int arrayIndex)
        {
            _samples.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ISample item)
        {
            int index=_samples.IndexOf(item);
            if (index >= 0)
            {
                if (item.IsPositive)
                    _posCount--;
                else
                    _negCount--;
                _samples.RemoveAt(index);
                return true;
            }
            else
                return false;
        }

        #endregion

        #region IEnumerable<ISample> 成员

        public IEnumerator<ISample> GetEnumerator()
        {
            return _samples.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _samples.GetEnumerator();
        }

        #endregion

        public void TrimExcess()
        {
            _samples.TrimExcess();
        }

        /// <summary>
        /// 将集合中的元素减少到maxSampleNum
        /// </summary>
        /// <param name="maxSampleNum"></param>
        internal void TrimExcess(int maxSampleNum)
        {
            int posCount = 0, negCount = 0;
            for (int i = maxSampleNum; i < _samples.Count;i++ )
            {
                if (_samples[i].IsPositive)
                    posCount++;
                else
                    negCount++;
            }
            _samples.RemoveRange(maxSampleNum, _samples.Count - maxSampleNum);
            _negCount -= negCount;
            _posCount -= posCount;
        }
    }
}

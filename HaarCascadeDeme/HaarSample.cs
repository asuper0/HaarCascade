using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

using MyFloat = System.Single;

namespace HaarCascadeDeme
{
     class HaarSample : ISample
    {
        protected bool _isPositive;
        protected int _id;
        protected MyFloat[,] _grayIntergralImage, _saturationIntergralImage, _graySquareIntergralImage;
        protected int _xOffset, _yOffset;
        protected MyFloat _mean, _std;

        protected HaarSample() { }

        public HaarSample(Image<Bgr, Byte> img,bool isPositive,ColorType colorType)
        {
            MyFloat[,] grayIntergralImage = null, saturationIntergralImage = null, graySquareIntergralImage = null; ;
            if ((colorType & ColorType.Gray) != 0)
            {
                Image<Gray, Byte> gray = img.Convert<Gray, Byte>();
                Image<Gray, double> grayIntergral, squareIntergral;

                //MyFloat[,] norm = NormalizeVariance(gray);
                gray.Integral(out grayIntergral, out squareIntergral);
                grayIntergralImage = ConvertIntergral(grayIntergral);
                graySquareIntergralImage = ConvertIntergral(squareIntergral);

            }
            if ((colorType & ColorType.Saturation) != 0)
            {
                Image<Hsv, Byte> hsv = img.Convert<Hsv, Byte>();
                Image<Gray, Byte> saturation = hsv[1];
                saturationIntergralImage = CalcIntergarl(saturation);
            }           
            
            _grayIntergralImage = grayIntergralImage;
            _saturationIntergralImage = saturationIntergralImage;
            _graySquareIntergralImage = graySquareIntergralImage;
            _isPositive = false;
            _xOffset = 0;
            _yOffset = 0;
            CalcMeanAndStd(img.Size);
            
            _isPositive = isPositive;
//             _xOffset = 0;
//             _yOffset = 0;
//             _graySquareIntergralImage = null;
//             _grayIntergralImage = null;
//             _saturationIntergralImage = null;
// 
//             if ((colorType & ColorType.Gray) != 0)
//             {
//                 Image<Gray, Byte> gray = img.Convert<Gray, Byte>();
//                 MyFloat[,] norm = NormalizeVariance(gray);
//                 _grayIntergralImage = CalcIntergarl(norm);
//             }
//             if ((colorType & ColorType.Saturation) != 0)
//             {
//                 Image<Hsv, Byte> hsv = img.Convert<Hsv, Byte>();
//                 Image<Gray, Byte> saturation = hsv[1];
//                 _saturationIntergralImage = CalcIntergarl(saturation);
//             }
        }

        public HaarSample(Image<Bgr, Byte> img)
        {
            _xOffset = 0;
            _yOffset = 0;
            _graySquareIntergralImage = null;

            Image<Gray, Byte> gray = img.Convert<Gray, Byte>();
            //gray._EqualizeHist();
            MyFloat[,] norm = NormalizeVariance(gray);
            _grayIntergralImage = CalcIntergarl(norm);

            Image<Hsv, Byte> hsv = img.Convert<Hsv, Byte>();
            Image<Gray, Byte> saturation = hsv[1];
            _saturationIntergralImage = CalcIntergarl(saturation);

        }

        public HaarSample(Image<Gray, Byte> subGray, Image<Gray, Byte> subSaturation)
        {
            _xOffset = 0;
            _yOffset = 0;
            _graySquareIntergralImage = null;

            MyFloat[,] norm = NormalizeVariance(subGray);
            _grayIntergralImage = CalcIntergarl(norm);
            _saturationIntergralImage = CalcIntergarl(subSaturation);
        }

        public HaarSample(DetectHaarSample fullSample, Point offset, Size windowSize)
         {
             _grayIntergralImage =fullSample._grayIntergralImage;
             _saturationIntergralImage =fullSample._saturationIntergralImage;
             _graySquareIntergralImage =fullSample._graySquareIntergralImage;
             _xOffset = offset.X;
             _yOffset = offset.Y;
             CalcMeanAndStd(windowSize);
         }
        /// <summary>
        /// 从一个负样本图像中加载所有样本，共用一个图像
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="colorType"></param>
        /// <param name="windowSize">检测窗口大小</param>
        /// <returns></returns>
        public static HaarSample[] LoadNegSample(string filename,ColorType colorType,Size windowSize,int startIndex)
        {
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(filename);
            MyFloat[,] grayIntergralImage = null, saturationIntergralImage = null, graySquareIntergralImage = null; ;
            if ((colorType & ColorType.Gray) != 0)
            {
                Image<Gray, Byte> gray = img.Convert<Gray, Byte>();
                Image<Gray, double> grayIntergral, squareIntergral;
                
                //MyFloat[,] norm = NormalizeVariance(gray);
                gray.Integral(out grayIntergral, out squareIntergral);
                grayIntergralImage = ConvertIntergral(grayIntergral);
                graySquareIntergralImage = ConvertIntergral(squareIntergral);

//                 MyFloat[,] norm = NormalizeVariance(gray);
//                 grayIntergralImage = CalcIntergarl(norm);
//                 graySquareIntergralImage= new MyFloat[0,0];
//                 gray.Integral(out grayIntergral, out squareIntergral);
//                 grayIntergralImage = ConvertIntergral(grayIntergral);
//                 graySquareIntergralImage = ConvertIntergral(squareIntergral);
            }
            if ((colorType & ColorType.Saturation) != 0)
            {
                Image<Hsv, Byte> hsv = img.Convert<Hsv, Byte>();
                Image<Gray, Byte> saturation = hsv[1];
                saturationIntergralImage = CalcIntergarl(saturation);
            } 
            
            int height = img.Rows, width = img.Cols;
            int yEnd = height - windowSize.Height + 1, xEnd = width - windowSize.Width + 1;
            HaarSample[] samples = new HaarSample[yEnd * xEnd];
            int i = 0;
            for (int y = 0; y <yEnd;y++ )
            {
                for (int x = 0; x < xEnd;x++ )
                {
                    HaarSample s = new HaarSample();
                    s._grayIntergralImage = grayIntergralImage;
                    s._saturationIntergralImage = saturationIntergralImage;
                    s._graySquareIntergralImage = graySquareIntergralImage;
                    s._isPositive = false;
                    s._xOffset = x;
                    s._yOffset = y;
                    s._id = startIndex + i;
                    s.CalcMeanAndStd(windowSize);
//                     s._mean = 0;
//                     s._std = 1;
                    samples[i++] = s;
                }
            }
            return samples;            
        }

        protected void CalcMeanAndStd(Size windowSize)
        {
            //计算窗口内的平均值*特征大小，标准差
            _mean = 0; _std = 1;
            MyFloat sum = GetSumRect(_grayIntergralImage, new Rectangle(0, 0, windowSize.Width, windowSize.Height));
            MyFloat squareSum = GetSumRect(_graySquareIntergralImage, new Rectangle(0, 0, windowSize.Width, windowSize.Height));
            _mean = sum / (windowSize.Height * windowSize.Width);
            _std =(MyFloat) Math.Sqrt(squareSum / (windowSize.Height * windowSize.Width) - _mean * _mean);
        }

        protected static MyFloat[,] ConvertIntergral(Image<Gray, double> grayIntergral)
        {
            MyFloat[,] result = new MyFloat[grayIntergral.Rows, grayIntergral.Cols];
            for (int i = 0; i < grayIntergral.Rows;i++ )
            {
                for (int j = 0; j < grayIntergral.Cols;j++ )
                {
                    result[i, j] = (MyFloat)grayIntergral[i, j].Intensity;
                }
            }
            return result;
        }

        protected static MyFloat[,] NormalizeVariance(Image<Gray, Byte> gray)
        {
            Image<Gray, double> sum, squareSum;
            gray.Integral(out sum, out squareSum);
            int ptCount = gray.Rows * gray.Cols;
            MyFloat mean = (MyFloat)(sum[gray.Rows, gray.Cols].Intensity / ptCount);
            MyFloat standardDeviation = (MyFloat)Math.Sqrt(squareSum[gray.Rows, gray.Cols].Intensity / ptCount - mean * mean);
            MyFloat[,] norm = new MyFloat[gray.Rows, gray.Cols];
            for (int i = 0; i < gray.Rows; i++)
            {
                for (int j = 0; j < gray.Cols; j++)
                {
                    norm[i, j] = (MyFloat)((gray[i, j].Intensity - mean) / standardDeviation);
                }
            }
            return norm;
        }
        
        protected static MyFloat[,] CalcIntergarl(Image<Gray, Byte> gray)
        {
            int width=gray.Cols,height=gray.Rows;
            MyFloat[,] nums = new MyFloat[height + 1, width + 1];
            int i, j;
            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    nums[i + 1, j + 1] = (MyFloat)gray[i, j].Intensity - nums[i, j] + nums[i, j + 1] + nums[i + 1, j];
                }
            }
            return nums;
        }

        protected static MyFloat[,] CalcIntergarl(MyFloat[,] gray)
        {
            int width = gray.GetLength(1), height = gray.GetLength(0);
            MyFloat[,] nums = new MyFloat[height + 1, width + 1];
            int i, j;
            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    nums[i + 1, j + 1] = gray[i, j] - nums[i, j] + nums[i, j + 1] + nums[i + 1, j];
                }
            }
            return nums;
        }

        #region ISample 成员

        public MyFloat GetSumRect(float[,] intergralImage, Rectangle rectangle)
        {
            MyFloat sum = 0;
            int x = rectangle.X, y = rectangle.Y, w = rectangle.Width, h = rectangle.Height;

            if (_graySquareIntergralImage != null)
            {
                x += _xOffset;
                y += _yOffset;
            }
            MyFloat s11 = intergralImage[y, x];
            MyFloat s21 = intergralImage[y + h, x];
            MyFloat s12 = intergralImage[y, x + w];
            MyFloat s22 = intergralImage[y + h, x + w];
            sum = s22 + s11 - s12 - s21;
            if (_graySquareIntergralImage != null)
            {
                sum = (sum - _mean * w * h) / _std;
            }
            return sum;
        }

        public bool IsPositive
        {
            get { return _isPositive; }
        }

        public int SampleType
        {
            get { return _isPositive ? 1 : 0; }
        }

        public int Id
        {
            get { return _id; }
        }

        public MyFloat[,] GrayIntergralImage
        {
            get { return _grayIntergralImage; }
        }

        public MyFloat[,] SaturationIntergralImage
        {
            get { return _saturationIntergralImage; }
        }

        #endregion
    }
}

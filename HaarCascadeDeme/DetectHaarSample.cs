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
    class DetectHaarSample : HaarSample
    {
        public Size Size { get; private set; }

        public DetectHaarSample(Image<Bgr, Byte> img)
        {
            MyFloat[,] grayIntergralImage = null, saturationIntergralImage = null, graySquareIntergralImage = null; ;
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>();
            Image<Gray, double> grayIntergral, squareIntergral;

            gray.Integral(out grayIntergral, out squareIntergral);
            grayIntergralImage = ConvertIntergral(grayIntergral);
            graySquareIntergralImage = ConvertIntergral(squareIntergral);

//             MyFloat[,] norm = NormalizeVariance(gray);
//             grayIntergralImage = CalcIntergarl(norm);

            Image<Hsv, Byte> hsv = img.Convert<Hsv, Byte>();
            Image<Gray, Byte> saturation = hsv[1];
            saturationIntergralImage = CalcIntergarl(saturation);

            _grayIntergralImage = grayIntergralImage;
            _saturationIntergralImage = saturationIntergralImage;
            _graySquareIntergralImage = graySquareIntergralImage;
            _isPositive = false;
            _xOffset = 0;
            _yOffset = 0;
            Size = gray.Size;
            //CalcMeanAndStd(gray.Size);
        }
    }
}

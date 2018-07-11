using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Drawing;

using MyFloat = System.Single;

namespace HaarCascadeDeme
{
    struct HaarFeature : IFeature
    {
        public int _id;
        public Rectangle[] _rects;
        public int[] _weights;
        ColorType _colorType;

        public HaarFeature(int id, Rectangle[] rects, int[] weights,ColorType colorType)
        {
            _id = id;
            _rects = rects;
            _weights = weights;
            _colorType = colorType;
        }

        public MyFloat GetValue(ISample img)
        {
            MyFloat[,] intergralImage;
            if (_colorType == ColorType.Gray)
                intergralImage = img.GrayIntergralImage;
            else
                intergralImage = img.SaturationIntergralImage;

            MyFloat sum = 0;
            for (int i = _rects.Length - 1; i >= 0;i-- )
            {
                if (0 == _weights[i])
                    continue;
                sum += _weights[i] * img.GetSumRect(intergralImage, _rects[i]);
            }
            return sum;
        }

        public MyFloat GetValue(ISample img,Point offset)
        {
            MyFloat[,] intergralImage;
            if (_colorType == ColorType.Gray)
                intergralImage = img.GrayIntergralImage;
            else
                intergralImage = img.SaturationIntergralImage;

            MyFloat sum = 0;
            for (int i = _rects.Length - 1; i >= 0; i--)
            {
                if (0 == _weights[i])
                    continue;
                Rectangle rect= _rects[i];
                rect.Offset(offset);
                sum += _weights[i] * img.GetSumRect(intergralImage, rect);
            }
            return sum;
        }

//         private MyFloat GetSumRect(MyFloat[,] intergralImage, Rectangle rectangle)
//         {
//             MyFloat s11 = intergralImage[rectangle.Y, rectangle.X];
//             MyFloat s21 = intergralImage[rectangle.Y + rectangle.Height, rectangle.X];
//             MyFloat s12 = intergralImage[rectangle.Y, rectangle.X + rectangle.Width];
//             MyFloat s22 = intergralImage[rectangle.Y + rectangle.Height, rectangle.X + rectangle.Width];
//             return s22 + s11 - s12 - s21;
//         }

        public MyFloat GetValue(int id)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(100);
            sb.Append(_colorType.ToString());
            sb.AppendLine(",");
            for (int i = 0; i < _weights.Length; i++)
            {
                sb.Append(_rects[i].ToString());
                sb.Append(",");
                if (_weights[i] > 0)
                    sb.Append('+');
                sb.Append(_weights[i].ToString());
                sb.AppendLine(",");
            }
            
            return sb.ToString();
        }
        #region IFeature 成员


        public ColorType ColorType
        {
            get { return _colorType; }
        }

        int IFeature.Id
        {
            get { return _id; }
        }

        #endregion
    }
}

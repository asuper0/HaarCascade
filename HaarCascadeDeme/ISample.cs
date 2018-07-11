using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MyFloat = System.Single;

namespace HaarCascadeDeme
{
    interface ISample
    {
        //MyFloat GetValue(int featureID);
        //MyFloat GetValue(int x,int y,int width,int height);
        //MyFloat Weight { get; }
        bool IsPositive { get; }
        int SampleType { get; }
        int Id { get; }

//         static Size _size;
//         static void SetSampleSize(Size size);


//         MyFloat[,] _grayIntergralImage;
//         MyFloat[,] _saturationIntergralImage;
        MyFloat[,] GrayIntergralImage { get; }
        MyFloat[,] SaturationIntergralImage { get; }
        MyFloat GetSumRect(MyFloat[,] intergralImage, Rectangle rectangle);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MyFloat = System.Single;

namespace HaarCascadeDeme
{
    interface IFeature
    {
        int Id { get; }
        MyFloat GetValue(ISample img);
        MyFloat GetValue(int id);
        MyFloat GetValue(ISample img, Point offset);
        ColorType ColorType { get; }
    }

    enum ColorType:int
    {
        Null=0,
        Gray=0x1,
        Saturation=0x2,
    }
    enum FeatureType:int
    {
        Haar=0x1,
        Hog=0x2,
    }
}

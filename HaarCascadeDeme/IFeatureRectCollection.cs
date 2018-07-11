using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaarCascadeDeme
{
    interface IFeatureRectCollection
    {
        HaarFeature GetFeature(int featureId);
        void Init();

    }
}

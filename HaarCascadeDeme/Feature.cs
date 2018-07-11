using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Drawing;

namespace HaarCascadeDeme
{
    struct Feature
    {
        public int Id;
        public Rectangle[] Rects;
        public int[] Weights;

        public Feature(int id, Rectangle[] rects, int[] weights)
        {
            Id = id;
            Rects = rects;
            Weights = weights;
        }
    }
}

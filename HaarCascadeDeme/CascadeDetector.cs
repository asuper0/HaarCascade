using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace HaarCascadeDeme
{
    class CascadeDetector
    {
        CascadeClassifier _cascade;
        int _xStep, _yStep;

        public CascadeDetector(CascadeClassifier cascade,int xStep,int yStep)
        {
            _cascade = cascade;
            _xStep = xStep;
            _yStep = yStep;
        }

        public Point[] Detect(Image<Bgr, Byte> img)
        {
            DetectHaarSample sample = new DetectHaarSample(img);
            int width = sample.Size.Width;
            int height = sample.Size.Height;
            Size window = _cascade.Size;
            int yEnd=height-window.Height;
            int xEnd=width-window.Width;

            List<Point> result = new List<Point>((int)Math.Sqrt((xEnd + 1) * (yEnd + 1)));
            for (int y = 0; y <= yEnd;y+=_yStep )
            {
                for (int x = 0; x <= xEnd;x+=_xStep )
                {
                    Point offset = new Point(x, y);
                    HaarSample subSample = new HaarSample(sample, offset, window);
                    if (_cascade.Predict(subSample))
                        result.Add(offset);
                }
            }

            return result.ToArray();
        }

        public Point[] Detect(Image<Bgr, Byte> img,int classifierId)
        {
            DetectHaarSample sample = new DetectHaarSample(img);
            int width = sample.Size.Width;
            int height = sample.Size.Height;
            Size window = _cascade.Size;
            int yEnd = height - window.Height;
            int xEnd = width - window.Width;

            List<Point> result = new List<Point>((int)Math.Sqrt((xEnd + 1) * (yEnd + 1)));
            for (int y = 0; y <= yEnd; y += _yStep)
            {
                for (int x = 0; x <= xEnd; x += _xStep)
                {
                    Point offset = new Point(x, y);
                    HaarSample subSample = new HaarSample(sample, offset, window);
                    if (_cascade.PredictUseOneClassifier(subSample,classifierId))
                        result.Add(offset);
                }
            }

            return result.ToArray();
        }

        public Point[] CombineRepeate(Point[] src, Size imgSize, Size windowSize, int minPoint)
        {
            int xOffset=windowSize.Width,yOffset=windowSize.Height;
            bool[,] map = new bool[imgSize.Height + 2 * yOffset, imgSize.Width + 2 * xOffset];
            List<Point> combinePoints = new List<Point>(src.Length);
            foreach (Point pt in src)
            {
                map[pt.Y+yOffset, pt.X+xOffset] = true;
            }
            foreach (Point pt in src)
            {
                if (!map[pt.Y + yOffset, pt.X + xOffset])
                    continue;
                int xSum = 0, ySum = 0, ptCount = 0;
                int x0=pt.X+xOffset,y0=pt.Y+yOffset;
                for (int y = -windowSize.Height / 2; y <= windowSize.Height / 2;y++ )
                {
                    for (int x = -windowSize.Width / 2; x <= windowSize.Width / 2;x++ )
                    {
                        if(map[y0+y,x0+x])
                        {
                            xSum += x;
                            ySum += y;
                            ptCount++;
                        }
                    }
                }
                x0 += xSum / ptCount;
                y0 += ySum / ptCount;

                xSum = ySum = ptCount = 0;
                for (int y = -windowSize.Height / 2; y <= windowSize.Height / 2; y++)
                {
                    for (int x = -windowSize.Width / 2; x <= windowSize.Width / 2; x++)
                    {
                        if (map[y0 + y, x0 + x])
                        {
                            xSum += x;
                            ySum += y;
                            ptCount++;
                            map[y0 + y, x0 + x] = false;
                        }
                    }
                }
                if(ptCount>=minPoint)
                {
                    x0 += xSum / ptCount;
                    y0 += ySum / ptCount;
                    Point newPt = new Point(x0 - xOffset, y0 - yOffset);
                    combinePoints.Add(newPt);
                }
            }
            return combinePoints.ToArray();
        }

        public void ShowDetectResult(string filename, Point[] result)
        {
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(filename);
            foreach (Point pt in result)
            {
                img.Draw(new Rectangle(pt, _cascade.Size), new Bgr(0, 0, 255.0), 1);
            }
            
            Emgu.CV.UI.ImageViewer viewer = new Emgu.CV.UI.ImageViewer(img, "识别结果");
            viewer.ShowDialog();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;

using MyFloat = System.Single;

namespace HaarCascadeDeme
{
    class XmlClass
    {
        static XmlDocument _doc = null;
        public static void CreateNewXml(Size size, int stageCount)
        {

            _doc = new XmlDocument();

            _doc.AppendChild(_doc.CreateXmlDeclaration("1.0", "gb2312", "yes"));
            XmlElement node = _doc.CreateElement("CascadeClassifier");
            node.SetAttribute("Width", size.Width.ToString());
            node.SetAttribute("Height", size.Height.ToString());
            node.SetAttribute("StageCount", stageCount.ToString());

            _doc.AppendChild(node);
        }

        public static XmlNode CreateStage(int stageNum, double threshold, int weakCount)
        {
            XmlNode cascade = _doc.SelectSingleNode("CascadeClassifier");
            XmlElement ele = _doc.CreateElement("Stage");
            ele.SetAttribute("StageNum", stageNum.ToString());
            ele.SetAttribute("Threshold", threshold.ToString());
            ele.SetAttribute("ClassifiersNum", weakCount.ToString());
            cascade.AppendChild(ele);
            return ele;
        }
   
        public static void AddWeakClassifier(XmlNode stageNode, MyFloat threshold, bool posLargeThanThreshold, MyFloat weight, ColorType colorType,
            Rectangle[] rects, int[] rectWeights)
        {
            XmlElement node = _doc.CreateElement("WeakClassifier");
            node.SetAttribute("Threshold", threshold.ToString());
            node.SetAttribute("PosLargeThanThreshold", posLargeThanThreshold.ToString());
            node.SetAttribute("Weight", weight.ToString());
            node.SetAttribute("ColorType", colorType.ToString());
            for (int i= 0;i<rects.Length;i++ )
            {
                Rectangle rect=rects[i];
                XmlElement rectNode = _doc.CreateElement("Rect");
                rectNode.InnerText = string.Format("{0} {1} {2} {3} {4}",
                    rect.X, rect.Y, rect.Width, rect.Height, rectWeights[i]);
                node.AppendChild(rectNode);
            }
            stageNode.AppendChild(node);
        }

        public static CascadeClassifier Load(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            XmlNode cascadeNode = doc.SelectSingleNode("CascadeClassifier");
            int width = int.Parse(cascadeNode.Attributes["Width"].Value);
            int height = int.Parse(cascadeNode.Attributes["Height"].Value);
            int stageCount = int.Parse(cascadeNode.Attributes["StageCount"].Value);

            //XmlNodeList stageList = doc.SelectNodes("Stage");
            StageClassifier[] classifiers = new StageClassifier[stageCount];
            for (int i = 0; i < stageCount;i++ )
            {
                XmlNode stageNode = cascadeNode.ChildNodes[i];
                string numStr=(i+1).ToString();

                if (false == numStr.Equals(stageNode.Attributes["StageNum"].Value))
                {
                    throw new Exception("Xml文件损坏");
                }
                classifiers[i] = LoadStageClassifier(i+1,stageNode);
            }

            CascadeClassifier cascade = new CascadeClassifier();
            cascade.LoadFrom(new Size(width, height), classifiers);

            return cascade;
        }

        private static StageClassifier LoadStageClassifier(int id,XmlNode stageNode)
        {
            MyFloat threshold = MyFloat.Parse(stageNode.Attributes["Threshold"].Value);
            int weakNum = int.Parse(stageNode.Attributes["ClassifiersNum"].Value);

            WeakClassifier[] weaks = new WeakClassifier[weakNum];
            for (int i = 0; i < weakNum;i++ )
            {
                XmlNode weakNode = stageNode.ChildNodes[i];
                weaks[i] = LoadWeakClassifier(weakNode);
            }

            StageClassifier stage = StageClassifier.Load(id,threshold, weaks);
            return stage;
        }

        private static WeakClassifier LoadWeakClassifier(XmlNode weakNode)
        {
            MyFloat threshold = MyFloat.Parse(weakNode.Attributes["Threshold"].Value);
            MyFloat weight = MyFloat.Parse(weakNode.Attributes["Weight"].Value);
            bool posLargeThanThreshold = bool.Parse(weakNode.Attributes["PosLargeThanThreshold"].Value);
            ColorType colorType= ColorType.Null ;
            if (weakNode.Attributes["ColorType"] != null)
            {
                string colorValue = weakNode.Attributes["ColorType"].Value;
                if (colorValue.Contains(ColorType.Gray.ToString()))
                    colorType |= ColorType.Gray;
                if (colorValue.Contains(ColorType.Saturation.ToString()))
                    colorType |= ColorType.Saturation;
                if (colorType == ColorType.Null)
                    colorType = ColorType.Gray;
            }
            else
                colorType = ColorType.Gray;
            
            List<Rectangle> rects = new List<Rectangle>(4);
            List<int> weights = new List<int>(4);
            foreach (XmlNode node in weakNode.ChildNodes)
            {
                string[] nums = node.InnerText.Split(' ');
                int x = int.Parse(nums[0]);
                int y = int.Parse(nums[1]);
                int width = int.Parse(nums[2]);
                int  height= int.Parse(nums[3]);
                int ww = int.Parse(nums[4]);
                rects.Add(new Rectangle(x, y, width, height));
                weights.Add(ww);
            }
            return WeakClassifier.Load(threshold, posLargeThanThreshold, weight, rects.ToArray(), weights.ToArray(),colorType);
        }

        public static string GetXml()
        {
            return _doc.InnerXml;
        }

        internal static void Save(string filename)
        {
            _doc.Save(filename);
        }

    }
}

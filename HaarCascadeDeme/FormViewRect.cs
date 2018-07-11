using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace HaarCascadeDeme
{
    internal partial class FormViewRect : Form
    {
        CascadeClassifier _cascade;
        Image<Gray, Byte> _imgBlank;
        Image<Bgr, Byte> _imgPos;

        public FormViewRect(CascadeClassifier cascade)
        {
            InitializeComponent();
            _cascade = cascade;

            _imgBlank = new Image<Gray, Byte>(cascade.Size);
            _imgBlank.SetValue(new Gray(128.0));
            _imgPos = new Image<Bgr, Byte>(@"D:\My Documents\MATLAB\MyResearch\haarlike\对称正样本\pic004.bmp");
            _imgPos = _imgPos.Copy(new Rectangle(Point.Empty, cascade.Size));

            imageBox1.Image = _imgBlank;
            imageBox2.Image = _imgPos;
            imageBox1.SetZoomScale(16, Point.Empty);
            imageBox2.SetZoomScale(16, Point.Empty);
            InitTree();

            CountClassifiers(cascade);
        }

        private void CountClassifiers(CascadeClassifier cascade)
        {
            int count=0, gray=0, saturation = 0;
            foreach (StageClassifier stage in cascade.Classifiers)
            {
                foreach (WeakClassifier weak in stage.Classifiers)
                {
                    switch (weak.Feature.ColorType)
                    {
                        case ColorType.Gray: gray++;
                            break;
                        case ColorType.Saturation: saturation++;
                            break;
                    }
                    count++;
                }
            }
            this.Text = string.Format("共{0}个弱分类器，其中灰度：{1}，饱和度{2}",
                count, gray, saturation);
        }

        private void InitTree()
        {
            List<TreeNode> root = new List<TreeNode>(_cascade.StageCount);

            foreach (StageClassifier stage in _cascade.Classifiers)
            {
                TreeNode stageNode = new TreeNode(stage.ToString());
                stageNode.Tag = stage;
                foreach (WeakClassifier weak in stage.Classifiers)
                {
                    TreeNode weakNode = new TreeNode(weak.ToString());
                    weakNode.Tag = weak;
                    stageNode.Nodes.Add(weakNode);
                }
                root.Add(stageNode);
            }

            treeView1.Nodes.AddRange(root.ToArray());
            if(treeView1.Nodes.Count>0)
                treeView1.Nodes[0].Expand();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((e.Node.Tag is WeakClassifier) == false)
                return;
            WeakClassifier weak = e.Node.Tag as WeakClassifier;
            textBox1.Text = weak.GetNumString();

            Image<Gray, Byte> imgBlank = _imgBlank.Copy();
            Image<Bgr,Byte> imgPos = _imgPos.Copy();
            HaarFeature feature =(HaarFeature) weak.Feature ;
            for (int i = 0; i < feature._rects.Length;i++ )
            {
                Image<Gray, Byte> mask = new Image<Gray, Byte>(imgBlank.Size);
                int weight = feature._weights[i];
                Rectangle rect=feature._rects[i];
                rect.Height--;
                rect.Width--;
                mask.Draw(rect, new Gray(Math.Abs(weight)), 0);
                if (weight > 0)
                   imgBlank= imgBlank.Add(mask);
                else
                    imgBlank = imgBlank.Sub(mask);

                imgPos.Draw(rect, new Bgr(), 1);
            }
            Image<Gray, Byte> light = imgBlank.ThresholdBinary(new Gray(128), new Gray(255));
            Image<Gray, Byte> dark = imgBlank.ThresholdBinaryInv(new Gray(127), new Gray(255));

            imgBlank.SetValue(new Gray(255), light);
            imgBlank.SetValue(new Gray(0), dark);
            imageBox1.Image = imgBlank;
            imageBox2.Image = imgPos;
        }
        
        
    }
}

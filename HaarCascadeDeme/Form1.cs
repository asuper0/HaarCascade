using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;

using MyFloat = System.Single;
using System.Diagnostics;

namespace HaarCascadeDeme
{
    public partial class Form1 : Form
    {
        SampleCollection _posSamples, _negSamples, _validateSamples;
        CascadeClassifier _cascadeClassifier=null;
        ColorType _colorType;
        Size _size;
        int _validateCount;

        MyFloat _minHitRate, _maxFalsePositiveRate, _targetFalsePositiveRate;

//         private int Cmp(WeakClassifier.FeatureValueWithPosFlag a, WeakClassifier.FeatureValueWithPosFlag b)
//         {
//             return Math.Sign(a.value - b.value);
//         }
                
        public Form1()
        {
            InitializeComponent();
 
//             this.Text = string.Format("{0:D3}", 3);
//               Image<Bgr, Byte> img = new Image<Bgr, Byte>(@"lena.jpg");


//             Image<Gray,double> img = new Image<Gray, double>(12, 12);
//             img.Draw(new Rectangle(2, 2, 7, 7), new Gray(255.0), 1);
//             //img=img.
//             imageBox1.Image = img;
//             Image<Gray,double> img2=img.SmoothGaussian(5);
//             imageBox2.Image = img2;
//             return;
            //textBox_sample_path.Text = @"D:\My Documents\百度云\我的文档\研究生\MyResearch\haarlike\half_detec";
            
            saveFileDialog1.InitialDirectory = Application.StartupPath;
            saveFileDialog1.Filter = "Xml文件(*.xml)|*.xml|所有文件(*.*)|*.*";
            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "Xml文件(*.xml)|*.xml|所有文件(*.*)|*.*";

            openFileDialog2.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";

            Init();

//             string filename = @"D:\My Documents\MATLAB\myresearch\haarlike\negBig\pic000.bmp";
//             HaarSample[] hs = HaarSample.LoadNegSample(filename, _colorType, _size, 0);
//             Image<Bgr, Byte> img = new Image<Bgr, Byte>(filename);
//             img=img.Copy(new Rectangle(new Point(5,3), _size));
//             
//             h1 = new HaarSample(img);
//             h2 = hs[119*3+5];
//             textBox1.Text = "0 0 1 1";
        }

//         HaarSample h1, h2;
//         private void button1_Click_1(object sender, EventArgs e)
//         {
//             string[] strs = textBox1.Text.Split(' ');
//             int x = int.Parse(strs[0]);
//             int y = int.Parse(strs[1]);
//             int w = int.Parse(strs[2]);
//             int h = int.Parse(strs[3]);
// 
//             Rectangle rect = new Rectangle(x, y, w, h);
//             this.Text = h1.GetSumRect(h1.GrayIntergralImage, rect).ToString() + "   " +
//                 h2.GetSumRect(h2.GrayIntergralImage, rect).ToString();
//         }

        private void Init()
        {
            //textBox_sample_path.Text = @"D:\My Documents\MATLAB\MyResearch\haarlike";
            textBox_sample_path.Text = @"D:\Documents\MATLAB\MyResearch\haarlike";
            //cascadeClassifier = new CascadeClassifier();
            _colorType = ColorType.Gray | ColorType.Saturation;
            //            colorType = ColorType.Gray;

            _minHitRate = (MyFloat)0.995;
            _maxFalsePositiveRate = (MyFloat)0.5;
            _targetFalsePositiveRate = (MyFloat)1e-6;
            _size = new Size(32, 14);
            _validateCount = 200000;

            this.Text = string.Format("MinHit={0},MaxFalse={1},TargetFalse={2}{3}{4}",
                _minHitRate, _maxFalsePositiveRate, _targetFalsePositiveRate,
                (_colorType & ColorType.Gray) != 0 ? ",Gray" : "",
                (_colorType & ColorType.Saturation) != 0 ? ",Saturation" : "");
        }
       
        private void button_loadSamples_Click(object sender, EventArgs e)
        {
            string path = Path.GetFullPath(textBox_sample_path.Text);
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists==false)
            {
                MessageBox.Show("路径错误");
                return;
            }
            string pos_path = Path.Combine(path, "对称正样本");
            string neg_path = Path.Combine(path, "negBig");
            //string neg_path = Path.Combine(path, "negSamples");
            //string validate_path = Path.Combine(path, "validate");

            _posSamples = SampleCollection.LoadPosSamples(pos_path, true, _colorType);
            _negSamples = SampleCollection.LoadNegSamples(neg_path, _colorType, _size);
            _validateSamples = _negSamples.GetNegSamples(_validateCount);
            _validateSamples.Capacity += _posSamples.Count;
            foreach (ISample s in _posSamples)
            {
                _validateSamples.Add(s);
            }
        }

        bool inited = false;
        private void button_train_Click(object sender, EventArgs e)
        {
            textBox_debug.BringToFront();
            if (_cascadeClassifier != null)
            {
                if (MessageBox.Show("已存在分类器，在现有分类器上继续训练吗？", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    _cascadeClassifier = new CascadeClassifier();
            }
            else
                _cascadeClassifier = new CascadeClassifier();
            if (!inited)
            {
                button_loadSamples_Click(sender, e);
//                 WeakClassifierManager.Instance.CreateHaarFeatures(_size.Width, _size.Height , _colorType);
                WeakClassifierManager.Instance.CreateHaarFeatures(_size.Width, _size.Height / 2, _colorType);
                WeakClassifierManager.Instance.AddSymmetricHaarFeatures(_size.Width, _size.Height, _colorType);
            }
            //StageClassifier.ViewId = int.Parse(textBox1.Text);
            GC.Collect();
            GC.WaitForFullGCComplete();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(
                (a, b) =>
                {
                    //cascadeClassifier.Train(posSamples, negSamples, (MyFloat)0.001, (MyFloat)0.5, (MyFloat)0.99);
                    _cascadeClassifier.Train(_posSamples, _negSamples,_validateSamples, _size,_targetFalsePositiveRate,_maxFalsePositiveRate,_minHitRate);
                });
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.WorkerReportsProgress = true;

            DebugMsg.Init(worker);
            worker.RunWorkerAsync();
            inited = true;
        }

        private void button_detect_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                imageBox1.BringToFront();
                imageBox2.BringToFront();
                CascadeDetector detect = new CascadeDetector(_cascadeClassifier, 1,1);
                //                 Point[][] result= detect.DetectUpDown(openFileDialog2.FileName);
                //                 detect.ShowDetectResult(openFileDialog2.FileName, result);
                string filename = openFileDialog2.FileName;
                Image<Bgr, Byte> img = new Image<Bgr, Byte>(filename);

                int minPoint = 0, classifierId = -1;
                int.TryParse(textBox1.Text, out minPoint);
                int.TryParse(textBox_class_id.Text, out classifierId);

                Stopwatch watch = new Stopwatch();
                watch.Start();
                Point[] result = (classifierId < 0) ? detect.Detect(img) : detect.Detect(img, classifierId);
                TimeSpan span1 = watch.Elapsed;
                Point[] resultConbined = detect.CombineRepeate(result, img.Size, _cascadeClassifier.Size, minPoint);
                watch.Stop();
                TimeSpan span2 = watch.Elapsed;


                this.Text = Path.GetFileName(filename) + "    " +
                    span1.ToString() + "    " + span2.ToString();
                Size windowSize = _cascadeClassifier.Size;
                Image<Bgr, Byte> img1 = img.Clone();
                Bgr markColor = new Bgr(0, 0, 255.0);
                foreach (Point pt in result)
                {
                    img1[pt.Y, pt.X] = markColor;
                    //img1.Draw(new Rectangle(pt, windowSize), markColor, 1);
                }
                
                foreach (Point pt in resultConbined)
                {
                    img.Draw(new Rectangle(pt, windowSize), markColor, 1);
                }
                imageBox1.Image = img;
                imageBox2.Image = img1;
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                File.WriteAllText(@"D:\ccc.txt", textBox_debug.Text, Encoding.Default);
            }
            catch (System.Exception ex)
            {
                DebugMsg.AddMessage(ex.ToString(), 0);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object[] state=e.UserState as object[];
            string msg = state[0] as string;
            int lineBack =(int) state[1];
            DebugMsg_MessageAdded(msg, lineBack);
        }

        void DebugMsg_MessageAdded(string msg, int lineBack)
        {
            if (lineBack == 0)
            {
                textBox_debug.AppendText(msg);
            }
            else if (lineBack > 0)
            {
                string text = textBox_debug.Text;
                int startIndex = text.LastIndexOf('\n');
                if (startIndex != text.Length - 1)
                    lineBack--;
                while (lineBack-- > 0)
                {
                    startIndex = text.LastIndexOf('\n', startIndex - 1);
                }
                if (startIndex < 0)
                    textBox_debug.Text = msg;
                else
                    textBox_debug.Text = text.Substring(0, startIndex + 1) + msg;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
//             string dir = @"D:\My Documents\MATLAB\MyResearch\images\陈任\pos\";
//             string[] list= Directory.GetFiles(dir);
//             int i = 92;
//             foreach (string filename in list)
//             {
//                 File.Move(filename, string.Format("{0}pic{1:D3}.bmp", dir, i));
//                 i++;
//             }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog()== DialogResult.OK)
            {
                _cascadeClassifier.Save(saveFileDialog1.FileName);
            }
        }

        private void button_load_cascade_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()== DialogResult.OK)
            {
                _cascadeClassifier = XmlClass.Load(openFileDialog1.FileName);
            }
        }

        private void button_viewCascade_Click(object sender, EventArgs e)
        {
            if (_cascadeClassifier == null  || _cascadeClassifier.StageCount<=0)
                _cascadeClassifier = XmlClass.Load(@"cascade_0.000001_0.5_0.99_gray_saturation_20150507_step2neg_height14.xml");
//查看生成的所有特征矩形
//             cascadeClassifier = new CascadeClassifier();
//             cascadeClassifier.Size = _size;
//             WeakClassifierManager.Instance.CreateHaarFeatures(_size.Width, _size.Height / 2, colorType);
//             WeakClassifierManager.Instance.AddSymmetricHaarFeatures(_size.Width, _size.Height, colorType);
//             StageClassifier stage = new StageClassifier(0);
//             stage.Classifiers = WeakClassifierManager.Instance.WeakClassifiers;
//             cascadeClassifier.Classifiers=new StageClassifier[]{stage};
            FormViewRect fv = new FormViewRect(_cascadeClassifier);
            fv.Show();
        }


    }
}

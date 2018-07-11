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
using System.IO;

namespace SymmetryDetection
{
    public partial class Form1 : Form
    {
        string _currFile;
        double _mid;

        public Form1()
        {
            InitializeComponent();
            //textBox_sample_path.Text = @"D:\My Documents\MATLAB\MyResearch\haarlike\posSamples";
            textBox_sample_path.Text = @"D:\My Documents\MATLAB\MyResearch\images\pos";
            textBox_save_path.Text = @"C:\Users\任\Desktop\对称正样本";
            label_save_state.ResetText();
        }

        private void button_convert_Click(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(textBox1.Text, out id) == false ||
                Directory.Exists(textBox_sample_path.Text) == false)
                return;
            string[] filelist = GetFilename();
            if (id >= filelist.Length)
                return;

            _currFile = filelist[id];
            Image<Bgr, Byte> bgr = new Image<Bgr, Byte>(_currFile);
            Image<Gray, Byte> gray = bgr.Convert<Gray, Byte>();
            bool odd = true;
            double bestMiddle = 0, minSum = double.MaxValue;
            int height = gray.Rows, width = gray.Cols;
            int y1,y2;
            int left = width / 3, right = width - left;
            int top = height / 3, bottom = height - top;
            int mid = top;
            while (mid < bottom)
            {
                 y2 = odd ? mid : mid + 1;
                 y1 = mid;
                double sum = 0;
                while (y1>0 && y2<height-1) //忽略上下的边沿
                {
                    for (int i = left; i < right; i++)  //忽略左右的边沿
                    {
                        sum += Math.Abs(gray[y1, i].Intensity - gray[y2, i].Intensity);
                    }
                    y1--;
                    y2++;
                }
                double area = (y2 - y1 + 1 - 2) * (right-left);    //y2和y1均超出图像范围，所以最后-2
                sum /= area;
                if (sum<minSum)
                {
                    minSum = sum;
                    bestMiddle = odd ? mid : mid + 0.5;
                }
                odd = !odd;
                if (odd)
                    mid++;
            }

            _mid = bestMiddle;
            y1 = (int)(bestMiddle + 0.1);

            if ((int)(bestMiddle * 2 + 0.1) % 2 == 0)
                y2 = y1;
            else
                y2 = y1 + 1;
            int h1 = y1 + 1, h2 = height - y2;
            int h = Math.Min(h1, h2);

            byte[, ,] imgBytes = new byte[h, width, 1];
            for(int j=h-1;j>=0;j--)
            {
                for (int i = 0; i < width; i++)
                {
                    imgBytes[j, i, 0] = (Byte)((gray[y1, i].Intensity + gray[y2, i].Intensity) / 2 + 0.5);
                }
                y1--;
                y2++;
            }
            Image<Gray, Byte> symmetricImg = new Image<Gray, Byte>(imgBytes);

            imageBox1.Image = bgr;
            imageBox2.Image = symmetricImg;
            textBox_middle.Text = bestMiddle.ToString();
            textBox_diff.Text = minSum.ToString();
            textBox_img_size.Text = symmetricImg.Size.ToString();
            textBox_raw_size.Text = bgr.Size.ToString();
        }

        private string[] GetFilename()
        {
            string dir = textBox_sample_path.Text;
            string[] bmp = Directory.GetFiles(dir, "*.bmp");
            string[] jpg = Directory.GetFiles(dir, "*.jp?g");
            string[] png = Directory.GetFiles(dir, "*.png");

            List<string> list = new List<string>(bmp.Length + jpg.Length + png.Length);
            list.AddRange(bmp);
            list.AddRange(jpg);
            list.AddRange(png);
            string[] filelist = list.ToArray();
            return filelist;
        }

        private void button_con_next_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(textBox1.Text);
                id++;
                textBox1.Text = id.ToString();
                button_convert_Click(sender, e);
            }
            catch { }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox_save_path.Text) == false)
            {
                label_save_state.ForeColor = Color.Red;
                label_save_state.Text = "目录不存在";
                return;
            }
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(_currFile);
            int y1 = (int)(_mid + 0.1),y2;
            int height = img.Rows, width = img.Cols;

            if ((int)(_mid * 2 + 0.1) % 2 == 0)
                y2 = y1;
            else
                y2 = y1 + 1;
            int h1 = y1 + 1, h2 = height - y2;
            int h = Math.Min(h1, h2);

            int top = y1 - h + 1;
            int bottom = y2 + h - 1;
            Image<Bgr, Byte> tmp = img.Copy(new Rectangle(0, top, width, bottom - top + 1));
            tmp = tmp.Resize(32, 14, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            string filename = Path.Combine(textBox_save_path.Text, Path.GetFileName(_currFile));
            tmp.Save(filename);
            label_save_state.ForeColor = Color.Black;
            label_save_state.Text = "保存成功";
        }
    }
}

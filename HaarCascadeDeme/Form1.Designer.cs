namespace HaarCascadeDeme
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button_loadSamples = new System.Windows.Forms.Button();
            this.textBox_sample_path = new System.Windows.Forms.TextBox();
            this.button_train = new System.Windows.Forms.Button();
            this.textBox_debug = new System.Windows.Forms.TextBox();
            this.button_save = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.button_load_cascade = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button_detect = new System.Windows.Forms.Button();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.button_viewCascade = new System.Windows.Forms.Button();
            this.imageBox1 = new Emgu.CV.UI.ImageBox();
            this.imageBox2 = new Emgu.CV.UI.ImageBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox_class_id = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // button_loadSamples
            // 
            this.button_loadSamples.Location = new System.Drawing.Point(11, 39);
            this.button_loadSamples.Name = "button_loadSamples";
            this.button_loadSamples.Size = new System.Drawing.Size(75, 23);
            this.button_loadSamples.TabIndex = 0;
            this.button_loadSamples.Text = "加载样本";
            this.button_loadSamples.UseVisualStyleBackColor = true;
            this.button_loadSamples.Click += new System.EventHandler(this.button_loadSamples_Click);
            // 
            // textBox_sample_path
            // 
            this.textBox_sample_path.Location = new System.Drawing.Point(12, 12);
            this.textBox_sample_path.Name = "textBox_sample_path";
            this.textBox_sample_path.Size = new System.Drawing.Size(372, 21);
            this.textBox_sample_path.TabIndex = 1;
            this.textBox_sample_path.Text = "I:\\百度云\\我的文档\\研究生\\myresearch\\haarlike\\half_detec";
            // 
            // button_train
            // 
            this.button_train.Location = new System.Drawing.Point(105, 39);
            this.button_train.Name = "button_train";
            this.button_train.Size = new System.Drawing.Size(99, 23);
            this.button_train.TabIndex = 2;
            this.button_train.Text = "加载样本并训练";
            this.button_train.UseVisualStyleBackColor = true;
            this.button_train.Click += new System.EventHandler(this.button_train_Click);
            // 
            // textBox_debug
            // 
            this.textBox_debug.Location = new System.Drawing.Point(12, 83);
            this.textBox_debug.Multiline = true;
            this.textBox_debug.Name = "textBox_debug";
            this.textBox_debug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_debug.Size = new System.Drawing.Size(553, 462);
            this.textBox_debug.TabIndex = 3;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(228, 39);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 4;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_load_cascade
            // 
            this.button_load_cascade.Location = new System.Drawing.Point(309, 39);
            this.button_load_cascade.Name = "button_load_cascade";
            this.button_load_cascade.Size = new System.Drawing.Size(75, 23);
            this.button_load_cascade.TabIndex = 5;
            this.button_load_cascade.Text = "载入分类器";
            this.button_load_cascade.UseVisualStyleBackColor = true;
            this.button_load_cascade.Click += new System.EventHandler(this.button_load_cascade_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button_detect
            // 
            this.button_detect.Location = new System.Drawing.Point(390, 39);
            this.button_detect.Name = "button_detect";
            this.button_detect.Size = new System.Drawing.Size(75, 23);
            this.button_detect.TabIndex = 6;
            this.button_detect.Text = "检测目标";
            this.button_detect.UseVisualStyleBackColor = true;
            this.button_detect.Click += new System.EventHandler(this.button_detect_Click);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            // 
            // button_viewCascade
            // 
            this.button_viewCascade.Location = new System.Drawing.Point(477, 39);
            this.button_viewCascade.Name = "button_viewCascade";
            this.button_viewCascade.Size = new System.Drawing.Size(75, 23);
            this.button_viewCascade.TabIndex = 7;
            this.button_viewCascade.Text = "查看分类器";
            this.button_viewCascade.UseVisualStyleBackColor = true;
            this.button_viewCascade.Click += new System.EventHandler(this.button_viewCascade_Click);
            // 
            // imageBox1
            // 
            this.imageBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBox1.Location = new System.Drawing.Point(12, 79);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(553, 281);
            this.imageBox1.TabIndex = 2;
            this.imageBox1.TabStop = false;
            // 
            // imageBox2
            // 
            this.imageBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBox2.Location = new System.Drawing.Point(12, 368);
            this.imageBox2.Name = "imageBox2";
            this.imageBox2.Size = new System.Drawing.Size(554, 281);
            this.imageBox2.TabIndex = 2;
            this.imageBox2.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(390, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(75, 21);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "2";
            // 
            // textBox_class_id
            // 
            this.textBox_class_id.Location = new System.Drawing.Point(477, 12);
            this.textBox_class_id.Name = "textBox_class_id";
            this.textBox_class_id.Size = new System.Drawing.Size(75, 21);
            this.textBox_class_id.TabIndex = 9;
            this.textBox_class_id.Text = "-1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 661);
            this.Controls.Add(this.textBox_class_id);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.imageBox2);
            this.Controls.Add(this.imageBox1);
            this.Controls.Add(this.button_viewCascade);
            this.Controls.Add(this.button_detect);
            this.Controls.Add(this.button_load_cascade);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.textBox_debug);
            this.Controls.Add(this.button_train);
            this.Controls.Add(this.textBox_sample_path);
            this.Controls.Add(this.button_loadSamples);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_loadSamples;
        private System.Windows.Forms.TextBox textBox_sample_path;
        private System.Windows.Forms.Button button_train;
        private System.Windows.Forms.TextBox textBox_debug;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button button_load_cascade;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button_detect;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Button button_viewCascade;
        private Emgu.CV.UI.ImageBox imageBox1;
        private Emgu.CV.UI.ImageBox imageBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox_class_id;


    }
}


namespace SymmetryDetection
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
            this.textBox_sample_path = new System.Windows.Forms.TextBox();
            this.imageBox1 = new Emgu.CV.UI.ImageBox();
            this.imageBox2 = new Emgu.CV.UI.ImageBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_convert = new System.Windows.Forms.Button();
            this.button_con_next = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_middle = new System.Windows.Forms.TextBox();
            this.textBox_diff = new System.Windows.Forms.TextBox();
            this.textBox_img_size = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_raw_size = new System.Windows.Forms.TextBox();
            this.textBox_save_path = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button_save = new System.Windows.Forms.Button();
            this.label_save_state = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox_sample_path
            // 
            this.textBox_sample_path.Location = new System.Drawing.Point(71, 12);
            this.textBox_sample_path.Name = "textBox_sample_path";
            this.textBox_sample_path.Size = new System.Drawing.Size(439, 21);
            this.textBox_sample_path.TabIndex = 0;
            // 
            // imageBox1
            // 
            this.imageBox1.Location = new System.Drawing.Point(12, 159);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(512, 224);
            this.imageBox1.TabIndex = 2;
            this.imageBox1.TabStop = false;
            // 
            // imageBox2
            // 
            this.imageBox2.Location = new System.Drawing.Point(11, 389);
            this.imageBox2.Name = "imageBox2";
            this.imageBox2.Size = new System.Drawing.Size(512, 112);
            this.imageBox2.TabIndex = 2;
            this.imageBox2.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(71, 69);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "文件序号";
            // 
            // button_convert
            // 
            this.button_convert.Location = new System.Drawing.Point(177, 67);
            this.button_convert.Name = "button_convert";
            this.button_convert.Size = new System.Drawing.Size(75, 23);
            this.button_convert.TabIndex = 5;
            this.button_convert.Text = "转换";
            this.button_convert.UseVisualStyleBackColor = true;
            this.button_convert.Click += new System.EventHandler(this.button_convert_Click);
            // 
            // button_con_next
            // 
            this.button_con_next.Location = new System.Drawing.Point(264, 67);
            this.button_con_next.Name = "button_con_next";
            this.button_con_next.Size = new System.Drawing.Size(75, 23);
            this.button_con_next.TabIndex = 6;
            this.button_con_next.Text = "转换下一个";
            this.button_con_next.UseVisualStyleBackColor = true;
            this.button_con_next.Click += new System.EventHandler(this.button_con_next_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "对称轴位置";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "单位像素差异";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(199, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "图像尺寸";
            // 
            // textBox_middle
            // 
            this.textBox_middle.Location = new System.Drawing.Point(90, 100);
            this.textBox_middle.Name = "textBox_middle";
            this.textBox_middle.Size = new System.Drawing.Size(86, 21);
            this.textBox_middle.TabIndex = 10;
            // 
            // textBox_diff
            // 
            this.textBox_diff.Location = new System.Drawing.Point(90, 132);
            this.textBox_diff.Name = "textBox_diff";
            this.textBox_diff.Size = new System.Drawing.Size(86, 21);
            this.textBox_diff.TabIndex = 10;
            // 
            // textBox_img_size
            // 
            this.textBox_img_size.Location = new System.Drawing.Point(258, 132);
            this.textBox_img_size.Name = "textBox_img_size";
            this.textBox_img_size.Size = new System.Drawing.Size(153, 21);
            this.textBox_img_size.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(199, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "原图尺寸";
            // 
            // textBox_raw_size
            // 
            this.textBox_raw_size.Location = new System.Drawing.Point(258, 100);
            this.textBox_raw_size.Name = "textBox_raw_size";
            this.textBox_raw_size.Size = new System.Drawing.Size(153, 21);
            this.textBox_raw_size.TabIndex = 10;
            // 
            // textBox_save_path
            // 
            this.textBox_save_path.Location = new System.Drawing.Point(71, 42);
            this.textBox_save_path.Name = "textBox_save_path";
            this.textBox_save_path.Size = new System.Drawing.Size(439, 21);
            this.textBox_save_path.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "读取路径";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "保存路径";
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(359, 67);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 12;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // label_save_state
            // 
            this.label_save_state.AutoSize = true;
            this.label_save_state.Location = new System.Drawing.Point(440, 72);
            this.label_save_state.Name = "label_save_state";
            this.label_save_state.Size = new System.Drawing.Size(41, 12);
            this.label_save_state.TabIndex = 13;
            this.label_save_state.Text = "label8";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 508);
            this.Controls.Add(this.label_save_state);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.textBox_save_path);
            this.Controls.Add(this.textBox_raw_size);
            this.Controls.Add(this.textBox_img_size);
            this.Controls.Add(this.textBox_diff);
            this.Controls.Add(this.textBox_middle);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_con_next);
            this.Controls.Add(this.button_convert);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.imageBox2);
            this.Controls.Add(this.imageBox1);
            this.Controls.Add(this.textBox_sample_path);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_sample_path;
        private Emgu.CV.UI.ImageBox imageBox1;
        private Emgu.CV.UI.ImageBox imageBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_convert;
        private System.Windows.Forms.Button button_con_next;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_middle;
        private System.Windows.Forms.TextBox textBox_diff;
        private System.Windows.Forms.TextBox textBox_img_size;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_raw_size;
        private System.Windows.Forms.TextBox textBox_save_path;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Label label_save_state;
    }
}


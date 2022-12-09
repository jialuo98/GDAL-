namespace 遥感图像处理作业_1
{
    partial class HistogramWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.logCheck = new System.Windows.Forms.CheckBox();
            this.maxLabel = new System.Windows.Forms.Label();
            this.minLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.percentileLabel = new System.Windows.Forms.Label();
            this.countLabel = new System.Windows.Forms.Label();
            this.levelLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.medianLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.stdDevLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.meanLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.channelCombo = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.histogram = new 遥感图像处理作业_1.Controls.Histogram();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // logCheck
            // 
            this.logCheck.Location = new System.Drawing.Point(298, 34);
            this.logCheck.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.logCheck.Name = "logCheck";
            this.logCheck.Size = new System.Drawing.Size(90, 26);
            this.logCheck.TabIndex = 58;
            this.logCheck.Text = "Log";
            this.logCheck.CheckedChanged += new System.EventHandler(this.logCheck_CheckedChanged);
            // 
            // maxLabel
            // 
            this.maxLabel.Location = new System.Drawing.Point(102, 465);
            this.maxLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.maxLabel.Name = "maxLabel";
            this.maxLabel.Size = new System.Drawing.Size(72, 22);
            this.maxLabel.TabIndex = 57;
            this.maxLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // minLabel
            // 
            this.minLabel.Location = new System.Drawing.Point(102, 434);
            this.minLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.minLabel.Name = "minLabel";
            this.minLabel.Size = new System.Drawing.Size(72, 22);
            this.minLabel.TabIndex = 56;
            this.minLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(21, 465);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 22);
            this.label9.TabIndex = 55;
            this.label9.Text = "Max:";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(21, 434);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 22);
            this.label8.TabIndex = 54;
            this.label8.Text = "Min:";
            // 
            // percentileLabel
            // 
            this.percentileLabel.Location = new System.Drawing.Point(308, 399);
            this.percentileLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.percentileLabel.Name = "percentileLabel";
            this.percentileLabel.Size = new System.Drawing.Size(108, 24);
            this.percentileLabel.TabIndex = 53;
            // 
            // countLabel
            // 
            this.countLabel.Location = new System.Drawing.Point(308, 368);
            this.countLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.countLabel.Name = "countLabel";
            this.countLabel.Size = new System.Drawing.Size(108, 24);
            this.countLabel.TabIndex = 52;
            // 
            // levelLabel
            // 
            this.levelLabel.Location = new System.Drawing.Point(308, 334);
            this.levelLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.levelLabel.Name = "levelLabel";
            this.levelLabel.Size = new System.Drawing.Size(108, 24);
            this.levelLabel.TabIndex = 51;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(190, 399);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 26);
            this.label7.TabIndex = 50;
            this.label7.Text = "Percentile:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(190, 368);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 24);
            this.label6.TabIndex = 49;
            this.label6.Text = "Count:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(190, 334);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 21);
            this.label5.TabIndex = 48;
            this.label5.Text = "Level:";
            // 
            // medianLabel
            // 
            this.medianLabel.Location = new System.Drawing.Point(102, 400);
            this.medianLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.medianLabel.Name = "medianLabel";
            this.medianLabel.Size = new System.Drawing.Size(72, 22);
            this.medianLabel.TabIndex = 47;
            this.medianLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(21, 400);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 20);
            this.label4.TabIndex = 46;
            this.label4.Text = "Median:";
            // 
            // stdDevLabel
            // 
            this.stdDevLabel.Location = new System.Drawing.Point(102, 369);
            this.stdDevLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.stdDevLabel.Name = "stdDevLabel";
            this.stdDevLabel.Size = new System.Drawing.Size(72, 22);
            this.stdDevLabel.TabIndex = 45;
            this.stdDevLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(21, 369);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 18);
            this.label3.TabIndex = 44;
            this.label3.Text = "Std Dev:";
            // 
            // meanLabel
            // 
            this.meanLabel.Location = new System.Drawing.Point(102, 336);
            this.meanLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.meanLabel.Name = "meanLabel";
            this.meanLabel.Size = new System.Drawing.Size(72, 22);
            this.meanLabel.TabIndex = 43;
            this.meanLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(21, 336);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 20);
            this.label2.TabIndex = 42;
            this.label2.Text = "Mean:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(30, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 28);
            this.label1.TabIndex = 40;
            this.label1.Text = "通道:";
            // 
            // channelCombo
            // 
            this.channelCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.channelCombo.Location = new System.Drawing.Point(93, 28);
            this.channelCombo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.channelCombo.Name = "channelCombo";
            this.channelCombo.Size = new System.Drawing.Size(194, 26);
            this.channelCombo.TabIndex = 41;
            this.channelCombo.SelectedIndexChanged += new System.EventHandler(this.channelCombo_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(16, 68);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(423, 234);
            this.pictureBox1.TabIndex = 80;
            this.pictureBox1.TabStop = false;
            // 
            // histogram
            // 
            this.histogram.Location = new System.Drawing.Point(24, 89);
            this.histogram.Margin = new System.Windows.Forms.Padding(4);
            this.histogram.Name = "histogram";
            this.histogram.Size = new System.Drawing.Size(392, 204);
            this.histogram.TabIndex = 81;
            this.histogram.Text = "histogram1";
            this.histogram.PositionChanged += new 遥感图像处理作业_1.Controls.Histogram.HistogramEventHandler(this.histogram_PositionChanged);
            this.histogram.SelectionChanged += new 遥感图像处理作业_1.Controls.Histogram.HistogramEventHandler(this.histogram_SelectionChanged);
            // 
            // HistogramWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 518);
            this.Controls.Add(this.histogram);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.logCheck);
            this.Controls.Add(this.maxLabel);
            this.Controls.Add(this.minLabel);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.percentileLabel);
            this.Controls.Add(this.countLabel);
            this.Controls.Add(this.levelLabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.medianLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.stdDevLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.meanLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.channelCombo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HistogramWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HistogramWindow";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox logCheck;
        private System.Windows.Forms.Label maxLabel;
        private System.Windows.Forms.Label minLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label percentileLabel;
        private System.Windows.Forms.Label countLabel;
        private System.Windows.Forms.Label levelLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label medianLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label stdDevLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label meanLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox channelCombo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private 遥感图像处理作业_1.Controls.Histogram histogram;
    }
}
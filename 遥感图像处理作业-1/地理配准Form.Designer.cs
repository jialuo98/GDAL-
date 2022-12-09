namespace 遥感图像处理作业_1
{
    partial class 地理配准Form
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
            this.components = new System.ComponentModel.Container();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.图像拼接ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.计算变换系数ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.特征点信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.特征点匹配ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.特征点提取ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.提取同名点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开图片2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox2
            // 
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(476, 359);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseMove);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox2);
            this.splitContainer1.Size = new System.Drawing.Size(935, 359);
            this.splitContainer1.SplitterDistance = 455;
            this.splitContainer1.TabIndex = 9;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(455, 359);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(130, 2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(340, 23);
            this.progressBar1.TabIndex = 8;
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(300, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(99, 22);
            this.toolStripLabel2.Text = "      图片2坐标：";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(387, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(75, 22);
            this.toolStripLabel1.Text = "图片1坐标：";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripTextBox1,
            this.toolStripLabel2,
            this.toolStripTextBox2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 384);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(935, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // 图像拼接ToolStripMenuItem
            // 
            this.图像拼接ToolStripMenuItem.Name = "图像拼接ToolStripMenuItem";
            this.图像拼接ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.图像拼接ToolStripMenuItem.Text = "图像拼接";
            this.图像拼接ToolStripMenuItem.Click += new System.EventHandler(this.图像拼接ToolStripMenuItem_Click);
            // 
            // 计算变换系数ToolStripMenuItem
            // 
            this.计算变换系数ToolStripMenuItem.Name = "计算变换系数ToolStripMenuItem";
            this.计算变换系数ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.计算变换系数ToolStripMenuItem.Text = "计算变换系数";
            this.计算变换系数ToolStripMenuItem.Click += new System.EventHandler(this.计算变换系数ToolStripMenuItem_Click);
            // 
            // 特征点信息ToolStripMenuItem
            // 
            this.特征点信息ToolStripMenuItem.Name = "特征点信息ToolStripMenuItem";
            this.特征点信息ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.特征点信息ToolStripMenuItem.Text = "特征点信息";
            this.特征点信息ToolStripMenuItem.Click += new System.EventHandler(this.特征点信息ToolStripMenuItem_Click);
            // 
            // 特征点匹配ToolStripMenuItem
            // 
            this.特征点匹配ToolStripMenuItem.Name = "特征点匹配ToolStripMenuItem";
            this.特征点匹配ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.特征点匹配ToolStripMenuItem.Text = "特征点匹配";
            this.特征点匹配ToolStripMenuItem.Click += new System.EventHandler(this.特征点匹配ToolStripMenuItem_Click);
            // 
            // 特征点提取ToolStripMenuItem
            // 
            this.特征点提取ToolStripMenuItem.Name = "特征点提取ToolStripMenuItem";
            this.特征点提取ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.特征点提取ToolStripMenuItem.Text = "特征点提取";
            this.特征点提取ToolStripMenuItem.Click += new System.EventHandler(this.特征点提取ToolStripMenuItem_Click);
            // 
            // 提取同名点ToolStripMenuItem
            // 
            this.提取同名点ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.特征点提取ToolStripMenuItem,
            this.特征点匹配ToolStripMenuItem,
            this.特征点信息ToolStripMenuItem,
            this.计算变换系数ToolStripMenuItem,
            this.图像拼接ToolStripMenuItem});
            this.提取同名点ToolStripMenuItem.Name = "提取同名点ToolStripMenuItem";
            this.提取同名点ToolStripMenuItem.Size = new System.Drawing.Size(73, 21);
            this.提取同名点ToolStripMenuItem.Text = "配准/拼接";
            // 
            // 打开图片2ToolStripMenuItem
            // 
            this.打开图片2ToolStripMenuItem.Name = "打开图片2ToolStripMenuItem";
            this.打开图片2ToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.打开图片2ToolStripMenuItem.Text = "打开图片2";
            this.打开图片2ToolStripMenuItem.Click += new System.EventHandler(this.打开图片2ToolStripMenuItem_Click);
            // 
            // 打开ToolStripMenuItem
            // 
            this.打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            this.打开ToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.打开ToolStripMenuItem.Text = "打开图片1";
            this.打开ToolStripMenuItem.Click += new System.EventHandler(this.打开ToolStripMenuItem_Click);
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开ToolStripMenuItem,
            this.打开图片2ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.提取同名点ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(935, 25);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 地理配准Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 409);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "地理配准Form";
            this.Text = "地理配准Form";
            this.Load += new System.EventHandler(this.地理配准Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripMenuItem 图像拼接ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 计算变换系数ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 特征点信息ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 特征点匹配ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 特征点提取ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 提取同名点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开图片2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}
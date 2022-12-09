namespace 遥感图像处理作业_1
{
    partial class 遥感影像变化检测Form
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开变化前影像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开变化后影像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.遥感影像变化检测ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.差值图像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(418, 575);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.遥感影像变化检测ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(841, 32);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开变化前影像ToolStripMenuItem,
            this.打开变化后影像ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(58, 28);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 打开变化前影像ToolStripMenuItem
            // 
            this.打开变化前影像ToolStripMenuItem.Name = "打开变化前影像ToolStripMenuItem";
            this.打开变化前影像ToolStripMenuItem.Size = new System.Drawing.Size(218, 30);
            this.打开变化前影像ToolStripMenuItem.Text = "打开变化前影像";
            this.打开变化前影像ToolStripMenuItem.Click += new System.EventHandler(this.打开变化前影像ToolStripMenuItem_Click);
            // 
            // 打开变化后影像ToolStripMenuItem
            // 
            this.打开变化后影像ToolStripMenuItem.Name = "打开变化后影像ToolStripMenuItem";
            this.打开变化后影像ToolStripMenuItem.Size = new System.Drawing.Size(218, 30);
            this.打开变化后影像ToolStripMenuItem.Text = "打开变化后影像";
            this.打开变化后影像ToolStripMenuItem.Click += new System.EventHandler(this.打开变化后影像ToolStripMenuItem_Click);
            // 
            // 遥感影像变化检测ToolStripMenuItem
            // 
            this.遥感影像变化检测ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.差值图像ToolStripMenuItem});
            this.遥感影像变化检测ToolStripMenuItem.Name = "遥感影像变化检测ToolStripMenuItem";
            this.遥感影像变化检测ToolStripMenuItem.Size = new System.Drawing.Size(166, 28);
            this.遥感影像变化检测ToolStripMenuItem.Text = "遥感影像变化检测";
            // 
            // 差值图像ToolStripMenuItem
            // 
            this.差值图像ToolStripMenuItem.Name = "差值图像ToolStripMenuItem";
            this.差值图像ToolStripMenuItem.Size = new System.Drawing.Size(210, 30);
            this.差值图像ToolStripMenuItem.Text = "差值图像";
            this.差值图像ToolStripMenuItem.Click += new System.EventHandler(this.差值图像ToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox2);
            this.splitContainer1.Size = new System.Drawing.Size(841, 575);
            this.splitContainer1.SplitterDistance = 418;
            this.splitContainer1.TabIndex = 3;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(419, 575);
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // 遥感影像变化检测Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 575);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Name = "遥感影像变化检测Form";
            this.Text = "遥感影像变化检测Form";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开变化前影像ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开变化后影像ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 遥感影像变化检测ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 差值图像ToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
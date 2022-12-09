using System.Windows.Forms;
namespace 遥感图像处理作业_1
{
    partial class MapForm
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
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("ImageList");
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ilamgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.绘制直方图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.移除图片ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.清除所有ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示光谱特征ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.RGB_stripLable = new System.Windows.Forms.ToolStripStatusLabel();
            this.XY_stripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.band = new System.Windows.Forms.ToolStripStatusLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ilamgToolStripMenuItem,
            this.绘制直方图ToolStripMenuItem,
            this.移除图片ToolStripMenuItem,
            this.清除所有ToolStripMenuItem,
            this.显示光谱特征ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(189, 144);
            // 
            // ilamgToolStripMenuItem
            // 
            this.ilamgToolStripMenuItem.Name = "ilamgToolStripMenuItem";
            this.ilamgToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.ilamgToolStripMenuItem.Text = "显示图像";
            this.ilamgToolStripMenuItem.Click += new System.EventHandler(this.显示图像ToolStripMenuItem_Click);
            // 
            // 绘制直方图ToolStripMenuItem
            // 
            this.绘制直方图ToolStripMenuItem.Name = "绘制直方图ToolStripMenuItem";
            this.绘制直方图ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.绘制直方图ToolStripMenuItem.Text = "绘制直方图";
            this.绘制直方图ToolStripMenuItem.Click += new System.EventHandler(this.绘制直方图ToolStripMenuItem_Click);
            // 
            // 移除图片ToolStripMenuItem
            // 
            this.移除图片ToolStripMenuItem.Name = "移除图片ToolStripMenuItem";
            this.移除图片ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.移除图片ToolStripMenuItem.Text = "移除图片";
            this.移除图片ToolStripMenuItem.Click += new System.EventHandler(this.移除图片ToolStripMenuItem_Click);
            // 
            // 清除所有ToolStripMenuItem
            // 
            this.清除所有ToolStripMenuItem.Name = "清除所有ToolStripMenuItem";
            this.清除所有ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.清除所有ToolStripMenuItem.Text = "清除所有";
            this.清除所有ToolStripMenuItem.Click += new System.EventHandler(this.清除所有ToolStripMenuItem_Click);
            // 
            // 显示光谱特征ToolStripMenuItem
            // 
            this.显示光谱特征ToolStripMenuItem.Name = "显示光谱特征ToolStripMenuItem";
            this.显示光谱特征ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.显示光谱特征ToolStripMenuItem.Text = "显示光谱特征";
            this.显示光谱特征ToolStripMenuItem.Click += new System.EventHandler(this.显示光谱特征ToolStripMenuItem_Click);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
            this.treeView1.Name = "treeView1";
            treeNode2.Name = "节点0";
            treeNode2.Text = "ImageList";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.treeView1.Size = new System.Drawing.Size(207, 512);
            this.treeView1.TabIndex = 2;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(61, 4);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Size = new System.Drawing.Size(656, 512);
            this.splitContainer1.SplitterDistance = 207;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 4;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RGB_stripLable,
            this.XY_stripLabel,
            this.band});
            this.statusStrip1.Location = new System.Drawing.Point(0, 483);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(443, 29);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // RGB_stripLable
            // 
            this.RGB_stripLable.Name = "RGB_stripLable";
            this.RGB_stripLable.Size = new System.Drawing.Size(46, 24);
            this.RGB_stripLable.Text = "RGB";
            // 
            // XY_stripLabel
            // 
            this.XY_stripLabel.Name = "XY_stripLabel";
            this.XY_stripLabel.Size = new System.Drawing.Size(33, 24);
            this.XY_stripLabel.Text = "XY";
            // 
            // band
            // 
            this.band.Name = "band";
            this.band.Size = new System.Drawing.Size(55, 24);
            this.band.Text = "band";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(443, 512);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // MapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 512);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MapForm";
            this.Text = "                                  ";
            this.Load += new System.EventHandler(this.MapForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem ilamgToolStripMenuItem;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        public System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStripMenuItem 绘制直方图ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private SplitContainer splitContainer1;
        public PictureBox pictureBox1;
        private ToolStripMenuItem 移除图片ToolStripMenuItem;
        private ToolStripMenuItem 清除所有ToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel RGB_stripLable;
        private ToolStripStatusLabel XY_stripLabel;
        private ToolStripMenuItem 显示光谱特征ToolStripMenuItem;
        private ToolStripStatusLabel band;
        
    }
}
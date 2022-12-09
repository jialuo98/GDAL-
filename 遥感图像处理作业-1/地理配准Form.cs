using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace 遥感图像处理作业_1
{
    public partial class 地理配准Form : Form
    {
        #region 变量
        Dataset dataset1;  //图片1  左图片
        Dataset dataset2;  //图片2  右图片
        int w1, w2, w;
        int h1, h2, h;
        Bitmap bitmap1, bitmap2, bitmap1gray, bitmap2gray;
        MapForm pMapForm = new MapForm();//引用GetImage 函数

        //特征点匹配
        int[,] FeaturePoints = new int[800, 5];  //特征点坐标
        double[,] rp = new double[800, 1];
        int k = 0;  //表示行数
        //gary
        int[,] u1, u2;
        //m1
        int[,] t1, t2; //
        int c1 = 0, c2 = 0;

        //计算变换系数
        double ax, bx, ay, by;
        #endregion
        public 地理配准Form()
        {
            InitializeComponent();
        }
         
        public Bitmap gray1(Bitmap curBitmap)//获取图1的灰度
        {
            int i, j;
            u1 = new int[curBitmap.Width, curBitmap.Height];
            Bitmap bitmapgray = new Bitmap(curBitmap.Width, curBitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            if (curBitmap != null)
            {
                Color curColor;
                int ret;
                for (i = 0; i < curBitmap.Width; i++)
                {
                    for (j = 0; j < curBitmap.Height; j++)
                    {
                        curColor = curBitmap.GetPixel(i, j);
                        ret = (int)(curColor.R * 0.299 + curColor.G * 0.587 + curColor.B * 0.114);
                        u1[i, j] = ret;
                        bitmapgray.SetPixel(i, j, Color.FromArgb(ret, ret, ret));
                    }
                }
                // g.DrawImage(bitmapgray, 0, 0, pictureBox1.Width, pictureBox1.Height);
            }
            return bitmapgray;
        }

        public Bitmap gray2(Bitmap curBitmap)//获取图2的灰度
        {
            int i, j;
            u2 = new int[curBitmap.Width, curBitmap.Height];
            Bitmap bitmapgray = new Bitmap(curBitmap.Width, curBitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            if (curBitmap != null)
            {
                Color curColor;
                int ret;
                for (i = 0; i < curBitmap.Width; i++)
                {
                    for (j = 0; j < curBitmap.Height; j++)
                    {
                        curColor = curBitmap.GetPixel(i, j);
                        ret = (int)(curColor.R * 0.299 + curColor.G * 0.587 + curColor.B * 0.114);
                        u2[i, j] = ret;
                        bitmapgray.SetPixel(i, j, Color.FromArgb(ret, ret, ret));
                    }
                }
            }
            return bitmapgray;
        }

        public void m1(Bitmap curBitmap, int[,] u)
        {
            //定义一个和curBitmap一样大的空的矩阵curBitmap
            int[,] r = new int[curBitmap.Width, curBitmap.Height];
            int i, j;
            for (i = 0; i < curBitmap.Width; i++)
            {
                for (j = 0; j < curBitmap.Height; j++)
                {
                    r[i, j] = 0;
                }
            }

            int[] V = new int[5];
            int k;
            for (i = 2; i < curBitmap.Width - 2; i++)
                for (j = 2; j < curBitmap.Height - 2; j++)
                {
                    //计算V1方向相邻像素灰度差平方和
                    V[1] = (u[i - 2, j] - u[i - 1, j]) * (u[i - 2, j] - u[i - 1, j]) + (u[i - 1, j] - u[i, j]) * (u[i - 1, j] - u[i, j]) + (u[i, j] - u[i + 1, j]) * (u[i, j] - u[i + 1, j]) + (u[i + 1, j] - u[i + 2, j]) * (u[i + 1, j] - u[i + 2, j]);
                    V[2] = (u[i, j - 2] - u[i, j - 1]) * (u[i, j - 2] - u[i, j - 1]) + (u[i, j - 1] - u[i, j]) * (u[i, j - 1] - u[i, j]) + (u[i, j] - u[i, j + 1]) * (u[i, j] - u[i, j + 1]) + (u[i, j + 1] - u[i, j + 2]) * (u[i, j + 1] - u[i, j + 2]);
                    V[3] = (u[i - 2, j - 2] - u[i - 1, j - 1]) * (u[i - 2, j - 2] - u[i - 1, j - 1]) + (u[i - 1, j - 1] - u[i, j]) * (u[i - 1, j - 1] - u[i, j]) + (u[i, j] - u[i + 1, j + 1]) * (u[i, j] - u[i + 1, j + 1]) + (u[i + 1, j + 1] - u[i + 2, j + 2]) * (u[i + 1, j + 1] - u[i + 2, j + 2]);
                    V[4] = (u[i + 2, j - 2] - u[i + 1, j - 1]) * (u[i + 2, j - 2] - u[i + 1, j - 1]) + (u[i + 1, j - 1] - u[i, j]) * (u[i + 1, j - 1] - u[i, j]) + (u[i, j] - u[i - 1, j + 1]) * (u[i, j] - u[i - 1, j + 1]) + (u[i - 1, j + 1] - u[i - 2, j + 2]) * (u[i - 1, j + 1] - u[i - 2, j + 2]);
                    V[0] = V[1];
                    for (k = 2; k < 5; k++)
                    {
                        if (V[k] < V[0])
                            V[0] = V[k];
                    }   //从V1、V2、V3、V4中取最小值作为该点兴趣值
                    r[i, j] = V[0];   //有问题啊
                }

            double sum = 0;
            for (i = 2; i < curBitmap.Width - 2; i++)
                for (j = 2; j < curBitmap.Height - 2; j++)
                    sum = sum + r[i, j];

            double men = sum / ((curBitmap.Width - 4) * (curBitmap.Height - 4));
            for (i = 2; i < curBitmap.Width - 2; i++)
                for (j = 2; j < curBitmap.Height - 2; j++)
                {
                    if (r[i, j] < men * 20)
                        r[i, j] = 0;
                }

            int max = 0; int ii, jj, rii = 0, rjj = 0;
            //每隔20个像素的矩阵求 像素最大值？？
            for (i = 2; i < curBitmap.Width - 22; i = i + 20)
                for (j = 2; j < curBitmap.Height - 22; j = j + 20)
                {
                    for (ii = 0; ii < 20; ii++)
                        for (jj = 0; jj < 20; jj++)
                            if (r[i + ii, j + jj] > max)
                            {
                                max = r[i + ii, j + jj];
                                rii = i + ii; rjj = j + jj;
                            }
                    if (max != 0)
                        r[rii, rjj] = -1;
                    max = 0;
                }
            
            //图上画点
            Graphics g = pictureBox1.CreateGraphics();
            Pen pen2 = new Pen(Color.Red, 1);
            t1 = new int[1000, 3];
            for (i = 0; i < curBitmap.Width; i++)
            {
                for (j = 0; j < curBitmap.Height; j++)
                {
                    if (r[i, j] == -1)
                    {
                        g.DrawLine(pen2, (int)(i), (int)((j - 3)), (int)(i), (int)((j + 3)));
                        g.DrawLine(pen2, (int)((i - 3)), (int)(j), (int)((i + 3)), (int)(j));
                        t1[c1, 0] = c1; t1[c1, 1] = i; t1[c1, 2] = j; //c1?
                        c1++;
                    }
                }
            }
        }

        public void m2(Bitmap curBitmap, int[,] u)
        {
            int[,] r = new int[curBitmap.Width, curBitmap.Height];
            int i, j;
            for (i = 0; i < curBitmap.Width; i++)
            {
                for (j = 0; j < curBitmap.Height; j++)
                {
                    r[i, j] = 0;
                }
            }
            int[] V = new int[5]; int k;
            for (i = 2; i < curBitmap.Width - 2; i++)
                for (j = 2; j < curBitmap.Height - 2; j++)
                {
                    V[1] = (u[i - 2, j] - u[i - 1, j]) * (u[i - 2, j] - u[i - 1, j]) + (u[i - 1, j] - u[i, j]) * (u[i - 1, j] - u[i, j]) + (u[i, j] - u[i + 1, j]) * (u[i, j] - u[i + 1, j]) + (u[i + 1, j] - u[i + 2, j]) * (u[i + 1, j] - u[i + 2, j]);
                    V[2] = (u[i, j - 2] - u[i, j - 1]) * (u[i, j - 2] - u[i, j - 1]) + (u[i, j - 1] - u[i, j]) * (u[i, j - 1] - u[i, j]) + (u[i, j] - u[i, j + 1]) * (u[i, j] - u[i, j + 1]) + (u[i, j + 1] - u[i, j + 2]) * (u[i, j + 1] - u[i, j + 2]);
                    V[3] = (u[i - 2, j - 2] - u[i - 1, j - 1]) * (u[i - 2, j - 2] - u[i - 1, j - 1]) + (u[i - 1, j - 1] - u[i, j]) * (u[i - 1, j - 1] - u[i, j]) + (u[i, j] - u[i + 1, j + 1]) * (u[i, j] - u[i + 1, j + 1]) + (u[i + 1, j + 1] - u[i + 2, j + 2]) * (u[i + 1, j + 1] - u[i + 2, j + 2]);
                    V[4] = (u[i + 2, j - 2] - u[i + 1, j - 1]) * (u[i + 2, j - 2] - u[i + 1, j - 1]) + (u[i + 1, j - 1] - u[i, j]) * (u[i + 1, j - 1] - u[i, j]) + (u[i, j] - u[i - 1, j + 1]) * (u[i, j] - u[i - 1, j + 1]) + (u[i - 1, j + 1] - u[i - 2, j + 2]) * (u[i - 1, j + 1] - u[i - 2, j + 2]);
                    V[0] = V[1];
                    for (k = 2; k < 5; k++)
                    {
                        if (V[k] < V[0])
                            V[0] = V[k];
                    }
                    r[i, j] = V[0];
                }
            int men, s = 0;
            for (i = 2; i < curBitmap.Width - 2; i++)
                for (j = 2; j < curBitmap.Height - 2; j++)
                    s = s + r[i, j];
            men = s / ((curBitmap.Width - 4) * (curBitmap.Height - 4));
            for (i = 2; i < curBitmap.Width - 2; i++)
                for (j = 2; j < curBitmap.Height - 2; j++)
                {
                    if (r[i, j] < men * 20)
                        r[i, j] = 0;
                }
            int max = 0; int ii, jj, rii = 0, rjj = 0;
            for (i = 2; i < curBitmap.Width - 22; i = i + 20)
                for (j = 2; j < curBitmap.Height - 22; j = j + 20)
                {
                    for (ii = 0; ii < 20; ii++)
                        for (jj = 0; jj < 20; jj++)
                            if (r[i + ii, j + jj] > max)
                            {
                                max = r[i + ii, j + jj];
                                rii = i + ii; rjj = j + jj;
                            }
                    if (max != 0)
                        r[rii, rjj] = -1;
                    max = 0;
                }
            Graphics g = pictureBox2.CreateGraphics();
            Pen pen2 = new Pen(Color.Red, 1);
            t2 = new int[1000, 3];
            for (i = 0; i < curBitmap.Width; i++)
            {
                for (j = 0; j < curBitmap.Height; j++)
                {
                    if (r[i, j] == -1)
                    {
                        g.DrawLine(pen2, (int)(i), (int)((j - 3)), (int)(i), (int)((j + 3)));
                        g.DrawLine(pen2, (int)((i - 3)), (int)(j), (int)((i + 3)), (int)(j));
                        t2[c2, 0] = c2; t2[c2, 1] = i; t2[c2, 2] = j;     //数组需要释放
                        c2++;
                    }
                }
            }
            
            // richTextBox1.Text = str;
        }

        public void Draw()
        {
            Graphics g1 = pictureBox1.CreateGraphics();
            Pen pen1 = new Pen(Color.Red, 1);
            Graphics g2 = pictureBox2.CreateGraphics();
            Pen pen2 = new Pen(Color.Red, 1);
            int i, j, ii, jj;
            g1.DrawImage(bitmap1, 0, 0, pictureBox1.Width, pictureBox1.Height);
            g2.DrawImage(bitmap2, 0, 0, pictureBox2.Width, pictureBox2.Height);
            for (int z = 1; z < k; z++)   //没有执行
            {
                i = FeaturePoints[z, 1]; 
                j = FeaturePoints[z, 2];
                g1.DrawLine(pen2, (int)(i), (int)((j - 5)), (int)(i), (int)((j + 5)));
                g1.DrawLine(pen2, (int)((i - 5)), (int)(j), (int)((i + 5)), (int)(j));
                ii = FeaturePoints[z, 3]; 
                jj = FeaturePoints[z, 4];
                g2.DrawLine(pen2, (int)(ii), (int)((jj - 5)), (int)(ii), (int)((jj + 5)));
                g2.DrawLine(pen2, (int)((ii - 5)), (int)(jj), (int)((ii + 5)), (int)(jj));
            }
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileName = new OpenFileDialog();
            FileName.Filter = "remote sensing image|*.tif;*.img;*.hdf;|regular digital image|*.jpg;*.png;*.bmp*";
            FileName.Title = "打开图像文件";
            FileName.ShowHelp = true;
           
            timer1.Enabled = true;
     
            if (FileName.ShowDialog() == DialogResult.OK)
            {
                string filename = FileName.FileName.ToString();
                if (filename == "")
                {
                    MessageBox.Show("影像路径不能为空");
                    return;
                }
                dataset1 = OSGeo.GDAL.Gdal.Open(filename, OSGeo.GDAL.Access.GA_ReadOnly);

                w1 = dataset1.RasterXSize;
                h1 = dataset1.RasterYSize;
                w = w1;
                h = h1;

                int[] disband = { 1,2,3 };
                Rectangle pictureRect = new Rectangle();
                pictureRect.X = 0;
                pictureRect.Y = 0;
                pictureRect.Width = this.pictureBox1.Width;
                pictureRect.Height = this.pictureBox1.Height;
                bitmap1 = pMapForm.GetImage(dataset1, pictureRect, disband);

                //sw1 = 1.0*pictureBox1.Width / bitmap1.Width;
                //sh1 =1.0* pictureBox1.Height / bitmap1.Height;
                //progressBar1.Value = 80;
                // pictureBox1.Image = Image.FromFile(zp);
                pictureBox1.Image = bitmap1;
                Graphics g = pictureBox1.CreateGraphics();
                // g.DrawImage(bitmap1, 0, 0, pictureBox1.Width, pictureBox1.Height);


            }
        }

        private void 打开图片2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileName = new OpenFileDialog();
            FileName.Filter = "remote sensing image|*.tif;*.img;*.hdf;|regular digital image|*.jpg;*.png;*.bmp*";
            FileName.Title = "打开图像文件";
            FileName.ShowHelp = true;

            if (FileName.ShowDialog() == DialogResult.OK)
            {
                string filename = FileName.FileName.ToString();
                if (filename == "")
                {
                    MessageBox.Show("影像路径不能为空");
                    return;
                }
                dataset2 = OSGeo.GDAL.Gdal.Open(filename, OSGeo.GDAL.Access.GA_ReadOnly);

                w2 = dataset2.RasterXSize;
                h2 = dataset2.RasterYSize;
                w = w2;
                h = h2;

                int[] disband = { 1, 2, 3 };
                Rectangle pictureRect = new Rectangle();
                pictureRect.X = 0;
                pictureRect.Y = 0;
                pictureRect.Width = this.pictureBox1.Width;
                pictureRect.Height = this.pictureBox1.Height;
                bitmap2 = pMapForm.GetImage(dataset2, pictureRect, disband);//
                //sw2 = 1.0*pictureBox2.Width / bitmap2.Width;
                // sh2 = 1.0*pictureBox2.Height / bitmap2.Height;
                progressBar1.Value = 80;
                // pictureBox1.Image = Image.FromFile(zp);
                pictureBox2.Image = bitmap2;
                Graphics g = pictureBox2.CreateGraphics();
                // g.DrawImage(bitmap2, 0, 0, pictureBox2.Width, pictureBox2.Height);
                progressBar1.Value = 100;
            }

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripTextBox1.Text = "X：" + e.X.ToString() + "   Y：" + e.Y.ToString();
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripTextBox2.Text = "X：" + e.X.ToString() + "   Y：" + e.Y.ToString();
        }

        private void 特征点提取ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 5;
            bitmap1gray = gray1(bitmap1);
            progressBar1.Value = 30;
            m1(bitmap1, u1); //u1为空
            progressBar1.Value = 50;

            bitmap2gray = gray2(bitmap2);
            progressBar1.Value = 80;
            m2(bitmap2, u2);
            progressBar1.Value = 100;
        }

        private void 特征点匹配ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 5;
            int i, j, ii = 0, jj = 0;
            double mean1, mean2, a, b;
            int[,] tezhengmuban1 = new int[5, 5];
            int[,] tezhengmuban2 = new int[5, 5];
            for (i = 0; i < c1; i++)
            {
                double Maxcor = 0;
                for (j = 0; j < c2; j++)
                {
                    int s1 = 0, s2 = 0;
                    for (int m = 0; m < 5; m++)
                    {
                        for (int n = 0; n < 5; n++)
                        {
                            tezhengmuban1[m, n] = u1[t1[i, 1] - 2 + m, t1[i, 2] - 2 + n];
                            tezhengmuban2[m, n] = u2[t2[j, 1] - 2 + m, t2[j, 2] - 2 + n];
                            s1 = s1 + tezhengmuban1[m, n];
                            s2 = s2 + tezhengmuban2[m, n];
                        }
                    }
                    mean1 = s1 / 25.0;
                    mean2 = s2 / 25.0;
                    double b1, b2;
                    a = 0; b1 = 0; b2 = 0;
                    for (int m = 0; m < 5; m++)
                        for (int n = 0; n < 5; n++)
                        {
                            a = a + (tezhengmuban1[m, n] - mean1) * (tezhengmuban2[m, n] - mean2);
                            b1 = b1 + (tezhengmuban1[m, n] - mean1) * (tezhengmuban1[m, n] - mean1);
                            b2 = b2 + (tezhengmuban2[m, n] - mean2) * (tezhengmuban2[m, n] - mean2);
                        }
                    b = Math.Sqrt(b1 * b2);
                    double p = 0;
                    p = a / b;

                    if (p >= Maxcor)
                    {
                        Maxcor = p; ii = i; jj = j;
                    }

                }
                if (Maxcor >= 0.96) //max<0.97  这个应该执行
                {
                    rp[k, 0] = Maxcor; 
                    FeaturePoints[k, 1] = t1[ii, 1]; 
                    FeaturePoints[k, 2] = t1[ii, 2];
                    FeaturePoints[k, 3] = t2[jj, 1];
                    FeaturePoints[k, 4] = t2[jj, 2];
                    k++; 
                }
            }
            progressBar1.Value = 70;
            Draw();
            progressBar1.Value = 100;


        }

        private void 特征点信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            特征点提取Form 特征点提取 = new 特征点提取Form();
            特征点提取.xs(FeaturePoints, k);
            特征点提取.ShowDialog();
        }

        private void 图像拼接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //角点坐标
            double[,] Ixy = new double[5, 3];
            Ixy[1, 1] = 0; Ixy[1, 2] = 0;
            Ixy[2, 1] = w1; Ixy[2, 2] = 0;
            Ixy[3, 1] = 0; Ixy[3, 2] = h1;
            Ixy[4, 1] = w1; Ixy[4, 2] = h1;
            //角点标准坐标
            double[,] XY = new double[5, 3];
            for (int i = 1; i < 5; i++)
            {
                XY[i, 1] = Ixy[i, 1] * ax + bx;
                XY[i, 2] = Ixy[i, 2] * ay + by;
            }
            //输出影像最小范围
            double Xmin = (XY[1, 1] < XY[3, 1] ? XY[1, 1] : XY[3, 1]);
            double Xmax = (XY[2, 1] > XY[4, 1] ? XY[2, 1] : XY[4, 1]);
            double Ymin = (XY[1, 2] < XY[2, 2] ? XY[1, 2] : XY[2, 2]);
            double Ymax = (XY[3, 2] > XY[4, 2] ? XY[3, 2] : XY[4, 2]);
            Xmin = (Xmin < 0 ? Xmin : 0) - 5;
            Xmax = (Xmax > w2 ? Xmax : w2) + 5;
            Ymin = (Ymin < 0 ? Ymin : 0) - 5;
            Ymax = (Ymax > h2 ? Ymax : h2) + 5;
            //Xmax = w2;
            //Ymin = 0;
            //Ymax = h2;
            int scw = (int)((Xmax - Xmin) / 1);
            int sch = (int)((Ymax - Ymin) / 1);
            //构建位图
            double Xp, Yp;
            double x, y;
            int i0, j0;
            Bitmap bitmap0 = new Bitmap(scw, sch, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Color curColor;
            for (int i = 0; i < scw; i++)
                for (int j = 0; j < sch; j++)
                {
                    Xp = i * 1.0 + Xmin;
                    // Yp = (sch - j - 1) * 1.0 + Ymin;
                    Yp = j * 1.0 + Ymin;
                    if (Xp >= 0 && Xp < w2 && Yp >= 0 && Yp < h2)
                    {
                        curColor = bitmap2.GetPixel((int)Xp, (int)Yp);
                        bitmap0.SetPixel(i, j, curColor);
                    }
                    else
                    {
                        x = (Xp - bx) / ax;
                        y = (Yp - by) / ay;
                        i0 = (int)(x); j0 = (int)(y);
                        if (i0 >= 0 && i0 < w1 && j0 >= 0 && j0 < h1)
                        {
                            curColor = bitmap1.GetPixel(i0, j0);
                            bitmap0.SetPixel(i, j, curColor);
                        }

                        else
                            bitmap0.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                    }
                }
            影像显示Form f = new 影像显示Form();
            f.ShowBitmap(bitmap0);
            f.Show();
            //pictureBox1.Image = bitmap0;                  
        }

        private void 计算变换系数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            var Bx = DenseMatrix.OfArray(new[,] { { FeaturePoints[1, 1] * 1.0, 1.0 }, { FeaturePoints[11, 1] * 1.0, 1.0 }, { FeaturePoints[21, 1] * 1.0, 1.0 }, { FeaturePoints[31, 1] * 1.0, 1.0 } });
            var Lx = DenseMatrix.OfArray(new[,] { { FeaturePoints[1, 3] * 1.0 }, { FeaturePoints[11, 3] * 1.0 }, { FeaturePoints[21, 3] * 1.0 }, { FeaturePoints[31, 3] * 1.0 } });
            var x = (Bx.Transpose() * Bx).Inverse() * (Bx.Transpose() * Lx);
            ax = x[0, 0];
            bx = x[1, 0];
            //matrixB = matrixA.Inverse()/Transpose()  //求逆和转置
            var By = DenseMatrix.OfArray(new[,] { { FeaturePoints[1, 2] * 1.0, 1.0 }, { FeaturePoints[11, 2] * 1.0, 1.0 }, { FeaturePoints[21, 2] * 1.0, 1.0 }, { FeaturePoints[31, 2] * 1.0, 1.0 } });
            var Ly = DenseMatrix.OfArray(new[,] { { FeaturePoints[1, 4] * 1.0 }, { FeaturePoints[11, 4] * 1.0 }, { FeaturePoints[21, 4] * 1.0 }, { FeaturePoints[31, 4] * 1.0 } });
            var y = (By.Transpose() * By).Inverse() * (By.Transpose() * Ly);
            ay = y[0, 0];
            by = y[1, 0];
            MessageBox.Show("变换系数计算完毕！\n" + ax.ToString() + "\n" + bx.ToString() + "\n" + ay.ToString() + "\n" + by.ToString(), "提示：", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        
             
        }

        private void 地理配准Form_Load(object sender, EventArgs e)
        {
            OSGeo.GDAL.Gdal.AllRegister();
        }


    }
}

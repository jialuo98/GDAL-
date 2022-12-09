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

namespace 遥感图像处理作业_1
{
    public partial class 监督分类Form : Form
    {
        OSGeo.GDAL.Dataset dataset;//
        int w, h;//Bitmap的长宽
        MapForm pMapForm = null; //地图窗口
        //MainForm pMianForm = null;//主窗口
        Bitmap CurBitmap;//pmapform的bitmap
        Bitmap bitmap;//原始bitmap

        int SampleNum = 0;//样本类别数
        bool startDraw; //矩形框图标函数
        double[,] SampleConter = new double[15, 5];//样本中心//总像元个数,R,G,B,band4z;
        string[] SampleName = new string[15];  //样本名字
        Color[] SampleColor = new Color[15];   //样本颜色
        Color selectcolor;//颜色选择
        //画矩形框
        Point start; //画框的起始点
        Point end;//画框的结束点
        bool blnDraw;//判断是否绘制
        System.Drawing.Rectangle rect;
        //临时存放灰度
        int[] tempArray1 = new int[256];  
        int[] tempArray2 = new int[256];
        int[] tempArray3 = new int[256];

        #region GetBitmap
        int[] r, g, b, band4z;
        double[] maxandmin1 = { 0, 0 };
        double[] maxandmin2 = { 0, 0 };
        double[] maxandmin3 = { 0, 0 };
        double[] maxandmin4 = { 0, 0 };
        #endregion

        public 监督分类Form()
        {
            InitializeComponent();
        }
        private Bitmap GetImage(OSGeo.GDAL.Dataset ds, int[] bandlist)//非监督分类getImage
        {
            int width = ds.RasterXSize;
            int height = ds.RasterYSize;
            if (width > 1000 && height > 1000)
            {
                w = width / 4;
                h = height / 4;
            }
            else
            {
                w = width;
                h = height;
            }
            r = new int[w * h];
            g = new int[w * h];
            b = new int[w * h];
            band4z = new int[w * h];///  
            //构建位图
            Bitmap bitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            if (ds.RasterCount > 3)
            {
                Band band4 = ds.GetRasterBand(4);///
                band4.ComputeRasterMinMax(maxandmin4, 0);///
                band4.ReadRaster(0, 0, width, height, band4z, w, h, 0, 0);
            }
            if (bandlist.Length == 3)     //RGB显示
            {
                Band band1 = ds.GetRasterBand(bandlist[0]);
                band1.ReadRaster(0, 0, width, height, r, w, h, 0, 0);  //读取图像
                band1.ComputeRasterMinMax(maxandmin1, 0);//

                Band band2 = ds.GetRasterBand(bandlist[1]);
                band2.ReadRaster(0, 0, width, height, g, w, h, 0, 0);
                band2.ComputeRasterMinMax(maxandmin2, 0);//

                Band band3 = ds.GetRasterBand(bandlist[2]);
                band3.ReadRaster(0, 0, width, height, b, w, h, 0, 0);
                band3.ComputeRasterMinMax(maxandmin3, 0);//

                int i, j, rVal, gVal, bVal;
                for (i = 0; i < w; i++)
                {
                    for (j = 0; j < h; j++)
                    {
                        rVal = Convert.ToInt32(r[i + j * w]);
                        rVal = (int)((rVal - maxandmin1[0]) / (maxandmin1[1] - maxandmin1[0]) * 255);


                        gVal = Convert.ToInt32(g[i + j * w]);
                        gVal = (int)((gVal - maxandmin2[0]) / (maxandmin2[1] - maxandmin2[0]) * 255);


                        bVal = Convert.ToInt32(b[i + j * w]);
                        bVal = (int)((bVal - maxandmin3[0]) / (maxandmin3[1] - maxandmin3[0]) * 255);


                        Color newColor = Color.FromArgb(rVal, gVal, bVal);
                        bitmap.SetPixel(i, j, newColor);
                    }
                }
            }
            else//灰度显示
            {
                Band band1 = ds.GetRasterBand(bandlist[0]);
                band1.ReadRaster(0, 0, width, height, r, w, h, 0, 0);
                double[] maxandmin1 = { 0, 0 };
                band1.ComputeRasterMinMax(maxandmin1, 0);

                int i, j, rVal;
                for (i = 0; i < w; i++)
                {
                    for (j = 0; j < h; j++)
                    {
                        rVal = Convert.ToInt32(r[i + j * w]);
                        rVal = (int)((rVal - maxandmin1[0]) / (maxandmin1[1] - maxandmin1[0]) * 255);

                        Color newColor = Color.FromArgb(rVal, rVal, rVal);
                        bitmap.SetPixel(i, j, newColor);
                    }
                }
            }
            return bitmap;
        }
        public void GetpMapBitmap(Bitmap bitmap)
        {
            CurBitmap = bitmap;
        }
        public void GetDataSet(OSGeo.GDAL.Dataset dataset)
        {
            this.dataset = dataset;
        }
        public void GetMapForm(MapForm pMapForm)
        {
            this.pMapForm = pMapForm;
        }
        private void xxbhjs()
        {
            Color curColor;

            int[] countPixel1 = new int[256];

            int[] countPixel2 = new int[256];

            int[] countPixel3 = new int[256];

            int i, j; int bytes = w * h;
            for (i = 0; i < w; i++)
                for (j = 0; j < h; j++)
                {
                    curColor = CurBitmap.GetPixel(i, j);
                    countPixel1[curColor.R]++; countPixel2[curColor.G]++; countPixel3[curColor.B]++;
                }
            //Count the ratio acculation function in each gray level
            for (i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    tempArray1[i] = tempArray1[i - 1] + countPixel1[i];
                    tempArray2[i] = tempArray2[i - 1] + countPixel2[i];
                    tempArray3[i] = tempArray3[i - 1] + countPixel3[i];
                }
                else
                {
                    tempArray1[0] = countPixel1[0];
                    tempArray2[0] = countPixel2[0];
                    tempArray3[0] = countPixel3[0];
                }
            }

        }
        private void MinDistance(ref Bitmap bitmap)//监督分类(最小距离)
        {
            int n;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    n = bifen(r[i + j * w], g[i + j * w], b[i + j * w], band4z[i + j * w]);
                    bitmap.SetPixel(i, j, SampleColor[n]);
                }
            }
            pictureBox1.Image = bitmap;
        }
        private int bifen(int r1, int b1, int g1, int band4z1)
        {
            int min = (int)(Math.Pow(r1 - SampleConter[0, 1], 2) 
                + Math.Pow(g1 - SampleConter[0, 2], 2)
                + Math.Pow(b1 - SampleConter[0, 3], 2)
                + Math.Pow(band4z1 - SampleConter[0, 4], 2));
            int n = 0;
            for (int i = 1; i < SampleNum; i++)
            {
                int m = (int)(Math.Pow(r1 - SampleConter[i, 1], 2) 
                    + Math.Pow(g1 - SampleConter[i, 2], 2)
                    + Math.Pow(b1 - SampleConter[i, 3], 2)
                    + Math.Pow(band4z1 - SampleConter[i, 4], 2));
                if (m < min)
                {
                    min = m;
                    n = i;
                }
            }
            return n;
        }
        private void SupervisedClassForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = CurBitmap;
            int[] disband = { 1, 2, 3 };
            bitmap = GetImage(dataset, disband);
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            /*
            w = (int)(w * 1.2);
            h = (int)(h * 1.2);
            if (r0 == 0)

                bitmap = GetImage(dataset, disband);
            else
                bitmap = ReadImage(dataset, disband);
            pictureBox1.Image = bitmap;
            xxbhjs();
            */
        }
        private void New_ROI_toolStripButton6_Click(object sender, EventArgs e)
        {
            toolStripTextBox3.Text = "类别" + (SampleNum + 1).ToString();
            SampleNum++;
        }
        private void Rectangle_toolStripButton5_Click(object sender, EventArgs e)
        {
            startDraw = true;
            this.Cursor = Cursors.Cross; 
        }
        private void Remove_All_ROIs_toolStripButton8_Click(object sender, EventArgs e)
        {
            SampleNum = 0;
            SampleConter = new double[15, 5];
            SampleColor = new Color[15];
            //bitmap = bitmap1;
            pictureBox1.Image = CurBitmap;
        }
        private void 颜色toolStripButton7_Click(object sender, EventArgs e)
        {
            ColorDialog loColorForm = new ColorDialog();
            loColorForm.ShowDialog();
            Graphics g = pictureBox2.CreateGraphics();
            g.Clear(loColorForm.Color);
            selectcolor = loColorForm.Color;  
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            start = e.Location;
            if (startDraw == true)
                blnDraw = true;
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (blnDraw)
            {
                if (pictureBox1.Image != null)
                {
                    if (rect != null && rect.Width > 0 && rect.Height > 0)
                    {
                        Graphics g0 = pictureBox1.CreateGraphics();
                        g0.FillRectangle(new SolidBrush(selectcolor), rect);//重新绘制颜色为红色
                    }
                }
            }
            blnDraw = false; //结束绘制
            //存储样本选择颜色
            SampleColor[SampleNum] = selectcolor;
            //存储样本类别名
            SampleName[SampleNum] = toolStripTextBox3.Text;
            //更新聚类中心
            for (int i = start.X; i <= start.X + rect.Width; i++)
                for (int j = start.Y; j <= start.Y + rect.Height; j++)
                {

                    //int i0=(int)(1.0*i*width/w);
                    //int j0 = (int)(1.0 * j * height / h);
                    int k0 = i + j * w;
                    SampleConter[SampleNum, 1] = (SampleConter[SampleNum, 1] * SampleConter[SampleNum, 0] + r[k0]) / (SampleConter[SampleNum, 0] + 1); //r,g,b,band4z为空
                    SampleConter[SampleNum, 2] = (SampleConter[SampleNum, 2] * SampleConter[SampleNum, 0] + g[k0]) / (SampleConter[SampleNum, 0] + 1);
                    SampleConter[SampleNum, 3] = (SampleConter[SampleNum, 3] * SampleConter[SampleNum, 0] + b[k0]) / (SampleConter[SampleNum, 0] + 1);
                    SampleConter[SampleNum, 4] = (SampleConter[SampleNum, 4] * SampleConter[SampleNum, 0] + band4z[k0]) / (SampleConter[SampleNum, 0] + 1);///
                    SampleConter[SampleNum, 0]++;
                }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //toolStripTextBox2.Text = "X：" + e.X.ToString() + " Y：" + e.Y.ToString();
            if (blnDraw)
            {
                if (e.Button != MouseButtons.Left)//判断是否按下左键
                    return;
                end = e.Location; //记录框的位置和大小
                rect.Location = new Point(
                Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y));
                rect.Size = new Size(
                Math.Abs(start.X - end.X),
                Math.Abs(start.Y - end.Y));
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            MinDistance(ref bitmap);
            pictureBox1.Image = bitmap;
        }
        private void 保存_Click(object sender, EventArgs e)
        {
            保存Form 保存 = new 保存Form();
            保存.Show();
            //OSGeo.GDAL.Dataset Ds = Gdal.Open(toolStripTextBox1.Text, Access.GA_Update);
        }

        public Bitmap getbitmap { get { return bitmap; } }

    }
}

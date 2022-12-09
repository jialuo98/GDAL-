using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OSGeo.GDAL;
using Gdal = OSGeo.GDAL.Gdal;
using Ogr = OSGeo.OGR.Ogr;
using WeifenLuo.WinFormsUI.Docking;


namespace 遥感图像处理作业_1
{
    public partial class 图像融合Form : DockContent
    {

        OSGeo.GDAL.Dataset dataset1, dataset2;
        int imgWidth1, imgHeight1;
        int imgWidth2, imgHeight2;
        int w  , h ;  //确保全色和多光谱的bitmap大小一样

        Bitmap bitmap1;
        Bitmap bitmap2, bitmap20;//,bitmap2zhh;
        Bitmap bitmap3;

        int[, ,] IHSz; //   IHSZ函数
        int[, ,] I;// zftpp函数
        影像显示Form 影像显示 = new 影像显示Form();

        public 图像融合Form()
        {
            InitializeComponent();
        }

        private Bitmap GetImage(OSGeo.GDAL.Dataset ds, Rectangle showRect, int[] bandlist)
        {
            int imgWidth = ds.RasterXSize;   //影像宽  
            int imgHeight = ds.RasterYSize;  //影像高  

            float ImgRatio = imgWidth / (float)imgHeight;  //影像宽高比  

            //获取显示控件大小  
            int BoxWidth = showRect.Width;
            int BoxHeight = showRect.Height;
            float BoxRatio = imgWidth / (float)imgHeight;  //显示控件宽高比  

            //计算实际显示区域大小，防止影像畸变显示
            int BufferWidth, BufferHeight;
            if (BoxRatio >= ImgRatio)
            {
                BufferHeight = BoxHeight;
                BufferWidth = (int)(BoxHeight * ImgRatio);
            }
            else
            {
                BufferWidth = BoxWidth;
                BufferHeight = (int)(BoxWidth / ImgRatio);
            }
            int[,] RGB = new int[3, BufferWidth * BufferHeight];

            //构建位图  
            w = BufferWidth;
            h = BufferHeight;
            Bitmap bitmap = new Bitmap(BufferWidth, BufferHeight,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);  //赋值bitmap并定义为32位位图

            //bandlist.length表示数组bandlist的长度
            if (ds.RasterCount >= 3)     //RGB显示  
            {
                int[] r = new int[BufferWidth * BufferHeight];
                Band band1 = ds.GetRasterBand(bandlist[0]);//bandlist[0]=1 band1为dataset类的第一个波段　R？
                band1.ReadRaster(0, 0, imgWidth, imgHeight, r, BufferWidth, BufferHeight, 0, 0);  //读取图像到内存
                //为了显示好看，进行最大最小值拉伸显示  
                double[] maxandmin1 = { 0, 0 };
                band1.ComputeRasterMinMax(maxandmin1, 0);

                int[] g = new int[BufferWidth * BufferHeight];
                Band band2 = ds.GetRasterBand(bandlist[1]);
                band2.ReadRaster(0, 0, imgWidth, imgHeight, g, BufferWidth, BufferHeight, 0, 0);

                double[] maxandmin2 = { 0, 0 };
                band2.ComputeRasterMinMax(maxandmin2, 0);

                int[] b = new int[BufferWidth * BufferHeight];
                Band band3 = ds.GetRasterBand(bandlist[2]);
                band3.ReadRaster(0, 0, imgWidth, imgHeight, b, BufferWidth, BufferHeight, 0, 0);
                double[] maxandmin3 = { 0, 0 };
                band3.ComputeRasterMinMax(maxandmin3, 0);

                int i, j;
                for (i = 0; i < BufferWidth; i++)
                {
                    for (j = 0; j < BufferHeight; j++)
                    {
                        RGB[0, i * BufferHeight + j] = r[i * BufferHeight + j];
                        RGB[1, i * BufferHeight + j] = g[i * BufferHeight + j];
                        RGB[2, i * BufferHeight + j] = b[i * BufferHeight + j];
                    }
                }
                for (i = 0; i < BufferWidth; i++)
                {
                    for (j = 0; j < BufferHeight; j++)
                    {
                        int rVal = Convert.ToInt32(r[i + j * BufferWidth]);
                        rVal = (int)((rVal - maxandmin1[0]) / (maxandmin1[1] - maxandmin1[0]) * 255);

                        int gVal = Convert.ToInt32(g[i + j * BufferWidth]);
                        gVal = (int)((gVal - maxandmin2[0]) / (maxandmin2[1] - maxandmin2[0]) * 255);

                        int bVal = Convert.ToInt32(b[i + j * BufferWidth]);
                        bVal = (int)((bVal - maxandmin3[0]) / (maxandmin3[1] - maxandmin3[0]) * 255);

                        Color newColor = Color.FromArgb(rVal, gVal, bVal);
                        bitmap.SetPixel(i, j, newColor);
                    }
                }
            }
            else               //灰度显示  
            {
                int[] r = new int[BufferWidth * BufferHeight];
                Band band1 = ds.GetRasterBand(bandlist[0]);//bandlist[]={3,2,1}
                band1.ReadRaster(0, 0, imgWidth, imgHeight, r, BufferWidth, BufferHeight, 0, 0);

                double[] maxandmin1 = { 0, 0 };
                band1.ComputeRasterMinMax(maxandmin1, 0);

                int i, j;
                for (i = 0; i < BufferWidth; i++)
                {
                    for (j = 0; j < BufferHeight; j++)
                    {
                        int rVal = Convert.ToInt32(r[i + j * BufferWidth]);
                        rVal = (int)((rVal - maxandmin1[0]) / (maxandmin1[1] - maxandmin1[0]) * 255);

                        Color newColor = Color.FromArgb(rVal, rVal, rVal); //基于指定的 8 位颜色值（红色、绿色和蓝色）创建 Color 结构
                        bitmap.SetPixel(i, j, newColor);  //获取此 Bitmap 中指定像素的颜色。
                    }
                }
            }

            return bitmap;
        }
        private void  Brovey()
        {

            Color color1, color2;
            int i, j, rVal, gVal, bVal,s;
            Bitmap bitmap0 = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            for ( i = 0; i < w; i++)
            {
                for ( j = 0; j < h; j++)
                {
                    color1 = bitmap1.GetPixel(i, j);  //bitmap1 为单波段
                    color2 = bitmap2.GetPixel(i, j);  //bitmap2 为多波段
                    s = color2.R + color2.G + color2.B;
                    rVal = (int)((1.0 * color1.R * color2.R) / s + 0.5);
                    gVal = (int)((1.0 * color1.R * color2.G) / s + 0.5);
                    bVal = (int)((1.0 * color1.R * color2.B) / s + 0.5);
                    if (rVal < 1)
                        rVal = 0;
                    if (gVal < 1)
                        gVal = 0;
                    if (bVal < 1)
                        bVal = 0;

                    Color newColor = Color.FromArgb(rVal, gVal, bVal);    
                    bitmap0.SetPixel(i, j, newColor);
                }
            }
            bitmap3 = bitmap0;
            影像显示.ShowBitmap(bitmap3);
            影像显示.ShowDialog();
           // pictureBox1.Image = bitmap3;
        }
        private void IHS()
        {

            ihsz();

            zftpp();

            bitmap3 = ishn();

            //pictureBox1.Image = bitmap3;

            影像显示.ShowBitmap(bitmap3);
            影像显示.ShowDialog();
        }
        public int min(int a, int b, int c)
        {
            int d = (a <= b ? a : b);
            return (d <= c ? d : c);
        }
        public void ihsz()
        {
            IHSz = new int[w, h, 3];
            Color color;
            int i, j, m;
            for (i = 0; i < w; i++)
            {
                for (j = 0; j < h; j++)
                {
                    color = bitmap2.GetPixel(i, j);
                    IHSz[i, j, 0] = color.R + color.G + color.B;
                    m = min(color.R, color.G, color.B);
                    if (color.B == m)
                        IHSz[i, j, 1] = (int)(1.0 * (color.G - color.B) / (IHSz[i, j, 0] - 3 * color.B) + 0.5);
                    else
                        if (color.R == m)
                            IHSz[i, j, 1] = (int)(1.0 * (color.B - color.R) / (IHSz[i, j, 0] - 3 * color.R) + 1.5);
                        else
                            IHSz[i, j, 1] = (int)(1.0 * (color.R - color.G) / (IHSz[i, j, 0] - 3 * color.G) + 2.5);

                    if (IHSz[i, j, 1] < 1)
                        IHSz[i, j, 1] = (int)((IHSz[i, j, 0] - 3 * color.B) / IHSz[i, j, 0] + 0.5);
                    else
                        if (IHSz[i, j, 1] < 2)
                            IHSz[i, j, 1] = (int)((IHSz[i, j, 0] - 3 * color.R) / IHSz[i, j, 0] + 0.5);
                        else
                            IHSz[i, j, 1] = (int)((IHSz[i, j, 0] - 3 * color.G) / IHSz[i, j, 0] + 0.5);
                }
            }
        }
        public Bitmap ishn()
        {
            Color color; int rVal, gVal, bVal;
            Bitmap bitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                {
                    if (IHSz[i, j, 1] < 1)
                    {
                        rVal = (int)(1.0 * IHSz[i, j, 0] * (1 + 2 * IHSz[i, j, 2] - 3 * IHSz[i, j, 1] * IHSz[i, j, 2]) / 3 + 0.5);
                        gVal = (int)(1.0 * IHSz[i, j, 0] * (1 - IHSz[i, j, 2] + 3 * IHSz[i, j, 1] * IHSz[i, j, 2]) / 3 + 0.5);
                        bVal = (int)(1.0 * IHSz[i, j, 0] * (1 - IHSz[i, j, 2]) / 3 + 0.5);
                    }
                    else
                        if (IHSz[i, j, 1] < 2)
                        {
                            rVal = (int)(1.0 * IHSz[i, j, 0] * (1 - IHSz[i, j, 2]) / 3 + 0.5);
                            gVal = (int)(1.0 * IHSz[i, j, 0] * (1 + 2 * IHSz[i, j, 2] - 3 * (IHSz[i, j, 1] - 1) * IHSz[i, j, 2]) / 3 + 0.5);
                            bVal = (int)(1.0 * IHSz[i, j, 0] * (1 - IHSz[i, j, 2] - 3 * (IHSz[i, j, 1] - 1) * IHSz[i, j, 2]) / 3 + 0.5);
                        }
                        else
                        {
                            rVal = (int)(1.0 * IHSz[i, j, 0] * (1 - IHSz[i, j, 2] - 3 * (IHSz[i, j, 1] - 2) * IHSz[i, j, 2]) / 3 + 0.5);
                            gVal = (int)(1.0 * IHSz[i, j, 0] * (1 - IHSz[i, j, 2]) / 3 + 0.5);
                            bVal = (int)(1.0 * IHSz[i, j, 0] * (1 + 2 * IHSz[i, j, 2] - 3 * (IHSz[i, j, 1] - 2) * IHSz[i, j, 2]) / 3 + 0.5);
                        }
                    color = Color.FromArgb(rVal, gVal, bVal);
                    bitmap.SetPixel(i, j, color);
                }
            return bitmap;
        }
        public void zftpp()
        {
            int bytes = w * h;   //Three bands total       
            I = new int[w, h, 1];
            int i, j;
            Color curColor;
            int[] tempArrayI = new int[256];
            int[] countPixelI = new int[256];
            double[] pixelMapI = new double[256];
            int[] tempArrayIHS = new int[256 * 3 - 2];
            int[] countPixelIHS = new int[256 * 3 - 2];
            double[] pixelMapIHS = new double[256 * 3 - 2];
            //Calculate the pixel numbers in each gray scale level
            for (i = 0; i < w; i++)
                for (j = 0; j < h; j++)
                {
                    curColor = bitmap1.GetPixel(i, j);
                    countPixelI[curColor.R]++;
                    countPixelIHS[IHSz[i, j, 0]]++;
                    I[i, j, 0] = curColor.R;
                }
            //Count the ratio acculation function in each gray level
            for (i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    tempArrayI[i] = tempArrayI[i - 1] + countPixelI[i];
                }
                else
                {
                    tempArrayI[0] = countPixelI[0];
                }
                pixelMapI[i] = 1.0 * tempArrayI[i] / bytes;
            }

            for (i = 0; i < 256 * 3 - 2; i++)
            {
                if (i != 0)
                {
                    tempArrayIHS[i] = tempArrayIHS[i - 1] + countPixelIHS[i];
                }
                else
                {
                    tempArrayIHS[0] = countPixelIHS[0];
                }
                pixelMapIHS[i] = 1.0 * tempArrayIHS[i] / bytes;
            }

            //映射函数 
            int[] PixelI = new int[256];
            int PG = 0;
            for (i = 0; i < 256; ++i)
            {
                for (j = 0; j < 255 * 3 - 1; ++j)
                {
                    if ((pixelMapIHS[j] - pixelMapI[i]) >= 0)
                    {
                        PG = j; break;
                    }

                }
                PixelI[i] = PG;
            }

            for (i = 0; i < w; i++)
                for (j = 0; j < h; j++)
                {
                    IHSz[i, j, 0] = PixelI[I[i, j, 0]];
                }

        }
        private void 打开全色影像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileName = new OpenFileDialog();
            FileName.Filter = "remote sensing image|*.tif;*.img";
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
                imgWidth1 = dataset1.RasterXSize;
                imgWidth2 = dataset1.RasterYSize;

                int[] disband = { 1,2,3 };
                Rectangle pictureRect = new Rectangle();
                pictureRect.X = 0;
                pictureRect.Y = 0;
                pictureRect.Width = this.pictureBox1.Width;
                pictureRect.Height = this.pictureBox1.Height;

                bitmap1 = GetImage(dataset1, pictureRect, disband);

                pictureBox1.Image = bitmap1;

            }
        }
        private void 打开多光谱影像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //需要重采样
            OpenFileDialog FileName = new OpenFileDialog();
            FileName.Filter = "remote sensing image|*.tif;*.img";
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
                imgHeight1 = dataset2.RasterXSize;
                imgHeight2 = dataset2.RasterYSize;


                int[] disband = { 1, 2, 3 };//
                Rectangle pictureRect = new Rectangle();
                pictureRect.X = 0;
                pictureRect.Y = 0;
                pictureRect.Width = this.pictureBox2.Width;
                pictureRect.Height = this.pictureBox2.Height;

                bitmap2 = GetImage(dataset2,pictureRect, disband);
 
                //bitmap20 = GetImage(dataset2);

                pictureBox2.Image = bitmap2;

            }
        }
        private void Brovey融合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Brovey();
        }
        private void IHS融合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IHS();
        }
        private void 图像融合Form_Load(object sender, EventArgs e)
        {
            OSGeo.GDAL.Gdal.AllRegister();           //第一行是注册所有的格式驱动，
            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");   //第二行是支持中文路径和名称
        }
    }
}

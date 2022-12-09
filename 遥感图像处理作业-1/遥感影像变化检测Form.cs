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
    public partial class 遥感影像变化检测Form : Form
    {
        Dataset dataset1, dataset2;
        Bitmap bitmap1, bitmap2;
        public 遥感影像变化检测Form()
        {
            InitializeComponent();
        }
        public Bitmap GetImage(OSGeo.GDAL.Dataset ds, Rectangle showRect, int[] bandlist)
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
            //构建位图  
            Bitmap bitmap = new Bitmap(BufferWidth, BufferHeight,
                                     System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            if (ds.RasterCount >= 3)     //RGB显示  
            {
                int[] r = new int[BufferWidth * BufferHeight];
                Band band1 = ds.GetRasterBand(bandlist[0]);
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
                Band band1 = ds.GetRasterBand(bandlist[0]);
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

                        Color newColor = Color.FromArgb(rVal, rVal, rVal);
                        bitmap.SetPixel(i, j, newColor);
                    }
                }
            }

            return bitmap;
        }
        public Bitmap CImage()
        {
            int i, j;
            Bitmap bitmapc = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Bitmap bit1 = GetGray(bitmap1);
            Bitmap bit2 = GetGray(bitmap2);
            if (bitmap1 != null && bitmap2 != null)
            {
                Color curColor1, curColor2;
                int ret;
                for (i = 0; i < bitmap1.Width; i++)
                {
                    for (j = 0; j < bitmap1.Height; j++)
                    {
                        curColor1 = bit1.GetPixel(i, j);
                        curColor2 = bit2.GetPixel(i, j);
                        ret = Math.Abs(curColor1.R - curColor2.R);
                        bitmapc.SetPixel(i, j, Color.FromArgb(ret, ret, ret));
                    }
                }
            }
            return bitmapc;
        }
        public Bitmap GetGray(Bitmap curBitmap)//获取/curBitmap的像素灰度值
        {
            int i, j;
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
                        bitmapgray.SetPixel(i, j, Color.FromArgb(ret, ret, ret));
                    }
                }
            }
            return bitmapgray;
        }
        private void 打开变化前影像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileName = new OpenFileDialog();
            FileName.Filter = "remote sensing image|*.tif;*.img";
            if (FileName.ShowDialog() == DialogResult.OK)
            {
                OSGeo.GDAL.Gdal.AllRegister();
                dataset1 = OSGeo.GDAL.Gdal.Open(FileName.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
                Rectangle pictureRect = new Rectangle();
                pictureRect.X = 0;
                pictureRect.Y = 0;

                pictureRect.Width = this.pictureBox1.Width;
                pictureRect.Height = this.pictureBox1.Height;
                int[] disband = { 1, 2, 3 };
                bitmap1 = GetImage(dataset1, pictureRect, disband);

                pictureBox1.Refresh();
                pictureBox1.Image = bitmap1;
            }
        }
        private void 打开变化后影像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileName = new OpenFileDialog();
            FileName.Filter = "remote sensing image|*.tif;*.img";
            if (FileName.ShowDialog() == DialogResult.OK)
            {
                OSGeo.GDAL.Gdal.AllRegister();
                dataset2 = OSGeo.GDAL.Gdal.Open(FileName.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
                Rectangle pictureRect = new Rectangle();
                pictureRect.X = 0;
                pictureRect.Y = 0;
                pictureRect.Width = this.pictureBox2.Width;
                pictureRect.Height = this.pictureBox2.Height;
                int[] disband = { 1, 2, 3 };
                bitmap2 = GetImage(dataset2, pictureRect, disband);
                pictureBox2.Refresh();
                pictureBox2.Image = bitmap2;
            }
        }
        private void 差值图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bitmap;
            bitmap = CImage();
            影像显示Form 影像显示 = new 影像显示Form();
            影像显示.pictureBox1.Image = bitmap;
            影像显示.Show();
        }
    }
}

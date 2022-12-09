using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OSGeo.GDAL;
using System.Drawing.Imaging;

namespace 遥感图像处理作业_1
{
    public partial class 图像融合Form1: Form
    {
        public 图像融合Form1()
        {
            InitializeComponent();
        }

        public static Band band1;
        private static Band band2;
        private static Band band3;
        private static int imgWidth;
        private static int imgHeight;
        private static int BufWidth;
        private static int BufHeight;
        private static Bitmap bitmap;
        private static string img1, img2;//多源遥感影像  全色影像

        private Bitmap GetBitmap(string ImageName, Rectangle pictureRect)
        {
            OSGeo.GDAL.Dataset dataset = Gdal.Open(ImageName, Access.GA_ReadOnly);
            imgWidth = dataset.RasterXSize;   //影像宽  
            imgHeight = dataset.RasterYSize;  //影像高  

            float ImgRatio = imgWidth / (float)imgHeight;  //影像宽高比  

            //获取显示控件大小  
            int BoxWidth = pictureRect.Width;
            int BoxHeight = pictureRect.Height;

            float BoxRatio = imgWidth / (float)imgHeight;  //显示控件宽高比  

            //计算实际显示区域大小，防止影像畸变显示  
            if (BoxRatio >= ImgRatio)
            {
                BufHeight = BoxHeight;
                BufWidth = (int)(BoxHeight * ImgRatio);
            }
            else
            {
                BufWidth = BoxWidth;
                BufHeight = (int)(BoxWidth / ImgRatio);
            }

            band1 = dataset.GetRasterBand(1);

            if (dataset.RasterCount > 2)
            {
                double[] maxandmin1 = { 0, 0 };
                double[] maxandmin2 = { 0, 0 };
                double[] maxandmin3 = { 0, 0 };
                band2 = dataset.GetRasterBand(2);
                band3 = dataset.GetRasterBand(3);
                bitmap = new Bitmap(BufWidth, BufHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                int[] r = new int[BufWidth * BufHeight];
                int[] b = new int[BufWidth * BufHeight];
                int[] g = new int[BufWidth * BufHeight];
                if (dataset.RasterCount == 4)
                {
                    band1.ReadRaster(0, 0, imgWidth, imgHeight, b, BufWidth, BufHeight, 0, 0);
                    band1.ComputeRasterMinMax(maxandmin3, 0);
                    band2.ReadRaster(0, 0, imgWidth, imgHeight, g, BufWidth, BufHeight, 0, 0);
                    band2.ComputeRasterMinMax(maxandmin2, 0);
                    band3.ReadRaster(0, 0, imgWidth, imgHeight, r, BufWidth, BufHeight, 0, 0);
                    band3.ComputeRasterMinMax(maxandmin1, 0);
                }
                else
                {
                    band1.ReadRaster(0, 0, imgWidth, imgHeight, r, BufWidth, BufHeight, 0, 0);
                    band1.ComputeRasterMinMax(maxandmin1, 0);
                    band2.ReadRaster(0, 0, imgWidth, imgHeight, g, BufWidth, BufHeight, 0, 0);
                    band2.ComputeRasterMinMax(maxandmin2, 0);
                    band3.ReadRaster(0, 0, imgWidth, imgHeight, b, BufWidth, BufHeight, 0, 0);
                    band3.ComputeRasterMinMax(maxandmin3, 0);
                }
                int i, j;
                for (i = 0; i < BufWidth; i++)
                {
                    for (j = 0; j < BufHeight; j++)
                    {
                        int rVal = Convert.ToInt32(r[i + j * BufWidth]);
                        rVal = (int)((rVal - maxandmin1[0]) / (maxandmin1[1] - maxandmin1[0]) * 255);

                        int gVal = Convert.ToInt32(g[i + j * BufWidth]);
                        gVal = (int)((gVal - maxandmin2[0]) / (maxandmin2[1] - maxandmin2[0]) * 255);

                        int bVal = Convert.ToInt32(b[i + j * BufWidth]);
                        bVal = (int)((bVal - maxandmin3[0]) / (maxandmin3[1] - maxandmin3[0]) * 255);
                        Color newColor = Color.FromArgb(rVal, gVal, bVal);
                        bitmap.SetPixel(i, j, newColor);
                    }
                }
                return bitmap;
            }
            else
            {
                bitmap = new Bitmap(BufWidth, BufHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                int[] r = new int[BufWidth * BufHeight];
                band1.ReadRaster(0, 0, imgWidth, imgHeight, r, BufWidth, BufHeight, 0, 0);
                double[] maxandmin1 = { 0, 0 };
                band1.ComputeRasterMinMax(maxandmin1, 0);
                int i, j;
                for (i = 0; i < BufWidth; i++)
                {
                    for (j = 0; j < BufHeight; j++)
                    {
                        int rVal = Convert.ToInt32(r[i + j * BufWidth]);
                        rVal = (int)((rVal - maxandmin1[0]) / (maxandmin1[1] - maxandmin1[0]) * 255);
                        Color newColor = Color.FromArgb(rVal, rVal, rVal);
                        bitmap.SetPixel(i, j, newColor);
                    }
                }
                return bitmap;
            }
        }
        private void SaveBitmap(string name, int index)//保存
        {
            OSGeo.GDAL.Dataset dataset = Gdal.Open(name, Access.GA_ReadOnly);
            int width = dataset.RasterXSize;
            int height = dataset.RasterYSize;
            if (dataset.RasterCount >= 3)
            {
                double[] maxandmin11 = { 0, 0 };
                double[] maxandmin12 = { 0, 0 };
                double[] maxandmin13 = { 0, 0 };
                Band band13 = dataset.GetRasterBand(1);
                Band band12 = dataset.GetRasterBand(2);
                Band band11 = dataset.GetRasterBand(3);
                bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                int[] r1 = new int[width * height];
                int[] b1 = new int[width * height];
                int[] g1 = new int[width * height];
                if (dataset.RasterCount == 4)
                {
                    band11.ReadRaster(0, 0, width, height, b1, width, height, 0, 0);
                    band11.ComputeRasterMinMax(maxandmin13, 0);
                    band12.ReadRaster(0, 0, width, height, g1, width, height, 0, 0);
                    band12.ComputeRasterMinMax(maxandmin12, 0);
                    band13.ReadRaster(0, 0, width, height, r1, width, height, 0, 0);
                    band13.ComputeRasterMinMax(maxandmin11, 0);
                }
                else
                {
                    band11.ReadRaster(0, 0, width, height, r1, width, height, 0, 0);
                    band11.ComputeRasterMinMax(maxandmin11, 0);
                    band12.ReadRaster(0, 0, width, height, g1, width, height, 0, 0);
                    band12.ComputeRasterMinMax(maxandmin12, 0);
                    band13.ReadRaster(0, 0, width, height, b1, width, height, 0, 0);
                    band13.ComputeRasterMinMax(maxandmin13, 0);
                }
                int i, j;
                for (i = 0; i < width; i++)
                {
                    for (j = 0; j < height; j++)
                    {
                        int rVal = Convert.ToInt32(r1[i + j * width]);
                        rVal = (int)((rVal - maxandmin11[0]) / (maxandmin11[1] - maxandmin11[0]) * 255);

                        int gVal = Convert.ToInt32(g1[i + j * width]);
                        gVal = (int)((gVal - maxandmin12[0]) / (maxandmin12[1] - maxandmin12[0]) * 255);

                        int bVal = Convert.ToInt32(b1[i + j * width]);
                        bVal = (int)((bVal - maxandmin13[0]) / (maxandmin13[1] - maxandmin13[0]) * 255);
                        Color newColor = Color.FromArgb(rVal, gVal, bVal);
                        bitmap.SetPixel(i, j, newColor);
                    }
                }
            }
            else
            {
                bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                int[] r1 = new int[width * height];
                Band band11 = dataset.GetRasterBand(1);
                band11.ReadRaster(0, 0, width, height, r1, width, height, 0, 0);
                double[] maxandmin11 = { 0, 0 };
                band11.ComputeRasterMinMax(maxandmin11, 0);
                int i, j;
                for (i = 0; i < width; i++)
                {
                    for (j = 0; j < height; j++)
                    {
                        int rVal = Convert.ToInt32(r1[i + j * width]);
                        rVal = (int)((rVal - maxandmin11[0]) / (maxandmin11[1] - maxandmin11[0]) * 255);
                        Color newColor = Color.FromArgb(rVal, rVal, rVal);
                        bitmap.SetPixel(i, j, newColor);
                    }
                }
            }
            if (index == 1)
                bitmap.Save("bt1.tif", ImageFormat.Tiff);
            else if (index == 2)
                bitmap.Save("bt2.tif", ImageFormat.Tiff);
            else return;
        }
        private void 打开多源遥感影像_Click(object sender, EventArgs e)//打开多源遥感影像
        {
            tabControl1.SelectedIndex = 0;
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "remote sensing image|*.tif;*.img;*.hdf|regular digital image|*.jpg;*.png;*.bmp";
            if (file.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                Rectangle rec = new Rectangle(new Point(0, 0), pictureBox1.Size);
                pictureBox1.Image = GetBitmap(file.FileName, rec);
            }
            img1 = file.FileName;
            Cursor = Cursors.Default;
        }
        private void 打开全色影像_Click(object sender, EventArgs e)//打开全色影像
        {
            tabControl1.SelectedIndex = 1;
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "remote sensing image|*.tif;*.img;*.hdf|regular digital image|*.jpg;*.png;*.bmp";
            if (file.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                Rectangle rec = new Rectangle(new Point(0, 0), pictureBox1.Size);
                pictureBox2.Image = GetBitmap(file.FileName, rec);
            }
            img2 = file.FileName;
            Cursor = Cursors.Default;
        }
        private void 融合_Click(object sender, EventArgs e)//融合
        {
            if (pictureBox1.Image == null || pictureBox2.Image == null || toolStripTextBox1.Text == "")
                return;
            Cursor = Cursors.WaitCursor;
            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = 10000;
            SaveBitmap(img1, 1); //保存图像1
            SaveBitmap(img2, 2); //保存图像2
            tabControl1.SelectedIndex = 2;
            int i, j, r3, g3, b3;
            double I, r1, g1, b1;
            Bitmap bt2 = new Bitmap("bt2.tif");
            int w = bt2.Width;
            int h = bt2.Height;
            //bt2.Dispose();
            Bitmap bt1 = new Bitmap("bt1.tif");
            Bitmap bt3 = new Bitmap(bt1, w, h);
            bt1.Dispose();
            Bitmap bit3 = new Bitmap(w, h);
            Color c1 = new Color();
            Color c2 = new Color();
            int stap = 10000 / w;

            #region Brovey变换
            if (toolStripComboBox1.Text == "Brovey变换")
            {
                for (i = 0; i < w; i++)
                {
                    for (j = 0; j < h; j++)
                    {
                        c1 = bt3.GetPixel(i, j);
                        r1 = Convert.ToDouble(c1.R);
                        g1 = Convert.ToDouble(c1.G);
                        b1 = Convert.ToDouble(c1.B);
                        c2 = bt2.GetPixel(i, j);
                        I = c2.GetBrightness() * 255;
                        double Il = Convert.ToDouble((r1 + g1 + b1));
                        if (Il != 0)
                        {
                            r3 = Convert.ToInt32(r1 / Il * I);
                            g3 = Convert.ToInt32(g1 / Il * I);
                            b3 = Convert.ToInt32(b1 / Il * I);
                            Color cc = Color.FromArgb(r3, g3, b3);
                            bit3.SetPixel(i, j, cc);
                        }
                    }
                    toolStripProgressBar1.Value += stap;
                }
                toolStripProgressBar1.Value = 10000;
                bit3.Save(toolStripTextBox1.Text, ImageFormat.Tiff);
                Gdal.AllRegister();
                OSGeo.GDAL.Dataset ds = Gdal.Open(img2, Access.GA_ReadOnly);
                Gdal.AllRegister();
                OSGeo.GDAL.Dataset Ds = Gdal.Open(toolStripTextBox1.Text, Access.GA_Update);
                double[] xy = new double[6];
                ds.GetGeoTransform(xy);
                Ds.SetGeoTransform(xy);
                Rectangle rec = new Rectangle(new Point(0, 0), pictureBox3.Size);
                pictureBox3.Image = GetBitmap(toolStripTextBox1.Text, rec);
                Cursor = Cursors.Default;
                toolStripProgressBar1.Visible = false;
            }
            #endregion

            #region IHS变换
            else if (toolStripComboBox1.Text == "IHS变换")
            {
                if (pictureBox2.Image == null)
                    return;
                Cursor = Cursors.WaitCursor;
                double I1, H1, S1;
                //Bitmap temp = (Bitmap)pictureBox2.Image;
                //Edit.GrayStretch(ref temp);
                for (i = 0; i < w; i++)
                {
                    for (j = 0; j < h; j++)
                    {
                        c1 = bt3.GetPixel(i, j);
                        r1 = Convert.ToDouble(c1.R);
                        g1 = Convert.ToDouble(c1.G);
                        b1 = Convert.ToDouble(c1.B);
                        I1 = r1 + g1 + b1;
                        c2 = bt2.GetPixel(i, j);
                        I = c2.GetBrightness() * 255;
                        if (b1 <= r1 && b1 <= g1)
                            H1 = ((g1 - b1) / (I1 - 3 * b1));
                        else if (r1 <= g1)
                            H1 = ((b1 - r1) / (I1 - 3 * r1)) + 1;
                        else
                            H1 = ((r1 - g1) / (I1 - 3 * g1)) + 2;
                        if (H1 < 1)
                        {
                            S1 = (I1 - 3 * b1) / I1;
                            r3 = Convert.ToInt32(I * (1 + 2 * S1 - 3 * S1 * H1) / 3);
                            g3 = Convert.ToInt32(I * (1 - S1 + 3 * S1 * H1) / 3);
                            b3 = Convert.ToInt32(I * (1 - S1) / 3);
                            Color cc = Color.FromArgb(r3, g3, b3);
                            bit3.SetPixel(i, j, cc);
                        }
                        else if (H1 >= 1 && H1 <= 2)
                        {
                            S1 = (I1 - 3 * r1) / I1;
                            r3 = Convert.ToInt32(I * (1 - S1) / 3);
                            g3 = Convert.ToInt32(I * (1 + 2 * S1 - 3 * S1 * (H1 - 1)) / 3);
                            b3 = Convert.ToInt32(I * (1 - S1 + 3 * S1 * (H1 - 1)) / 3);
                            Color cc = Color.FromArgb(r3, g3, b3);
                            bit3.SetPixel(i, j, cc);
                        }
                        else if (H1 > 2)
                        {
                            S1 = (I1 - 3 * g1) / I1;
                            r3 = Convert.ToInt32(I * (1 - S1 + 3 * S1 * (H1 - 2)) / 3);
                            g3 = Convert.ToInt32(I * (1 - S1) / 3);
                            b3 = Convert.ToInt32(I * (1 + 2 * S1 - 3 * S1 * (H1 - 2)) / 3);
                            Color cc = Color.FromArgb(r3, g3, b3);
                            bit3.SetPixel(i, j, cc);
                        }
                        /**else
                        {
                            bit3.SetPixel(i, j, Color.FromArgb(c1.R, c1.G, c1.B));
                        }*/
                    }
                    toolStripProgressBar1.Value += stap;
                }
            #endregion

                toolStripProgressBar1.Value = 10000;
                bit3.Save(toolStripTextBox1.Text, ImageFormat.Tiff);
                Gdal.AllRegister();
                OSGeo.GDAL.Dataset ds = Gdal.Open(img2, Access.GA_ReadOnly);
                Gdal.AllRegister();
                OSGeo.GDAL.Dataset Ds = Gdal.Open(toolStripTextBox1.Text, Access.GA_Update);
                double[] xy = new double[6];
                ds.GetGeoTransform(xy);
                Ds.SetGeoTransform(xy);
                Rectangle rec = new Rectangle(new Point(0, 0), pictureBox3.Size);
                pictureBox3.Image = GetBitmap(toolStripTextBox1.Text, rec);
                Cursor = Cursors.Default;
                toolStripProgressBar1.Visible = false;
            }
            else
            {
                Cursor = Cursors.Default;
                return;
            }
        }
        private void 选择输出路径_Click(object sender, EventArgs e)//选择输出路径
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "remote sensing image|*.tif";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                toolStripTextBox1.Text = dlg.FileName;
            }
        }
        private void 图像融合Form1_Load(object sender, EventArgs e)
        {
            OSGeo.GDAL.Gdal.AllRegister();           //第一行是注册所有的格式驱动，
            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");   //第二行是支持中文路径和名称
            toolStripProgressBar1.Visible = false;
        }
    }
}

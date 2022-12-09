using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gdal = OSGeo.GDAL.Gdal;
using Ogr = OSGeo.OGR.Ogr;

using OSGeo.GDAL;
using rpaulo.toolbar;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;



namespace 遥感图像处理作业_1
{

    public partial class MainForm : Form
    {
        #region VariablesDeclare
        string strImageList = "ImageList";


        //启动窗口变量   
        private bool bMapformVisible = false;       //地图
        private bool bHistformVisible = false;      //直方图

        private MapForm pMapForm = null; //地图窗口            
        private HistogramWindow pHistForm = null; //直方图窗口
        private MoravecWindow pMoraForm = null; //moravec算子特征点属性窗口

        private ToolBarManager toolBarManager;
        public static MainForm main = new MainForm();  //静态化
        OpenFileDialog ofd = new OpenFileDialog();  //存储遥感影像的位置
        OSGeo.GDAL.Dataset dataset;//定义源dataset
        
        int[,] color = new int[10, 3];//

        #region K_means

        int w, h;
        int[] r, g, b, band4z;
        double[] maxandmin1 = { 0, 0 };
        double[] maxandmin2 = { 0, 0 };
        double[] maxandmin3 = { 0, 0 };
        double[] maxandmin4 = { 0, 0 };

        #endregion

        #region Zoom

        //public int index = 0;//判断当前bitmap进行了哪些操作
        
        #endregion

        #region  MinDistance
        double[,] ybzx = new double[15, 5];//总像元个数,R,G,B,band4z;
        //int ybnum = 0;//样本类别数
        string[] ybname = new string[15];
        Color[] yncolor = new Color[15];
        #endregion

        #endregion

        public MainForm()
        {
            this.Cursor = Cursors.WaitCursor;
            InitializeComponent();
            toolBarManager = new ToolBarManager(this, this);
            ToolBarDockHolder holder;
            // main tool bar
            this.GIStoolStrip.Text = "主工具栏";
            holder = toolBarManager.AddControl(GIStoolStrip);
            holder.AllowedBorders = AllowedBorders.Top | AllowedBorders.Left | AllowedBorders.Right;

            this.menuStrip1.SendToBack();

            pMapForm = new MapForm();         //地图窗口
            pHistForm = new HistogramWindow(); //直方图窗口
            pMoraForm = new MoravecWindow();// moravec算子特征点属性窗口

            this.Cursor = Cursors.Default;
        }

        #region//函数

        /// <summary>
        /// 全等级灰度拉伸
        /// </summary>
        /// <param name="srcBmp">原图像</param>
        /// <param name="dstBmp">处理后图像</param>
        /// <returns>处理成功 true 失败 false</returns>
        private static bool HistogramGrayStretch(Bitmap srcBmp, out Bitmap dstBmp)//灰度拉伸
        {
            if (srcBmp == null)
            {
                dstBmp = null;
                return false;
            }
            double pR = 0.0;//斜率
            double pG = 0.0;//斜率
            double pB = 0.0;//斜率
            //byte minGrayDegree = 255;
            //byte maxGrayDegree = 0;
            byte minGrayDegreeR = 255;
            byte maxGrayDegreeR = 0;
            byte minGrayDegreeG = 255;
            byte maxGrayDegreeG = 0;
            byte minGrayDegreeB = 255;
            byte maxGrayDegreeB = 0;
            dstBmp = new Bitmap(srcBmp);
            Rectangle rt = new Rectangle(0, 0, dstBmp.Width, dstBmp.Height);
            //dstBmp.lockbits将Bitmap锁定到系统内存中
            BitmapData bmpData = dstBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                for (int i = 0; i < bmpData.Height; i++) //设置 Bitmap 对象的像素高度。 有时也称作扫描行数
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        if (minGrayDegreeR > *(ptr + j * 3 + 2))
                            minGrayDegreeR = *(ptr + j * 3 + 2);
                        if (maxGrayDegreeR < *(ptr + j * 3 + 2))
                            maxGrayDegreeR = *(ptr + j * 3 + 2);
                        if (minGrayDegreeG > *(ptr + j * 3 + 1))
                            minGrayDegreeG = *(ptr + j * 3 + 1);
                        if (maxGrayDegreeG < *(ptr + j * 3 + 1))
                            maxGrayDegreeG = *(ptr + j * 3 + 1);
                        if (minGrayDegreeB > *(ptr + j * 3))
                            minGrayDegreeB = *(ptr + j * 3);
                        if (maxGrayDegreeB < *(ptr + j * 3))
                            maxGrayDegreeB = *(ptr + j * 3);
                    }
                }
                pR = 255.0 / (maxGrayDegreeR - minGrayDegreeR);
                pG = 255.0 / (maxGrayDegreeG - minGrayDegreeG);
                pB = 255.0 / (maxGrayDegreeB - minGrayDegreeB);
                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr1 = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        *(ptr1 + j * 3) = (byte)((*(ptr1 + j * 3) - minGrayDegreeB) * pB + 0.5);
                        *(ptr1 + j * 3 + 1) = (byte)((*(ptr1 + j * 3 + 1) - minGrayDegreeG) * pG + 0.5);
                        *(ptr1 + j * 3 + 2) = (byte)((*(ptr1 + j * 3 + 2) - minGrayDegreeR) * pR + 0.5);
                    }
                }
            }
            dstBmp.UnlockBits(bmpData); //解锁BitMap
            return true;
        }
        private bool Moravec(ref Bitmap curBitmap) //Moravec算子特征点提取
        {
            if (curBitmap == null)
            {
                return false;
            }
            int u;
            double[,] Templet;
            int[,] Co;
            

            double[,] Value1 = new double[curBitmap.Height, curBitmap.Width];   //定义一个以Img1图像长，宽的矩阵

            if (curBitmap != null)
            {
                Color curColor;         // ARGB 透明度，R G B 

                for (int i = 0; i < curBitmap.Height; i++)
                {
                    for (int j = 0; j < curBitmap.Width; j++)
                    {
                        curColor = curBitmap.GetPixel(j, i);
                        Value1[i, j] = curColor.R * 0.299 + curColor.G * 0.587 + curColor.B * 0.114;
                    }
                }
            }
            //toolStripProgressBar1.Value = 0;
            //toolStripProgressBar1.Maximum = 10;
            //toolStripProgressBar1.Value = 1;

            double[,] Value2 = new double[curBitmap.Height, curBitmap.Width];
            for (int i = 2; i < curBitmap.Height - 2; i++)
            {
                for (int j = 2; j < curBitmap.Width - 2; j++)
                {
                    double V1 = 0;
                    for (int m = 0; m < 4; m++)
                    {
                        V1 = V1 + Math.Pow(Value1[i - 2 + m, j] - Value1[i - 1 + m, j], 2);    //计算V1方向相邻像素灰度差平方和
                    }
                    double V2 = 0;
                    for (int m = 0; m < 4; m++)
                    {
                        V2 = V2 + Math.Pow(Value1[i - 2 + m, j - 2 + m] - Value1[i - 1 + m, j - 1 + m], 2);    //计算V2方向相邻像素灰度差平方和
                    }
                    double V3 = 0;
                    for (int m = 0; m < 4; m++)
                    {
                        V3 = V3 + Math.Pow(Value1[i, j - 2 + m] - Value1[i, j - 1 + m], 2);    //计算V3方向相邻像素灰度差平方和
                    }
                    double V4 = 0;
                    for (int m = 0; m < 4; m++)
                    {
                        V4 = V4 + Math.Pow(Value1[i - 2 + m, j + 2 - m] - Value1[i - 1 + m, j + 1 - m], 2);    //计算V4方向相邻像素灰度差平方和
                    }
                    Value2[i, j] = Math.Min(Math.Min(Math.Min(V1, V2), V3), V4);    //从V1、V2、V3、V4中取最小值作为该点兴趣值
                }
            }
            //toolStripProgressBar1.Value = 3;
            double sum = 0;
            for (int i = 0; i < curBitmap.Height - 2; i++)
            {
                for (int j = 0; j < curBitmap.Width - 2; j++)
                {
                    sum += Value2[i, j];
                }
            }

            int p = 0, Strength = 0;
            u = 110;
            double range;
            Co = new int[curBitmap.Height * curBitmap.Width / 9, 2];
            double[,] Value3 = new double[curBitmap.Height, curBitmap.Width];
            double avg1 = sum / ((curBitmap.Height - 2) * (curBitmap.Width - 2));
            range = avg1;
            while (u > 100 || u < 50)
            {

                if (u > 100)
                {
                    Strength++;
                    range = avg1 * Strength;
                }
                else
                {
                    range = range * ((2 * Strength - 1) / 2 * Strength);
                }
                for (int i = 0; i < curBitmap.Height; i++)
                    for (int j = 0; j < curBitmap.Width; j++)
                    {
                        if (i == 0 || i == 1 || i == curBitmap.Height - 2 || i == curBitmap.Height - 1 || j == 0 || j == 1 || j == curBitmap.Width - 2 || j == curBitmap.Width - 1)
                        {
                            Value3[i, j] = 0;
                            continue;
                        }
                        else
                        {
                            if (Value2[i, j] > range)
                            {
                                Value3[i, j] = 1;
                                p = p + 1;
                            }
                            else
                                Value3[i, j] = 0;
                        }


                    }
                u = 0;
                for (int i = 2; i < curBitmap.Height - 2; i = i + 5)
                    for (int j = 2; j < curBitmap.Width - 2; j = j + 5)
                    {
                        double MAX = 0;                          //假定5*5模板最大值起始值为第一个元素值
                        int a = 0;                                          //设a为最大值行
                        int b = 0;                                          //设b为最大值列
                        for (int s = i - 2; s < i + 3; s++)
                            for (int t = j - 2; t < j + 3; t++)
                            {
                                if (Value3[s, t] > 0)
                                {
                                    if (Value2[s, t] > MAX)
                                    {
                                        MAX = Value2[s, t];
                                        a = s;
                                        b = t;
                                    }
                                }
                            }
                        if (MAX > 0)
                        {
                            Co[u, 0] = a;
                            Co[u, 1] = b;
                            u = u + 1;
                        }
                    }
            }
            //toolStripProgressBar1.Value = 8;
            Templet = new double[u, 25];
            for (int i = 0; i < u; i++)
            {
                int t = 0;
                for (int m = 0; m < 5; m++)
                {
                    for (int n = 0; n < 5; n++)
                    {
                        Templet[i, t] = Value1[Co[i, 0] - 2 + m, Co[i, 1] - 2 + n];    //存储5*5模板灰度
                        t++;
                    }
                }
            }

            DataTable dt = new DataTable();
            //给datatable添加二个列
            dt.Columns.Add("列", typeof(String));
            dt.Columns.Add("行", typeof(String));

            Graphics g = Graphics.FromImage(curBitmap);                  //创建Graphics对象
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;     //设置高质量双三次插值法 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;                    //设置高质量,低速度呈现平滑程度
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;          //设置绘制到此 Graphics 的合成图像的呈现质量
            g.DrawImage(curBitmap, 0, 0, curBitmap.Width, curBitmap.Height);         //以img为原本重新于（0,0）点绘制
            g.Dispose();                                           //释放资源
            //toolStripProgressBar1.Value = 9;
            for (int i = 0; i < u; i++)
            {
                Graphics myGraphics = Graphics.FromImage(curBitmap);    //创建Graphics对象
                myGraphics.DrawLine(new Pen(Color.Red, 1), new Point(Co[i, 1] + 5, Co[i, 0]), new Point(Co[i, 1] - 5, Co[i, 0]));    //画出竖直方向直线
                myGraphics.DrawLine(new Pen(Color.Red, 1), new Point(Co[i, 1], Co[i, 0] + 5), new Point(Co[i, 1], Co[i, 0] - 5));    //画出水平方向直线
                myGraphics.Dispose();                                 //释放资源
                //pMapForm.pictureBox1.Image = curBitmap;                          //显示含有“+”的图

                DataRow dr = dt.NewRow();
                dr[0] = Co[i, 1];                        //将列数据赋给表
                dr[1] = Co[i, 0];                         //将行数据赋给表
                dt.Rows.Add(dr);
            }
            this.pMoraForm.dataGridView1.DataSource = dt;                 //将datatable绑定到datagridview上显示结果
            pMoraForm.dataGridView1.AllowUserToAddRows = false;
            pMoraForm.groupBox1.Text = "特征点像素坐标" + "(" + u.ToString() + "个" + ")";
            //toolStripProgressBar1.Value = 10;
            pMoraForm.Show();
            return true;
        }
        private void Gradient(ref Bitmap Slopebitmap)//坡度计算
        {
            //边缘信息还未做处理
            try
            {
                int[,] greydata = pMapForm.PixelValue;    //获取每个像素的DEM值
                double[,] Slopedata = new double[greydata.GetLength(0), greydata.GetLength(1)];
                double[] adfGeoTransform = pMapForm.AdfGeoTransform;
                //设置3*3的遍历窗口
                double max = -9999, min = 9999;
                for (int y = 1; y < Slopebitmap.Height - 1; y++)
                {
                    for (int x = 1; x < Slopebitmap.Width - 1; x++)
                    {
                        double Slope_we = (greydata[x - 1, y] - greydata[x + 1, y]) / (2 * adfGeoTransform[1]);
                        double Slope_sn = (greydata[x, y - 1] - greydata[x, y + 1]) / (2 * adfGeoTransform[5]);
                        double k = Slope_sn * Slope_sn + Slope_we * Slope_we;
                        Slopedata[x, y] = 100 * Math.Sqrt(k);    //计算结果用百分比表示
                        if (max < Slopedata[x, y])
                            max = Slopedata[x, y];
                        if (min > Slopedata[x, y])
                            min = Slopedata[x, y];
                    }
                }

                //拉伸展示
                for (int y = 1; y < Slopebitmap.Height - 1; y++)
                {
                    for (int x = 1; x < Slopebitmap.Width - 1; x++)
                    {
                        int Val = (int)(((Slopedata[x, y] - min) / (max - min)) * 255);
                        Color newcolor = Color.FromArgb(Val, Val, Val);
                        Slopebitmap.SetPixel(x, y, newcolor);
                    }
                }
            }

            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
        private void Aspect(ref Bitmap Aspectbitmap) //坡向计算
        {
            try
            {
                int[,] greydata = pMapForm.PixelValue;    //获取每个像素的DEM值
                double[,] Aspectdata = new double[greydata.GetLength(0), greydata.GetLength(1)];   //初始化坡向数组
                double[] adfGeoTransform = pMapForm.AdfGeoTransform;

                //设置3*3的遍历窗口
                double max = -9999, min = 9999;
                for (int y = 1; y < Aspectbitmap.Height - 1; y++)
                {
                    for (int x = 1; x < Aspectbitmap.Width - 1; x++)
                    {
                        double Slope_we = (greydata[x - 1, y] - greydata[x + 1, y]) / (2 * adfGeoTransform[1]);
                        double Slope_sn = (greydata[x, y - 1] - greydata[x, y + 1]) / (2 * adfGeoTransform[5]);

                        double Aspect = Math.Atan2(Slope_sn, -Slope_we) / (Math.PI / 180.0);

                        if (Slope_we == 0 && Slope_sn == 0)
                        {
                            Aspect = -1;
                        }
                        else    //计算的结果方位角表示
                        {
                            if (Aspect > 90.0)
                                Aspect = 450.0 - Aspect;
                            else
                                Aspect = 90.0 - Aspect;
                        }

                        if (Aspect == 360.0)
                            Aspect = 0.0;
                        Aspectdata[x, y] = Aspect;
                        if (max < Aspectdata[x, y])
                            max = Aspectdata[x, y];
                        if (min > Aspectdata[x, y])
                            min = Aspectdata[x, y];
                    }
                }
                //拉伸展示
                for (int y = 1; y < Aspectbitmap.Height - 1; y++)
                {
                    for (int x = 1; x < Aspectbitmap.Width - 1; x++)
                    {
                        int Val = (int)(((Aspectdata[x, y] - min) / (max - min)) * 255);
                        Color newcolor = Color.FromArgb(Val, Val, Val);
                        Aspectbitmap.SetPixel(x, y, newcolor);
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
        private static bool RadianceCalibration(Bitmap inBmp,out Bitmap outBmp)//光谱辐射定标
        {
            /* inBmp 原始图像
             * outBmp 处理后输出的图像
             * L=(Lmax-Lmin)/255 * DN+Lmin
             * Lsat1=Bias+(Gain * Dn)
             * Bias-偏移，Gain-增益，Dn-像元值
             * 其中，L为图像的辐亮度，单位为Wm-2sr-1 ；
             * Lmin为与最小灰度级对应的辐亮度；
             * Lmax为与最大灰度级对应的辐亮度；
             * 
             */
            if (inBmp == null)
            {
                outBmp = null;
                return false;
            }
            byte Lsat1B = 0, Lsat1G = 0, Lsat1R = 0;
            byte BiasB = 0, GainB = 0,   BiasG = 0, GainG = 0,   BiasR = 0, GainR = 0;
            byte LmaxB = 0, LminB = 255, LmaxG = 0, LminG = 255, LmaxR = 0, LminR = 255;

            double[] cs = new double[7];
            辐射定标Form fsdb = new 辐射定标Form();
            fsdb.ShowDialog();
            cs = fsdb.rcs();
            BiasB = (byte)cs[1]; GainB = (byte)cs[2];
            BiasG = (byte)cs[3]; GainG = (byte)cs[4];
            BiasR = (byte)cs[5]; GainR = (byte)cs[6];

            outBmp = new Bitmap(inBmp);
            Rectangle rt = new Rectangle(0, 0, outBmp.Width, outBmp.Height);
            //outBmp.lockbits将Bitmap锁定到系统内存中
            //BitmapData是BGR，Bitmap是RGB
            BitmapData bmpData = outBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            IntPtr ptr = bmpData.Scan0;//Get the address of the first line.获取首行地址 
            unsafe
            {
                #region//找出Lmax,Lmin

                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* DN = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        Lsat1B = (byte)(BiasB + (*(DN + j * 3) * GainB));
                        Lsat1G = (byte)(BiasG+ (*(DN + j * 3 + 1) * GainG));
                        Lsat1R = (byte)(BiasR + (*(DN + j * 3 + 2) * GainR));

                        if (Lsat1B >= LmaxB)
                            LmaxB = Lsat1B;
                        else if (Lsat1B < LminB)
                            LminB = Lsat1B;
                            
                        if (Lsat1G >= LmaxG)
                            LmaxG = Lsat1G;
                        else if (Lsat1G < LminG)
                            LminG = Lsat1G;

                        if (Lsat1R >= LmaxR)
                            LmaxR = Lsat1R;
                        else if (Lsat1R < LminR)
                            LminR = Lsat1R;
                    }
                }
                #endregion
                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* DN = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        *(DN + 3 * j) = (byte)(*(DN + 3 * j) * (LmaxB - LminB) / 255 + LminB);
                        *(DN + 3 * j + 1) = (byte)(*(DN + 3 * j + 1) * (LmaxG - LminG) / 255 + LminG);
                        *(DN + 3 * j + 2) = (byte)(*(DN + 3 * j + 2) * (LmaxR - LminR) / 255 + LminR);
                    }
                }

            }
            outBmp.UnlockBits(bmpData);//解锁outBmp;
            return true;
        }
        private void StretchGaussian(ref Bitmap currentBitMap) //高斯拉伸 
        {
            if (currentBitMap != null)
            {
                double mean = MeanOfBitmap(currentBitMap);
                double var = VarOfBitmap(currentBitMap, mean);

                if (mean <= 0 || var <= 0)
                    return;

                double min = mean - 3 * Math.Sqrt(var);
                double max = mean + 3 * Math.Sqrt(var);

                if (min <= 0)
                    min = 0;
                if (max > 255)
                    max = 255;

                Rectangle rec = new Rectangle(0, 0, currentBitMap.Width, currentBitMap.Height);
                BitmapData bmpData = currentBitMap.LockBits(rec, System.Drawing.Imaging.ImageLockMode.ReadWrite, currentBitMap.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int bytes = currentBitMap.Width * currentBitMap.Height * 3;   //Three bands total
                byte[] grayValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);

                double p = 255.0 / (max - min);
                double value = 0.0;
                for (int i = 0; i < bytes; i++)
                {
                    value = p * (grayValues[i] - min) + 0.5;
                    if (value > 0 && value < 255)
                    {
                        grayValues[i] = Convert.ToByte(value);
                    }
                    else if (value <= 0)
                    { grayValues[i] = 0; }
                    else
                    { grayValues[i] = 255; }
                }

                System.Runtime.InteropServices.Marshal.Copy(grayValues, 0, ptr, bytes);
                currentBitMap.UnlockBits(bmpData);
            }
        }
        private double VarOfBitmap(Bitmap currentBitMap, double meanvalue) //统计方差函数（RGB空间）
        {
            if (currentBitMap != null)
            {
                double V = 0;
                Rectangle rect = new Rectangle(0, 0, currentBitMap.Width, currentBitMap.Height);
                System.Drawing.Imaging.BitmapData bmpData = currentBitMap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* pIn = (byte*)bmpData.Scan0.ToPointer();
                    byte* P;
                    int R, G, B;
                    double conv = 0, sum = 0;
                    int stride = bmpData.Stride;
                    for (int y = 0; y < currentBitMap.Height; y++)
                    {
                        for (int x = 0; x < currentBitMap.Width; x++)
                        {
                            P = pIn;
                            B = P[0];
                            G = P[1];
                            R = P[2];
                            sum += (B * 0.114 + G * 0.587 + R * 0.299 - meanvalue) * (B * 0.114 + G * 0.587 + R * 0.299 - meanvalue);
                            pIn += 3;
                        }
                        pIn += stride - currentBitMap.Width * 3;
                    }
                    conv = sum / (currentBitMap.Width * currentBitMap.Height - 1);
                    V = conv;
                }
                currentBitMap.UnlockBits(bmpData);
                return V;  //返回图像方差V
            }
            return -1;
        }
        private double MeanOfBitmap(Bitmap currentBitMap)//统计均值（RGB空间）
        {
            if (currentBitMap != null)
            {
                double V = 0;
                Rectangle rect = new Rectangle(0, 0, currentBitMap.Width, currentBitMap.Height);
                System.Drawing.Imaging.BitmapData bmpData = currentBitMap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* pIn = (byte*)bmpData.Scan0.ToPointer();
                    byte* P;
                    int R, G, B;
                    double meanvalue = 0, sum = 0;
                    int stride = bmpData.Stride;
                    for (int y = 0; y < currentBitMap.Height; y++)
                    {
                        for (int x = 0; x < currentBitMap.Width; x++)
                        {
                            P = pIn;
                            B = P[0];
                            G = P[1];
                            R = P[2];
                            sum += B * 0.114 + G * 0.587 + R * 0.299;
                            pIn += 3;
                        }
                        pIn += stride - currentBitMap.Width * 3;
                    }
                    meanvalue = sum / (currentBitMap.Width * currentBitMap.Height);
                    V = meanvalue;
                }
                currentBitMap.UnlockBits(bmpData);
                return V;  //返回图像均值V
            }
            return -1;
        }
        private Bitmap GeometricRoughCorrection(Bitmap currentBitmap)//无人机影像粗校正
        {
            double[] cs = new double[11];
            影像粗校正Form 粗校正 = new 影像粗校正Form();

            粗校正.ShowDialog();
            cs = 粗校正.rcs();

            if (cs[0] == 0)
            {
                MessageBox.Show("几何校正已取消", "提示：", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
            //校正参数
            double Xs = cs[1], Ys = cs[2], Zs = cs[3], fai = cs[4] / 180 * 3.141592657, w0 = cs[5] / 180 * 3.141592657;
            double k = cs[6] / 180 * 3.141592657, Iw = cs[7] * 0.001, Ih = cs[8] * 0.001, f = cs[9] * 0.001, u = cs[10];
            //旋转矩阵参数
            double a1 = Math.Cos(fai) * Math.Cos(k) - Math.Sin(fai) * Math.Sin(w0) * Math.Sin(k);
            double a2 = -Math.Cos(fai) * Math.Sin(k) - Math.Sin(fai) * Math.Sin(w0) * Math.Cos(k);
            double a3 = -Math.Sin(fai) * Math.Cos(w0);
            double b1 = Math.Cos(w0) * Math.Sin(k);
            double b2 = Math.Cos(w0) * Math.Cos(k);
            double b3 = -Math.Sin(w0);
            double c1 = Math.Sin(fai) * Math.Cos(k) + Math.Cos(fai) * Math.Sin(w0) * Math.Sin(k);
            double c2 = -Math.Sin(fai) * Math.Sin(k) + Math.Cos(fai) * Math.Sin(w0) * Math.Sin(k);
            double c3 = Math.Cos(fai) * Math.Cos(k);
            //角点图像坐标
            double[,] Ixy = new double[5, 3];
            Ixy[1, 1] = -Iw / 2; Ixy[1, 2] = Ih / 2;
            Ixy[2, 1] = Iw / 2; Ixy[2, 2] = Ih / 2;
            Ixy[3, 1] = -Iw / 2; Ixy[3, 2] = -Ih / 2;
            Ixy[4, 1] = Iw / 2; Ixy[4, 2] = -Ih / 2;
            //图像像元大小
            double d = Iw / currentBitmap.Width;
            //角点地面坐标
            double[,] XY = new double[5, 3];
            double Zp = 750;//平均高程待定
            for (int i = 1; i < 5; i++)
            {
                XY[i, 1] = Xs + (Zp - Zs) * (a1 * Ixy[i, 1] + a2 * Ixy[i, 2] - a3 * f) / (c1 * Ixy[i, 1] + c2 * Ixy[i, 2] - c3 * f);
                XY[i, 2] = Ys + (Zp - Zs) * (b1 * Ixy[i, 1] + b2 * Ixy[i, 2] - b3 * f) / (c1 * Ixy[i, 1] + c2 * Ixy[i, 2] - c3 * f);

            }
            //输出影像最小范围
            double Xmin = (XY[1, 1] < XY[3, 1] ? XY[1, 1] : XY[3, 1]) - 2;
            double Xmax = (XY[2, 1] > XY[4, 1] ? XY[2, 1] : XY[4, 1]) + 2;
            double Ymin = (XY[3, 2] < XY[4, 2] ? XY[3, 2] : XY[4, 2]) + 2;
            double Ymax = (XY[1, 2] > XY[2, 2] ? XY[1, 2] : XY[2, 2]) + 6;
            int scw = (int)((Xmax - Xmin) / u);
            int sch = (int)((Ymax - Ymin) / u);

            //构建位图
            double Xp, Yp;
            double x, y;
            int i0, j0;
            Bitmap bitmap0 = new Bitmap(scw, sch, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Color curColor;
            for (int i = 0; i < scw; i++)
                for (int j = 0; j < sch; j++)
                {
                    Xp = i * u + Xmin;
                    Yp = (sch - j - 1) * u + Ymin;
                    x = -f * (a1 * (Xp - Xs) + b1 * (Yp - Ys) + c1 * (Zp - Zs)) / (a3 * (Xp - Xs) + b3 * (Yp - Ys) + c3 * (Zp - Zs));
                    y = -f * (a2 * (Xp - Xs) + b2 * (Yp - Ys) + c2 * (Zp - Zs)) / (a3 * (Xp - Xs) + b3 * (Yp - Ys) + c3 * (Zp - Zs));
                    i0 = (int)(x / d + currentBitmap.Width / 2.0); j0 = (int)(currentBitmap.Height / 2.0 - y / d);
                    if (i0 >= 0 && i0 < currentBitmap.Width && j0 >= 0 && j0 < currentBitmap.Height)
                    {
                        curColor = currentBitmap.GetPixel(i0, j0);
                        bitmap0.SetPixel(i, j, curColor);
                    }
                    else
                        bitmap0.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                }
            return bitmap0;
        }
        private void ShowMap(bool show)    //显示Map form函数
        {
            bMapformVisible = show;
            地图视图ToolStripMenuItem.Checked = show;
            if (show)
            {
                pMapForm.Show(this.dockPanel1);

            }
            else
            {
                pMapForm.Hide();
            }
        }
        private void ShowHist(bool show)//显示histogram窗口函数
        {
            bHistformVisible = show;
            直方图显示ToolStripMenuItem.Checked = show;
            if (show)
            {
                pHistForm.Show(this.dockPanel1);
            }
            else
            {
                pHistForm.Hide();
            }
        }
        private void HistogramEqualize(ref Bitmap HistogramEqualizationBitmap)//直方图均衡化
        {
            int[][] Pixel;
            Pixel = pMapForm.Src_PixelData;  //获取显示到屏幕上的通道灰度值
            if (Pixel.Length == 3)
            {
                int[][] Count = new int[3][];//0:R  1:G  2:B
                Count[0] = new int[256];
                Count[1] = new int[256];
                Count[2] = new int[256];
                double[][] LETU_RGB = new double[3][];//0:R  1:G  2:B
                LETU_RGB[0] = new double[256];
                LETU_RGB[1] = new double[256];
                LETU_RGB[2] = new double[256];
                //统计各个波段对应的像素值的个数
                for (int i = 0; i < Pixel[0].Length; i++)
                {
                    Count[0][Pixel[0][i]] += 1;
                    Count[1][Pixel[1][i]] += 1;
                    Count[2][Pixel[2][i]] += 1;
                }
                //累计归一化
                for (int i = 0; i <= 255; i++)
                {
                    LETU_RGB[0][i] = (double)Count[0][i] / Pixel[0].Length;
                    LETU_RGB[1][i] = (double)Count[1][i] / Pixel[1].Length;
                    LETU_RGB[2][i] = (double)Count[2][i] / Pixel[2].Length;
                    if (i >= 1)
                    {
                        LETU_RGB[0][i] += LETU_RGB[0][i - 1];
                        LETU_RGB[1][i] += LETU_RGB[1][i - 1];
                        LETU_RGB[2][i] += LETU_RGB[2][i - 1];
                    }

                }
                //setPxiel
                for (int i = 0; i < HistogramEqualizationBitmap.Width; i++)
                {
                    for (int j = 0; j < HistogramEqualizationBitmap.Height; j++)
                    {
                        int rVal = (int)(LETU_RGB[0][Pixel[0][i + j * HistogramEqualizationBitmap.Width]] * 255);
                        int gVal = (int)(LETU_RGB[1][Pixel[1][i + j * HistogramEqualizationBitmap.Width]] * 255);
                        int bVal = (int)(LETU_RGB[2][Pixel[2][i + j * HistogramEqualizationBitmap.Width]] * 255);
                        Color newcolor = Color.FromArgb(rVal, gVal, bVal);
                        HistogramEqualizationBitmap.SetPixel(i, j, newcolor);
                    }
                }
            }
            else
            {
                int[][] Count = new int[1][];
                Count[0] = new int[256];
                double[][] LETU_RGB = new double[1][];
                LETU_RGB[0] = new double[256];
                //统计各个波段对应的像素值的个数
                for (int i = 0; i < Pixel[0].Length; i++)
                {
                    Count[0][Pixel[0][i]] += 1;
                }
                //累计归一化
                for (int i = 0; i <= 255; i++)
                {
                    LETU_RGB[0][i] = (double)Count[0][i] / Pixel[0].Length;
                    if (i >= 1)
                    {
                        LETU_RGB[0][i] += LETU_RGB[0][i - 1]; //相关的累加
                    }
                }
                //setPxiel
                for (int i = 0; i < HistogramEqualizationBitmap.Width; i++)
                {
                    for (int j = 0; j < HistogramEqualizationBitmap.Height; j++)
                    {
                        int rVal = (int)LETU_RGB[0][Pixel[0][i * HistogramEqualizationBitmap.Width + j]] * 255;
                        Color newcolor = Color.FromArgb(rVal, rVal, rVal);
                        HistogramEqualizationBitmap.SetPixel(i, j, newcolor);
                    }
                }
            }
        }
        private void LinearStretchA(ref Bitmap currentBitMap) //根据最大、最小值进行线性增强
        {
            if (currentBitMap != null)
            {
                AForge.Imaging.Filters.ContrastStretch filter = new ContrastStretch();
                filter.ApplyInPlace(currentBitMap);

            }
        }
        private void  LinearStretchB(ref Bitmap currentBitMap)//根据均值和方差进行线性增强
        {
            if (currentBitMap != null)
            {
                double mean = MeanOfBitmap(currentBitMap);
                double var = VarOfBitmap(currentBitMap, mean);

                if (mean <= 0 || var <= 0)
                    return;

                double min = mean - 3 * Math.Sqrt(var);
                double max = mean + 3 * Math.Sqrt(var);

                if (min <= 0 || max <= 0)
                    return;

                Rectangle rec = new Rectangle(0, 0, currentBitMap.Width, currentBitMap.Height);
                BitmapData bmpData = currentBitMap.LockBits(rec, System.Drawing.Imaging.ImageLockMode.ReadWrite, currentBitMap.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int bytes = currentBitMap.Width * currentBitMap.Height * 3;   //Three bands total
                byte[] grayValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);

                double p = 255.0 / (max - min);
                double value = 0.0;
                for (int i = 0; i < bytes; i++)
                {
                    value = p * (grayValues[i] - min) + 0.5;
                    if (value > 0 && value < 255)
                    {
                        grayValues[i] = Convert.ToByte(value);
                    }
                    else if (value <= 0)
                    { grayValues[i] = 0; }
                    else
                    { grayValues[i] = 255; }
                }

                System.Runtime.InteropServices.Marshal.Copy(grayValues, 0, ptr, bytes);
                currentBitMap.UnlockBits(bmpData);
            }
        }
        public void ZoomIn(object sender, MouseEventArgs e)  //放大
        {
            double zoomin = pMapForm.zoom;
            if (zoomin >= 8)
                return;
            zoomin *= 2;
            pMapForm.zoom = zoomin;
        }
        public void ZoomOut(object sender, MouseEventArgs e) //缩小
        {
            double zoomout = pMapForm.zoom;
            if (zoomout <= 0.125)
                return;
            zoomout /= 2;
            pMapForm.zoom = zoomout;
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
        private void K_means(ref Bitmap curBitmap)//非监督分类K-means 
        {
            //Bitmap curBitmap = bitmap;
            if (curBitmap != null)
            {
                分割聚类数Form number = new 分割聚类数Form();
                if (number.ShowDialog() == DialogResult.OK)
                {
                    int numbers = number.GetNumber;//聚类类别数
                    int K = number.GetNumber;//迭代次数
                    
                    //初始聚类中心
                    double[,] csjlzx = new double[numbers, 5];//总个数,R,G,B,band4z;
                    for (int i = 0; i < numbers; i++)
                    {
                        csjlzx[i, 1] = maxandmin1[0] + (i + 0.5) * (maxandmin1[1] - maxandmin1[0]) / numbers;
                        csjlzx[i, 2] = maxandmin2[0] + (i + 0.5) * (maxandmin2[1] - maxandmin2[0]) / numbers;
                        csjlzx[i, 3] = maxandmin3[0] + (i + 0.5) * (maxandmin3[1] - maxandmin3[0]) / numbers;
                        csjlzx[i, 4] = maxandmin4[0] + (i + 0.5) * (maxandmin4[1] - maxandmin4[0]) / numbers;///
                    }
                    
                    int[] result = new int[w * h];//像元所属类别
                    double[,] s = new double[numbers, 5];
                    int ner;
                    for (int i = 0; i < K; i++)//迭代次数K次
                    {
                        for (int j = 0; j < w * h; j++)//将w*h个像元分到numbers个中心
                        {
                            ner = 0; double d0 = 1000000000;
                            for (int k = 0; k < numbers; k++)//找最邻近中心
                            {
                                double d = Math.Sqrt(Math.Pow(csjlzx[k, 1] - r[j], 2) 
                                    + Math.Pow(csjlzx[k, 2] - g[j], 2) 
                                    + Math.Pow(csjlzx[k, 3] - b[j], 2) 
                                    + Math.Pow(csjlzx[k, 4] - band4z[j], 2));
                                if (d < d0)
                                {
                                    d0 = d;
                                    ner = k;
                                }
                            }
                            s[ner, 1] = s[ner, 1] + r[j];
                            s[ner, 2] = s[ner, 2] + g[j];
                            s[ner, 3] = s[ner, 3] + b[j];
                            s[ner, 4] = s[ner, 4] + band4z[j];
                            s[ner, 0]++;

                            result[j] = ner;

                        }
                        //更新聚类中心
                        for (int n = 0; n < numbers; n++)
                        {
                            csjlzx[n, 1] = s[n, 1] / s[n, 0];
                            csjlzx[n, 2] = s[n, 2] / s[n, 0];
                            csjlzx[n, 3] = s[n, 3] / s[n, 0];
                            csjlzx[n, 4] = s[n, 4] / s[n, 0];///
                        }

                        //将聚类中心像元数及各灰度总和置零，进行下次循环
                        for (int k1 = 0; k1 < numbers; k1++)
                        {
                            s[k1, 0] = 0;
                            s[k1, 1] = 0;
                            s[k1, 2] = 0;
                            s[k1, 3] = 0;
                            s[k1, 4] = 0;
                        }
                    }

                    InitializeColor();//初始化颜色表
                    for (int i = 0; i < w; i++)
                    {
                        for (int j = 0; j < h; j++)
                        {
                            int Class = result[i + j * w];
                            int[] gary = new int[3];
                            gary = GetGary(Class);
                            Color newColor;
                            newColor = Color.FromArgb(gary[0], gary[1], gary[2]);
                            curBitmap.SetPixel(i, j, newColor);
                        }
                    }
                    pMapForm.ShowPic.Image = curBitmap;
                    //pictureBox1.Image = curBitmap;
                    pMapForm.treeView1.Nodes.Clear();
                    for (int k = 0; k < numbers; k++)
                    {
                        pMapForm.treeView1.Nodes.Add("类别" + (k + 1).ToString());
                        pMapForm.treeView1.Nodes[2 * k].BackColor = Color.White;

                        pMapForm.treeView1.Nodes.Add("  ");
                        int[] gary = new int[3];
                        gary = GetGary(k);
                        Color newColor;
                        newColor = Color.FromArgb(gary[0], gary[1], gary[2]);
                        pMapForm.treeView1.Nodes[2 * k + 1].BackColor = newColor;
                    }
                }
            }
        }
        private void IterativeThreshold(ref Bitmap inBmp)//迭代法阈值分割
        {   
            if (inBmp != null)
            {
                int[,] in_image = new int[w, h];//建一个跟inBmp一样大小的矩阵
                Bitmap bm = inBmp;
                int i, j, Th;
                long[] p = new long[256];
                long[] num = new long[256];
                //转为灰度图像in_image[,]            
                for (i = 0; i < w; i++)
                    for (j = 0; j < h; j++)
                        in_image[i, j] = (bm.GetPixel(i, j)).R;  //in_image矩阵存储bm R波段的像素值

                //初始化
                for (i = 0; i < 256; i++)
                    p[i] = 0;

                int Thresh,
                    NewThresh,
                    GrayMax, GrayMin;         //最大,最小灰度值

                //统计各灰度级出现的次数
                GrayMax = 0;
                GrayMin = 255;
                for (j = 0; j < h; j++)
                {
                    for (i = 0; i < w; i++)
                    {
                        int g = in_image[i, j];
                        p[g]++; //p[g]=p[g]+1;    p[g]统计in_image[]各像素值的个数
                        if (g > GrayMax) GrayMax = g;
                        if (g < GrayMin) GrayMin = g;   //找出像素最小最大值
                    }
                }
                p[0] = 0;
                Thresh = 0;
                NewThresh = (GrayMax + GrayMin) / 2;  //像素最大值最小值的平均值为初始阈值
                int MeanGray1, MeanGray2;
                long p1,
                     p2,
                     s1,
                     s2;
                //迭代
                for (i = 0; (Thresh != NewThresh) && (i < 100); i++)
                {
                    Thresh = NewThresh;
                    p1 = 0; p2 = 0; s1 = 0; s2 = 0; 

                    //求两个区域的灰度平均值
                    for (j = GrayMin; j < Thresh; j++)
                    {
                        p1 += p[j] * j;//p1=p1+p[j]*j  像素值总和
                        s1 += p[j];    //s1=s1+p[j]    总的像素点数     
                    }
                    MeanGray1 = (int)(p1 / s1);   //区域一的平均像素值
                    for (j = Thresh + 1; j < GrayMax; j++)
                    {
                        p2 += p[j] * j;
                        s2 += p[j];
                    }
                    MeanGray2 = (int)(p2 / s2);
                    NewThresh = (MeanGray1 + MeanGray2) / 2;
                }
                Th = NewThresh;       //最佳阈值
                inBmp = threshSeg(in_image, w, h, Th);
            }
        }
        private void MinDistance(ref Bitmap bitmap)//监督分类(最小距离)
        {

        }
        private void Sobel(ref Bitmap bitmap)//边缘检测Sobel算子
        {
            if (pMapForm.pictureBox1.Image != null)
            {
                int w = pMapForm.bufWidth,
                    h = pMapForm.bufHeight;
                long[,] so = new long[w, h];
                int[,] sobel = new int[w, h];
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        so[i, j] = (long)(bitmap.GetPixel(i, j).R * 0.299 
                            + bitmap.GetPixel(i, j).G * 0.587 
                            + bitmap.GetPixel(i, j).B * 0.114);
                    }
                }
                long GX, GY;
                for (int i = 2; i < w - 1; i++)
                {
                    for (int j = 2; j < h - 1; j++)
                    {
                        GX = (so[i + 1, j - 1] + 2 * so[i + 1, j] + so[i + 1, j + 1]) - (so[i - 1, j - 1] + 2 * so[i - 1, j] + so[i - 1, j + 1]);
                        GY = (so[i - 1, j - 1] + 2 * so[i, j - 1] + so[i + 1, j - 1]) - (so[i - 1, j + 1] + 2 * so[i, j + 1] + so[i + 1, j + 1]);
                        sobel[i, j] = (int)Math.Sqrt(GX * GX + GY * GY);
                    }
                }
                Color c;
                int th = GetThresh(bitmap);
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        if (sobel[i, j] > th)
                        {
                            c = Color.FromArgb(255, 255, 255);
                            bitmap.SetPixel(i, j, c);
                        }
                        else
                        {
                            c = Color.FromArgb(0, 0, 0);
                            bitmap.SetPixel(i, j, c);
                        }
                    }
                }
            }
            else
                MessageBox.Show("请打开图像");
        }
        private void Prewitt(ref Bitmap bitmap)//边缘检测Prewitt算子
        {
            if (pMapForm.pictureBox1.Image != null)
            {
                int w = pMapForm.bufWidth, h = pMapForm.bufHeight;
                long[,] so = new long[w, h];
                int[,] sobel = new int[w, h];
                //Bitmap bitmap = (Bitmap)pMapForm.pictureBox1.Image;
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        so[i, j] = (long)(bitmap.GetPixel(i, j).R * 0.299 
                            + bitmap.GetPixel(i, j).G * 0.587 
                            + bitmap.GetPixel(i, j).B * 0.114);
                    }
                }//灰度化

                for (int i = 2; i < w - 1; i++)
                {
                    for (int j = 2; j < h - 1; j++)
                    {
                        sobel[i, j] = (int)(Math.Abs(so[i - 1, j + 1] - so[i + 1, j + 1] + so[i - 1, j] - so[i + 1, j] + so[i - 1, j - 1] - so[i + 1, j - 1]) + Math.Abs(so[i - 1, j + 1] - so[i - 1, j - 1] + so[i, j + 1] - so[i, j - 1] + so[i + 1, j + 1] - so[i + 1, j - 1]));
                    }
                }
                Color c;
                int th = GetThresh(bitmap);
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        if (sobel[i, j] > th)
                        {
                            c = Color.FromArgb(255, 255, 255);
                            bitmap.SetPixel(i, j, c);
                        }
                        else
                        {
                            c = Color.FromArgb(0, 0, 0);
                            bitmap.SetPixel(i, j, c);
                        }
                    }
                }
            }
            else
                MessageBox.Show("请打开图像");
        }
        private int GetThresh(Bitmap bitmap)//获得阈值
        {
            int i, j, g, gmin = 255, gmax = 0;
            int w = pMapForm.bufWidth, h = pMapForm.bufHeight;
            int[,] gimage = new int[w, h];  //存储bitmap R波段灰度值
            long[] gr = new long[256];
            for (i = 0; i < w; i++)
            {
                for (j = 0; j < h; j++)
                {
                    gimage[i, j] = bitmap.GetPixel(i, j).R;
                }
            }
            for (i = 0; i < 256; i++)
                gr[i] = 0;
            for (j = 0; j < h; j++)
            {
                for (i = 0; i < w; i++)
                {
                    g = gimage[i, j];
                    gr[g]++;
                    if (g > gmax) gmax = g;
                    if (g < gmin) gmin = g;
                }
            }    //找出灰度最大最小值
            int newthresh = (gmax + gmin) / 2;
            int thresh = 0;
            for (i = 0; (thresh != newthresh) && (i < 100); i++)
            {
                thresh = newthresh;
                long p1 = 0, p2 = 0, s1 = 0, s2 = 0;
                for (j = gmin; j < thresh; j++)
                {
                    p1 += gr[j] * j;
                    s1 += gr[j];
                }
                int meangray1 = (int)(p1 / s1);
                for (j = thresh + 1; j < gmax; j++)
                {
                    p2 += gr[j] * j;
                    s2 += gr[j];
                }
                int meangray2 = (int)(p2 / s2);
                newthresh = (meangray1 + meangray2) / 2;
            }
            return newthresh;
        }
        private Bitmap threshSeg(int[,] in_image, int w0, int h0, int t)
        {
            Bitmap bm = new Bitmap(w0, h0);
            Color c;
            for (int j = 0; j < h0; j++)
            {
                for (int i = 0; i < w0; i++)
                {
                    if (in_image[i, j] > t)
                        c = Color.FromArgb(255, 255, 255);//设置RGB值
                    else
                        c = Color.FromArgb(0, 0, 0);
                    bm.SetPixel(i, j, c);
                }
            }
            return bm;
        }
        private void SetupShowHistEvents(MapForm pMap)
        {
            pMap.ShowHist += new MapForm.ShowHistEventHandler(this.map_ImageHist);
        }
        private int[] GetGary(int a)//获取灰度值
        {
            int[] gray = new int[3];
            gray[0] = color[a, 0]; 
            gray[1] = color[a, 1]; 
            gray[2] = color[a, 2];
            return gray;
        }
        private void InitializeColor()//初始化颜色表
        {
            color[0, 0] = 0;   color[0, 1] = 0;   color[0, 2] = 0;//Black
            color[1, 0] = 192; color[1, 1] = 192; color[1, 2] = 192;//Red
            color[2, 0] = 0;   color[2, 1] = 0;   color[2, 2] = 255;//
            color[3, 0] = 0;   color[3, 1] = 255; color[3, 2] = 0;//
            color[4, 0] = 0;   color[4, 1] = 255; color[4, 2] = 255;//
            color[5, 0] = 255; color[5, 1] = 255; color[5, 2] = 0;//
            color[6, 0] = 64;  color[6, 1] = 0;   color[6, 2] = 128;//
            color[7, 0] = 255; color[7, 1] = 255; color[7, 2] = 255;//
            color[8, 0] = 128; color[8, 1] = 0;   color[8, 2] = 0;//
            color[9, 0] = 123; color[9, 1] = 104; color[9, 2] = 238;//

        }
        private void map_ImageHist(object sender, MapForm.ShowHistEventArgs e)
        {
            if (File.Exists(e.Filename) != true)
                return;

            pHistForm.Filename = e.Filename;
            Gdal.AllRegister();
            Dataset dataset = Gdal.Open(e.Filename, OSGeo.GDAL.Access.GA_ReadOnly);

            if (dataset != null)
            {
                pHistForm.channelCombo.Items.Clear();
                for (int i = 0; i < dataset.RasterCount; i++)
                {
                    string name = "band_" + (i + 1).ToString();
                    pHistForm.channelCombo.Items.Add(name);
                }
            }

            ShowHist(true);
        } 
        public void GetcurBitmap(int index)
        {
            //OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap srcBmp = pMapForm.GetImage(dataset);
            Bitmap curBitmap;//
            switch (index)
            {
                case 0:
                    pMapForm.currentBitmap = srcBmp;
                    break ;
                case 1:
                    HistogramGrayStretch(srcBmp, out curBitmap); // 全等级灰度拉伸
                    pMapForm.currentBitmap = curBitmap;
                    break;
                case 2:
                    HistogramEqualize(ref srcBmp);     //直方图均衡化
                    pMapForm.currentBitmap = srcBmp;
                    break;
                case 3:
                    Moravec(ref srcBmp);
                    pMapForm.currentBitmap = srcBmp; //提取图像特征点
                    break;
                case 4:
                    Gradient(ref srcBmp);            //坡度计算
                    pMapForm.currentBitmap = srcBmp; 
                    break;
                case 5:
                    Aspect(ref srcBmp);  //坡向计算
                    pMapForm.currentBitmap = srcBmp;
                    break;
                case 6:
                    LinearStretchA(ref srcBmp);   //最大最小值增强
                    pMapForm.currentBitmap = srcBmp;
                    break;
                case 7:
                    LinearStretchB(ref srcBmp);//标准方差增强
                    pMapForm.currentBitmap = srcBmp;
                    break;
                case 8:
                    RadianceCalibration(srcBmp, out curBitmap);//光谱辐射定标
                    pMapForm.currentBitmap = curBitmap;
                    break;
                case 9:
                    StretchGaussian(ref srcBmp);//高斯处理
                    pMapForm.currentBitmap = srcBmp;
                    break;
                case 10:
                    curBitmap = GeometricRoughCorrection(srcBmp);//无人机影像校正
                    pMapForm.currentBitmap = curBitmap;
                    break;
                case 11:
                    K_means(ref srcBmp);//非监督分类Kmeans
                    pMapForm.currentBitmap = srcBmp;
                    break;
                case 12:
                    IterativeThreshold(ref srcBmp);   //迭代法阈值分割
                    pMapForm.currentBitmap = srcBmp;
                    break;
                case 13:
                    MinDistance(ref srcBmp);       //监督分类（最小距离法）
                    pMapForm.currentBitmap = srcBmp;
                    break;
                case 14:
                    Sobel(ref srcBmp);    //边缘检测 Sobel算子
                    pMapForm.currentBitmap = srcBmp;
                    break;
                case 15:
                    Prewitt(ref srcBmp);    //边缘检测 Prewitt算子
                    pMapForm.currentBitmap = srcBmp;
                    break;
                default:
                    
                    break ;
            }
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
        }

        #endregion

        #region//事件
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            //OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "remote sensing image|*.tif;*.img;*.hdf;|regular digital image|*.jpg;*.png;*.bmp*";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            TreeNode pNode = CommonClass.u_GetNodebyName(pMapForm.treeView1.Nodes, strImageList);
            pNode.Nodes.Add(ofd.FileName);
            pNode = CommonClass.u_GetNodebyName(pNode.Nodes, ofd.FileName);

            
            dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            if (dataset != null)
            {
                // string[] sss = dataset.GetMetadata("SUBDATASETS");

                for (int i = 0; i < dataset.RasterCount; i++)
                {
                    string name = "band_" + (i + 1).ToString();
                    pNode.Nodes.Add(name);
                }
            }
            int[] bandlist = { 1, 2, 3 };
            pMapForm.listImages.Add(bandlist); //初始化List下的通道数组
            pMapForm.ifGetRGB.Add(false);     //还未进行波段数组的更新，false
            if (dataset.RasterCount >= 3)
            {
                int[][] RGB_band = new int[3][];
                pMapForm.Src_PixelData = RGB_band; //初始化List下的灰度值数组，多波段
                Band[] bands = new Band[3];
                pMapForm.bands.Add(bands);  //初始化List下的Band型数组，多波段
            }
            else
            {
                int[][] RGB_band = new int[1][];
                pMapForm.Src_PixelData = RGB_band; //初始化List下的灰度值数组,单波段
                Band[] bands = new Band[1];
                pMapForm.bands.Add(bands);  //初始化List下的Band型数组，单波段
            }
            ShowMap(true);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            OSGeo.GDAL.Gdal.AllRegister();           //第一行是注册所有的格式驱动，
            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");   //第二行是支持中文路径和名称

            //this.toolStripStatusLabel2.Text = "当前时间: " + DateTime.Now.ToString();
            SetupShowHistEvents(pMapForm);
        }
        private void ZoomInButton_Click(object sender, EventArgs e) //放大
        {
            double zoomin = pMapForm.zoom; //get
            if (zoomin >= 8)
                return;
            zoomin *= 1.2;
            pMapForm.zoom = zoomin;  //set  //将mainform对Zoom的改变传递给mapform
            GetcurBitmap(pMapForm.index);
        }
        private void ZoomOutButton_Click(object sender, EventArgs e) //缩小
        {
            double zoomout = pMapForm.zoom;
            if (zoomout <= 0.125)
                return;
            zoomout /= 1.2;
            pMapForm.zoom = zoomout;//将mainform对Zoom的改变传递给mapform
            GetcurBitmap(pMapForm.index);
        }
        private void PanButton_Click(object sender, EventArgs e)//移动
        {
            pMapForm.MapCursor = Cursors.Hand;
        }
        private void ElementButton_Click(object sender, EventArgs e)//取消移动手
        {
            pMapForm.MapCursor = Cursors.Default;
        }
        private void 线性拉伸ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap srcBmp = pMapForm.GetImage(dataset);
            Bitmap dstBmp = pMapForm.Picbitmap;
            bool b= HistogramGrayStretch(srcBmp,out dstBmp);
            if (b)  //如果全等级灰度拉伸处理成功
            {
                pMapForm.currentBitmap = dstBmp;
                pMapForm.ShowPic.Image = pMapForm.currentBitmap;
                MessageBox.Show("全等级灰度拉伸处理成功");
                pMapForm.index = 1;
            }
            else 
            {
                MessageBox.Show("全等级灰度拉伸处理失败");
            }
        }
        private void 直方图均衡化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap srcBmp = pMapForm.GetImage(dataset);
            HistogramEqualize(ref srcBmp);
            pMapForm.index = 2;

            pMapForm.currentBitmap = srcBmp;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
        }
        private void 提取图像特征点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Bitmap curBitmap = pMapForm.Picbitmap;
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap srcBmp = pMapForm.GetImage(dataset);
            bool b = Moravec(ref srcBmp);
            if (b)
            {
                pMapForm.currentBitmap = srcBmp;
                pMapForm.ShowPic.Image = pMapForm.currentBitmap;
                MessageBox.Show("提取图像特征点处理成功");
                pMapForm.index = 3;
            }
            else
            {
                MessageBox.Show("提取图像特征点处理失败");
            }
        }
        private void 坡度计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap Slopebitmap = pMapForm.GetImage(dataset);

            Gradient(ref Slopebitmap);
            pMapForm.index = 4;
            pMapForm.currentBitmap = Slopebitmap;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
        }
        private void 坡向计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap Aspectbitmap = pMapForm.GetImage(dataset);    //定义坡向的位图
            pMapForm.index = 5;
            Aspect(ref Aspectbitmap);
            pMapForm.currentBitmap = Aspectbitmap;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
        }
        private void 最大最小值增强ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap srcBmp = pMapForm.GetImage(dataset);
            LinearStretchA(ref srcBmp);   //最大最小值增强 输入srcBmp,输出srcBmp
            pMapForm.currentBitmap = srcBmp;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
            MessageBox.Show("最大最小值增强成功");
            pMapForm.index = 6;
        }
        private void 标准方差增强ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap srcBmp = pMapForm.GetImage(dataset);
            LinearStretchB(ref srcBmp);//标准方差增强 输入srcBmp,输出srcBmp

            pMapForm.currentBitmap = srcBmp;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
            MessageBox.Show("标准方差增强成功");
            pMapForm.index = 7;
        }
        private void 辐射定标ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap srcBmp = pMapForm.GetImage(dataset);
            Bitmap dstBmp = pMapForm.Picbitmap;
            bool b = RadianceCalibration(srcBmp, out dstBmp);
            if (b)  //如果光谱辐射定标成功
            {
                pMapForm.currentBitmap = dstBmp;
                pMapForm.ShowPic.Image = pMapForm.currentBitmap;
                MessageBox.Show("光谱辐射定标成功");
                pMapForm.index = 8;
            }
            else
            {
                MessageBox.Show("光谱辐射定标失败");
            }
        }
        private void 高斯处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap srcBmp = pMapForm.GetImage(dataset);
            StretchGaussian(ref srcBmp);

            pMapForm.currentBitmap = srcBmp;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
            MessageBox.Show("高斯处理成功");
            pMapForm.index = 9;
        }
        private void 影像融合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            图像融合Form1 图像融合 = new 图像融合Form1();
            图像融合.ShowDialog(); 
        }
        private void 无人机影像几何粗校正ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap srcBmp = pMapForm.GetImage(dataset);

            pMapForm.currentBitmap = GeometricRoughCorrection(srcBmp);
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
            pMapForm.index = 10;
        }
        private void 地理配准ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            地理配准Form 地理配准 = new 地理配准Form();
            地理配准.ShowDialog();
        }
        private void 非监督分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            int[] disband = { 1,2,3 };
            Bitmap srcBmp = GetImage(dataset,disband);//
            K_means(ref srcBmp);

            pMapForm.currentBitmap = srcBmp;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
            pMapForm.index = 11;
        }
        private void 迭代阈值法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            int[] disband = { 1, 2, 3 };
            Bitmap srcBmp = GetImage(dataset,disband);
            IterativeThreshold(ref srcBmp);

            pMapForm.currentBitmap = srcBmp;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
            pMapForm.index = 12;
        }
        private void 监督分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            监督分类Form SupervisedClassification = new 监督分类Form();
            SupervisedClassification.GetpMapBitmap(pMapForm.currentBitmap);
            SupervisedClassification.GetDataSet(dataset);
            SupervisedClassification.Show();
            pMapForm.currentBitmap=SupervisedClassification.getbitmap;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
            pMapForm.index = 13;
            /*
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            int[] disband = { 1, 2, 3 };
            Bitmap srcBmp = GetImage(dataset, disband);
            MinDistance(ref srcBmp);

            pMapForm.currentBitmap = srcBmp;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
            index = 13;
            */
        }
        private void sobel算子ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap srcBmp = pMapForm.GetImage(dataset);
            Sobel(ref srcBmp);
            pMapForm.currentBitmap = srcBmp;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
            pMapForm.index = 14;
        }
        private void prewitt算子ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(ofd.FileName, OSGeo.GDAL.Access.GA_ReadOnly);
            Bitmap srcBmp = pMapForm.GetImage(dataset);
            Prewitt(ref srcBmp);
            pMapForm.currentBitmap = srcBmp;
            pMapForm.ShowPic.Image = pMapForm.currentBitmap;
            pMapForm.index = 15;
        }
        private void 遥感影像变化检测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            遥感影像变化检测Form 变化检测 = new 遥感影像变化检测Form();
            变化检测.Show();
        }    

    }
        #endregion
}

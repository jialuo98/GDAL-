using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using OSGeo.GDAL;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;


namespace 遥感图像处理作业_1
{
    public partial class MapForm : DockContent
    {
        #region 变量定义

        string strImageList = "ImageList";
        string strImagePath = null;
        
        OSGeo.GDAL.Dataset dataset;//初始读取的遥感影像

        int BandIndex = 1;   //从1开始
        string Bandstring = null;
        private int[,] GreyData;  //Moravec算子需要使用的数组，针对灰度图像
                                  //计算坡度坡向要用到的数组  DEM数据
        internal List<int[]> listImages = new List<int[]>();
        /*
         * 显示出来的对应影像的RGB通道的像素值的集合（屏幕上的图像，并非原图)
         */ 
        private List<int[][]> SrcRGB_band = new List<int[][]>();
        private int imageindex;       //操作treenode控件上的对应的图像的索引值
        internal List<bool> ifGetRGB = new List<bool>();  //判断是否存储了影像的RGB到数组
        internal List<Band[]> bands = new List<Band[]>();  // Band对象的集合
        private double[] adfGeoTransform;         //获取影像的地理坐标信息
        int TreeNodeX, TreeNodeY;
        Bitmap bitmap;

        public Bitmap currentBitmap;//当前Bitmap
        public int index; //记录dataset进行了哪些操作
        光谱特征Form 光谱特征 = new 光谱特征Form();
        bool SpectralSignature = false;//判断是否点了光谱特征按钮

        #region GetBitmap
        public int bufWidth, bufHeight; //显示图像按钮点击后产生的bitmap的width，height
        double[] maxandmin1 = { 0, 0 };
        double[] maxandmin2 = { 0, 0 };
        double[] maxandmin3 = { 0, 0 };
        double[] maxandmin4 = { 0, 0 };
        int[] r;
        int[] g;
        int[] b;
        int[] band4z;
        int[] disband;
        int w,h;
        double Zoom = 1; //
        #endregion

        #region MouseEventArgs
        private Cursor mapCursor = Cursors.Default;

        Point offset = new Point(0, 0);
        #endregion

        #region 影像移动
        private int originX, originY; //picturebox上展示出来的图像的左上角在原始图像中的坐标
        private int bufX = 0, bufY = 0;
        #endregion

        public delegate void ShowHistEventHandler(object sender, ShowHistEventArgs e); //定义委托
        public event ShowHistEventHandler ShowHist;                           //定义事件
        //public event ShowHistEventHandler ShowSpectralSignature; //定义打开光谱特征事件

        #endregion
        public MapForm()
        {
            InitializeComponent();
        }

        #region//函数

        private void InitialFullImage(OSGeo.GDAL.Dataset ds)
        {
            try
            {
                if (ds != null)
                {
                    int imgWidth = ds.RasterXSize;   //影像宽
                    int imgHeight = ds.RasterYSize;  //影像高                 
                    float ImgRatio = imgWidth / (float)imgHeight;  //影像宽高比  

                    //获取显示控件大小
                    int showWidth = pictureBox1.Width;
                    int showHeight = pictureBox1.Height;
                    float showRatio = showWidth / (float)showHeight;  //显示控件宽高比 

                    //计算实际显示区域大小，防止影像畸变显示 

                    if (showRatio >= ImgRatio)
                    {
                        bufHeight = showHeight;
                        bufWidth = (int)(showHeight * ImgRatio);
                    }
                    else
                    {
                        bufWidth = showWidth;
                        bufHeight = (int)(showWidth / ImgRatio);
                    }
                    if (Zoom >= 1)
                    {
                        imgWidth = (int)((double)imgWidth / Zoom);  //放大
                        imgHeight = (int)((double)imgHeight / Zoom);  //放大
                    }
                    else
                    {
                        bufHeight = (int)(bufHeight * Zoom);
                        bufWidth = (int)(bufWidth * Zoom);
                    }
                    //构建位图
                    bitmap = new Bitmap(bufWidth, bufHeight,
                       System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    GreyData = new int[bufWidth, bufHeight];
                    if (originX + imgWidth > ds.RasterXSize || originY + bufHeight > ds.RasterYSize)
                    {
                        originX = ds.RasterXSize - imgWidth;
                        originY = ds.RasterYSize - imgHeight;
                    }
                    if (ds.RasterCount >= 3)    //RGB显示
                    {
                        int[] r = new int[bufWidth * bufHeight];
                        Band band1 = ds.GetRasterBand(listImages[imageindex][0]);
                        band1.ReadRaster(originX, originY, imgWidth, imgHeight, r, bufWidth, bufHeight, 0, 0);
                        SrcRGB_band[imageindex][0] = r;


                        //为了显示好看，进行最大值最小值拉伸显示
                        double[] maxandmin1 = { 0, 0 };
                        band1.ComputeRasterMinMax(maxandmin1, 0);
                        bands[imageindex][0] = band1;

                        int[] g = new int[bufWidth * bufHeight];
                        Band band2 = ds.GetRasterBand(listImages[imageindex][1]);
                        band2.ReadRaster(originX, originY, imgWidth, imgHeight, g, bufWidth, bufHeight, 0, 0);
                        SrcRGB_band[imageindex][1] = g;

                        double[] maxandmin2 = { 0, 0 };
                        band2.ComputeRasterMinMax(maxandmin2, 0);
                        bands[imageindex][1] = band2;

                        int[] b = new int[bufWidth * bufHeight];
                        Band band3 = ds.GetRasterBand(listImages[imageindex][2]);
                        band3.ReadRaster(originX, originY, imgWidth, imgHeight, b, bufWidth, bufHeight, 0, 0);
                        SrcRGB_band[imageindex][2] = b;

                        double[] maxandmin3 = { 0, 0 };
                        band3.ComputeRasterMinMax(maxandmin3, 0);
                        bands[imageindex][2] = band3;


                        for (int i = 0; i < bufWidth; i++)
                        {
                            for (int j = 0; j < bufHeight; j++)
                            {
                                //转换到8位显示
                                int rVal = Convert.ToInt32(r[i + j * bufWidth]);
                                int gVal = Convert.ToInt32(g[i + j * bufWidth]);
                                int bVal = Convert.ToInt32(b[i + j * bufWidth]);

                                //下面这一行代码求得是平均值来灰度化
                                GreyData[i, j] = (int)((rVal + gVal + bVal) / 3); //获取没有拉伸的数据

                                rVal = (int)((rVal - maxandmin1[0]) / (maxandmin1[1] - maxandmin1[0]) * 255);
                                SrcRGB_band[imageindex][0][i + j * bufWidth] = rVal;      //再次获取值，获取到的是显示出来的图像Red通道的像素值

                                gVal = (int)((gVal - maxandmin2[0]) / (maxandmin2[1] - maxandmin2[0]) * 255);
                                SrcRGB_band[imageindex][1][i + j * bufWidth] = gVal;      //再次获取值，获取到的是显示出来的图像的Green通道的像素值

                                bVal = (int)((bVal - maxandmin3[0]) / (maxandmin3[1] - maxandmin3[0]) * 255);
                                SrcRGB_band[imageindex][2][i + j * bufWidth] = bVal;      //再次获取值，获取到的是显示出来的图像的Blue通道的像素值

                                Color newcolor = Color.FromArgb(rVal, gVal, bVal);
                                //GreyData[i, j] = (int)(rVal * 0.33 + gVal * 0.33 + bVal * 0.34);  //获取经过拉伸后的数据
                                bitmap.SetPixel(i, j, newcolor);
                            }
                        }
                        ifGetRGB[imageindex] = true; //获取到对应显示影像的RGB
                    }
                    else
                    {
                        int[] r = new int[bufWidth * bufHeight];
                        Band band1 = ds.GetRasterBand(listImages[imageindex][0]);
                        band1.ReadRaster(originX, originY, imgWidth, imgHeight, r, bufWidth, bufHeight, 0, 0);
                        SrcRGB_band[imageindex][0] = r;

                        //为了显示好看，进行最大值最小值拉伸显示
                        double[] maxandmin1 = { 0, 0 };
                        band1.ComputeRasterMinMax(maxandmin1, 0);
                        bands[imageindex][0] = band1;

                        for (int i = 0; i < bufWidth; i++)
                        {
                            for (int j = 0; j < bufHeight; j++)
                            {
                                int rVal = Convert.ToInt32(r[i + j * bufWidth]);
                                GreyData[i, j] = rVal;   //获取没有经过拉伸的原始像素值
                                rVal = (int)((rVal - maxandmin1[0]) / (maxandmin1[1] - maxandmin1[0]) * 255);
                                SrcRGB_band[imageindex][0][i + j * bufWidth] = rVal;
                                //GreyData[i, j] = rVal;   //获取经过拉伸的像素值
                                Color newcolor = Color.FromArgb(rVal, rVal, rVal);
                                bitmap.SetPixel(i, j, newcolor);
                            }
                        }
                        ifGetRGB[imageindex] = true; //获取到对应显示影像的RGB
                    }

                    pictureBox1.Refresh();
                    pictureBox1.Image = bitmap;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
        private void  ShowBitmap(Bitmap bitmap)//未引用
        {
            pictureBox1.Refresh();
            pictureBox1.Image = bitmap;
        }
        public class ShowHistEventArgs : EventArgs
        {
            private string filename;

            // Constructors
            public ShowHistEventArgs(string filename)
            {
                this.filename = filename;
            }

            // Location property
            public string Filename
            {
                get { return filename; }
            }
        }
        public Bitmap GetImage(OSGeo.GDAL.Dataset ds, Rectangle showRect, int[] bandlist)
        {
            int imgWidth = ds.RasterXSize;   //影像宽  
            int imgHeight = ds.RasterYSize;  //影像高  

            int BufferWidth = imgWidth; 
            int BufferHeight=imgHeight;
            /*
            float ImgRatio = imgWidth / (float)imgHeight;  //影像宽高比  

            //获取显示控件大小  
            int BoxWidth = showRect.Width;
            int BoxHeight = showRect.Height;
            float BoxRatio = imgWidth / (float)imgHeight;  //显示控件宽高比  

            //计算实际显示区域大小，防止影像畸变显示
            
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
            */
            
            int[,] RGB = new int[3, BufferWidth * BufferHeight];

            //构建位图  
            w = BufferWidth;
            h = BufferHeight;
            Bitmap bitmap = new Bitmap(BufferWidth, BufferHeight,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);  //赋值bitmap并定义为32位位图

            //bandlist.length表示数组bandlist的长度
            /*
            if (ds.RasterCount > 3)
            {
                Band band4 = ds.GetRasterBand(4);///
                double[] maxandmin4 = { 0, 0 };
                band4.ComputeRasterMinMax(maxandmin4, 0);///
                band4.ReadRaster(0, 0, width, height, band4z, w, h, 0, 0);
            }
            */
            if (ds.RasterCount >= 3)     //RGB显示  
            {
                //int[] 
                r = new int[BufferWidth * BufferHeight];
                Band band1 = ds.GetRasterBand(bandlist[0]);//bandlist[0]=1 band1为dataset类的第一个波段　R？
                band1.ReadRaster(0, 0, imgWidth, imgHeight, r, BufferWidth, BufferHeight, 0, 0);  //读取图像到内存
                //为了显示好看，进行最大最小值拉伸显示  
                double[] maxandmin1 = { 0, 0 };
                band1.ComputeRasterMinMax(maxandmin1, 0);

                //int[]
                g = new int[BufferWidth * BufferHeight];
                Band band2 = ds.GetRasterBand(bandlist[1]);
                band2.ReadRaster(0, 0, imgWidth, imgHeight, g, BufferWidth, BufferHeight, 0, 0);

                double[] maxandmin2 = { 0, 0 };
                band2.ComputeRasterMinMax(maxandmin2, 0);

                //int[] 
                b = new int[BufferWidth * BufferHeight];
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
        public Bitmap GetImage(OSGeo.GDAL.Dataset dataset)//引用
        {

            if (dataset != null)
            {
                int imgWidth = dataset.RasterXSize;   //影像宽
                int imgHeight = dataset.RasterYSize;  //影像高                 
                float ImgRatio = imgWidth / (float)imgHeight;  //影像宽高比  

                //获取显示控件大小
                
                int showWidth = pictureBox1.Width;
                int showHeight = pictureBox1.Height;
                float showRatio = showWidth / (float)showHeight;  //显示控件宽高比 

                //计算实际显示区域大小，防止影像畸变显示 

                if (showRatio >= ImgRatio)
                {
                    bufHeight = showHeight;
                    bufWidth = (int)(showHeight * ImgRatio);
                }
                else
                {
                    bufWidth = showWidth;
                    bufHeight = (int)(showWidth / ImgRatio);
                }
                
                if (Zoom >= 1)
                {
                    imgWidth = (int)((double)imgWidth / Zoom);  //放大
                    imgHeight = (int)((double)imgHeight / Zoom);  //放大
                }
                else
                {
                    bufHeight = (int)(bufHeight * Zoom);
                    bufWidth = (int)(bufWidth * Zoom);
                }
                //构建位图
                bitmap = new Bitmap(bufWidth, bufHeight,
                   System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                GreyData = new int[bufWidth, bufHeight];
                if (originX + imgWidth > dataset.RasterXSize || originY + bufHeight > dataset.RasterYSize)
                {
                    originX = dataset.RasterXSize - imgWidth;
                    originY = dataset.RasterYSize - imgHeight;
                }
                if (dataset.RasterCount >= 3)    //RGB显示
                {
                    int[] r = new int[bufWidth * bufHeight];
                    Band band1 = dataset.GetRasterBand(listImages[imageindex][0]);
                    band1.ReadRaster(originX, originY, imgWidth, imgHeight, r, bufWidth, bufHeight, 0, 0);
                    SrcRGB_band[imageindex][0] = r;


                    //为了显示好看，进行最大值最小值拉伸显示
                    double[] maxandmin1 = { 0, 0 };
                    band1.ComputeRasterMinMax(maxandmin1, 0);
                    bands[imageindex][0] = band1;

                    int[] g = new int[bufWidth * bufHeight];
                    Band band2 = dataset.GetRasterBand(listImages[imageindex][1]);
                    band2.ReadRaster(originX, originY, imgWidth, imgHeight, g, bufWidth, bufHeight, 0, 0);
                    SrcRGB_band[imageindex][1] = g;

                    double[] maxandmin2 = { 0, 0 };
                    band2.ComputeRasterMinMax(maxandmin2, 0);
                    bands[imageindex][1] = band2;

                    int[] b = new int[bufWidth * bufHeight];
                    Band band3 = dataset.GetRasterBand(listImages[imageindex][2]);
                    band3.ReadRaster(originX, originY, imgWidth, imgHeight, b, bufWidth, bufHeight, 0, 0);
                    SrcRGB_band[imageindex][2] = b;

                    double[] maxandmin3 = { 0, 0 };
                    band3.ComputeRasterMinMax(maxandmin3, 0);
                    bands[imageindex][2] = band3;


                    for (int i = 0; i < bufWidth; i++)
                    {
                        for (int j = 0; j < bufHeight; j++)
                        {
                            //转换到8位显示
                            int rVal = Convert.ToInt32(r[i + j * bufWidth]);
                            int gVal = Convert.ToInt32(g[i + j * bufWidth]);
                            int bVal = Convert.ToInt32(b[i + j * bufWidth]);

                            //下面这一行代码求得是平均值来灰度化
                            GreyData[i, j] = (int)((rVal + gVal + bVal) / 3); //获取没有拉伸的数据

                            rVal = (int)((rVal - maxandmin1[0]) / (maxandmin1[1] - maxandmin1[0]) * 255);
                            SrcRGB_band[imageindex][0][i + j * bufWidth] = rVal;      //再次获取值，获取到的是显示出来的图像Red通道的像素值

                            gVal = (int)((gVal - maxandmin2[0]) / (maxandmin2[1] - maxandmin2[0]) * 255);
                            SrcRGB_band[imageindex][1][i + j * bufWidth] = gVal;      //再次获取值，获取到的是显示出来的图像的Green通道的像素值

                            bVal = (int)((bVal - maxandmin3[0]) / (maxandmin3[1] - maxandmin3[0]) * 255);
                            SrcRGB_band[imageindex][2][i + j * bufWidth] = bVal;      //再次获取值，获取到的是显示出来的图像的Blue通道的像素值

                            Color newcolor = Color.FromArgb(rVal, gVal, bVal);
                            //GreyData[i, j] = (int)(rVal * 0.33 + gVal * 0.33 + bVal * 0.34);  //获取经过拉伸后的数据
                            bitmap.SetPixel(i, j, newcolor);
                        }
                    }
                    ifGetRGB[imageindex] = true; //获取到对应显示影像的RGB
                }
                else
                {
                    int[] r = new int[bufWidth * bufHeight];
                    Band band1 = dataset.GetRasterBand(listImages[imageindex][0]);
                    band1.ReadRaster(originX, originY, imgWidth, imgHeight, r, bufWidth, bufHeight, 0, 0);
                    SrcRGB_band[imageindex][0] = r;

                    //为了显示好看，进行最大值最小值拉伸显示
                    double[] maxandmin1 = { 0, 0 };
                    band1.ComputeRasterMinMax(maxandmin1, 0);
                    bands[imageindex][0] = band1;

                    for (int i = 0; i < bufWidth; i++)
                    {
                        for (int j = 0; j < bufHeight; j++)
                        {
                            int rVal = Convert.ToInt32(r[i + j * bufWidth]);
                            GreyData[i, j] = rVal;   //获取没有经过拉伸的原始像素值
                            rVal = (int)((rVal - maxandmin1[0]) / (maxandmin1[1] - maxandmin1[0]) * 255);
                            SrcRGB_band[imageindex][0][i + j * bufWidth] = rVal;
                            //GreyData[i, j] = rVal;   //获取经过拉伸的像素值
                            Color newcolor = Color.FromArgb(rVal, rVal, rVal);
                            bitmap.SetPixel(i, j, newcolor);
                        }
                    }
                    ifGetRGB[imageindex] = true; //获取到对应显示影像的RGB
                }
            }
            return bitmap;

        }
        private Bitmap KiResizeImage(Bitmap bmp,int Zoom)//缩放bitmap
        {
            try
            {
                int newW = bmp.Width*Zoom;
                int newH = bmp.Height * Zoom;
                //Bitmap b = new Bitmap(newW, newH);
                Bitmap b = GetImage(dataset);
                Graphics g = Graphics.FromImage(b);

                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();

                return b;
            }
            catch ( Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }
        private void BandRGB(int RGB)
        {
            //影像中的波段从1开始
            TreeNode CurrentNode = treeView1.GetNodeAt(TreeNodeX, TreeNodeY);//??
            if (CurrentNode.Index == 0)
            {
                treeView1.SelectedNode.Text = CurrentNode.Text;//选中这个节点,更新显示为R 这里选中的是地址
                listImages[CurrentNode.Parent.Index][0] = RGB;//把对应的波段赋予给R通道
                CurrentNode.Text = "R:band" + BandIndex.ToString();
            }
            if (CurrentNode.Index == 1)
            {
                treeView1.SelectedNode.Text = CurrentNode.Text;//选中这个节点,更新显示为R  这里选中的是地址
                listImages[CurrentNode.Parent.Index][1] = RGB; //把对应的波段赋予给G通道
                CurrentNode.Text = "G:band" + BandIndex.ToString();
            }
            if (CurrentNode.Index == 2)
            {
                treeView1.SelectedNode.Text = CurrentNode.Text;//选中这个节点,更新显示为R  这里选中的是地址
                listImages[CurrentNode.Parent.Index][2] = RGB; //把对应的波段赋予给B通道
                CurrentNode.Text = "B:band" + BandIndex.ToString();
            }
        }

        #endregion

        #region//事件
        private void MapForm_Load(object sender, EventArgs e)//加载MapForm事件
        {
            OSGeo.GDAL.Gdal.AllRegister();           //第一行是注册所有的格式驱动，
            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");   //第二行是支持中文路径和名称
            this.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);
        }
        private void GetBand_Click(object sender, EventArgs e)
        {
            Bandstring = sender.ToString();
            BandIndex = Convert.ToInt32(Bandstring[4] - 48);
            BandRGB(BandIndex); //从1开始
        }
        private void 显示图像ToolStripMenuItem_Click(object sender, EventArgs e)   //显示图片事件
        {
            try
            {
                if (strImagePath == null)
                    return;
                OSGeo.GDAL.Gdal.AllRegister();
                OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");    //支持中文路径
                dataset = OSGeo.GDAL.Gdal.Open(strImagePath, OSGeo.GDAL.Access.GA_ReadOnly);   //把读取到的文件赋予给dataset对象
                adfGeoTransform = new double[6];
                dataset.GetGeoTransform(adfGeoTransform);
                /*
                 * 地理坐标信息是一个含6个double型数据的数组，
                 * adfGeoTransform[1]和adfGeoTransform[5]表示东西和南北方向一个像素对应的距离，
                 * adfGeoTransform[0]和adfGeoTransform[3]表示左上角的坐标。
                 */
                Zoom = 1;
                originX = 0;
                originY = 0;

                currentBitmap = GetImage(dataset);
                pictureBox1.Image = currentBitmap;
                //InitialFullImage(dataset);

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            
        }    //事件
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) //事件
        {
            if (e.Button != MouseButtons.Right) return;
            //int n = e.Node.Level;
            switch (e.Node.Level)
            {
                case 1:
                    Point ClickPoint1 = new Point(e.X, e.Y);
                    TreeNode CurrentNode1 = treeView1.GetNodeAt(ClickPoint1);

                    CurrentNode1.ContextMenuStrip = contextMenuStrip1;
                    treeView1.SelectedNode = CurrentNode1;//选中这个节点
                    strImagePath = CurrentNode1.Text;
                    imageindex = CurrentNode1.Index;//更新当前filename索引

                    contextMenuStrip2.Items.Clear();//清除动态菜单的选项
                    dataset = OSGeo.GDAL.Gdal.Open(strImagePath, OSGeo.GDAL.Access.GA_ReadOnly);   //把读取到的文件赋予给dataset对象
                    for (int i = 0; i < dataset.RasterCount; i++)
                    {
                        System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
                        item.Size = new System.Drawing.Size(195, 24);
                        item.Name = "band" + (i + 1).ToString();
                        item.Text = "band" + (i + 1).ToString();

                        item.Click += new EventHandler(GetBand_Click);

                        contextMenuStrip2.Items.Add(item);
                    }
                    break;
                case 2:
                    Point ClickPoint2 = new Point(e.X, e.Y);
                    //检索位于指定节点的树节点
                    TreeNode CurrentNode2 = treeView1.GetNodeAt(ClickPoint2);
                    //与菜单相关联

                    CurrentNode2.ContextMenuStrip = contextMenuStrip2;
                    treeView1.SelectedNode = CurrentNode2;//选中这个节点
                    TreeNodeX = e.X;
                    TreeNodeY = e.Y;
                    break;
                default:
                    break;
            }
        }
        private void 绘制直方图ToolStripMenuItem_Click(object sender, EventArgs e)//打开直方图窗口事件
        {
            if (ShowHist != null)
                ShowHist(this, new ShowHistEventArgs(strImagePath));
        }
        private void 显示光谱特征ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpectralSignature = true;
            光谱特征.Show();
            //if (ShowSpectralSignature != null)
            //   ShowSpectralSignature(this, new ShowHistEventArgs(strImagePath));
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Hand || bitmap == null || Zoom <= 1)
                return;
            bufX = e.X;
            bufY = e.Y;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            this.Cursor = mapCursor;
            if (bitmap == null || dataset == null)
                return;
            //在显示坐标的时候应该要先展示1:1的
            if (pictureBox1.Image != null)
            {
                double[] adfGeoTransform = new double[6];
                dataset.GetGeoTransform(adfGeoTransform);
                double x = e.X / (double)pictureBox1.Image.Width * dataset.RasterXSize * adfGeoTransform[1] + adfGeoTransform[0];
                double y = e.Y / (double)pictureBox1.Image.Height * dataset.RasterYSize * adfGeoTransform[5] + adfGeoTransform[3];
                this.XY_stripLabel.Text = "X：" + x.ToString() + " Y：" + y.ToString();

            }
            if (e.Location.X > bitmap.Width || e.Location.Y > bitmap.Height)
            {
                RGB_stripLable.Text = " ";
                XY_stripLabel.Text = "  ";
                return;
            }
            int imageWidth = dataset.RasterXSize;  //原始影像宽
            int imageHeight = dataset.RasterYSize;//原始影像高
            double Zoom = 0;
            if (this.Zoom >= 1)
                Zoom = (double)bitmap.Width / dataset.RasterXSize * this.Zoom;
            else
            {
                Zoom = (double)bitmap.Width / (dataset.RasterXSize);
            }
            if (e.Location.X > 0 && e.Location.Y > 0 && e.Location.X < bitmap.Width && e.Location.Y < bitmap.Height)
            {
                double X = adfGeoTransform[0] + (e.Location.X + originX) / Zoom * adfGeoTransform[1] + (e.Location.Y + originY) / Zoom * adfGeoTransform[2];
                double Y = adfGeoTransform[3] + (e.Location.X + originX) / Zoom * adfGeoTransform[4] + (e.Location.Y + originY) / Zoom * adfGeoTransform[5];
                XY_stripLabel.Text = "(" + X.ToString("F2") + "," + Y.ToString("F2") + ")";
                RGB_stripLable.Text = bitmap.GetPixel(e.Location.X, e.Location.Y).ToString();

            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Hand || bitmap == null || Zoom <= 1)
            {
                originX = 0;
                originY = 0;
                return;
            }
            bufX = -(e.X - bufX);
            bufY = -(e.Y - bufY);
            if (originX + bufX < 0 ||
                originY + bufY < 0 ||
                originX + bufX > dataset.RasterXSize ||
                originY + bufY > dataset.RasterYSize)
                return;
            originX += bufX;
            originY += bufY;
            InitialFullImage(dataset);   //移动图像 更新图像
        }
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            RGB_stripLable.Text = " ";
            XY_stripLabel.Text = "  ";
        }
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                Zoom = Zoom * 1.2;
                Bitmap bmp = GetImage(dataset);
                currentBitmap = bmp;
                //MainForm.main.GetcurBitmap(index);
                ShowBitmap(currentBitmap);
                this.Text = "Mouse Wheeled Up";  //放大

            }
            else
            {
                Zoom = Zoom / 1.2;
                Bitmap bmp = GetImage(dataset);
                currentBitmap = bmp;
                //MainForm.main.GetcurBitmap(index);
                this.Text = "Mouse Wheeled Down";//缩小
                ShowBitmap(currentBitmap);
            }
        }
        private void 移除图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode == null)
                return;

            TreeNode pNode = this.treeView1.SelectedNode;

            if (pNode.Level != 1)
                return;
            pNode.Remove();

            //前面所定义的list
            // internal List<int[]> listImages = new List<int[]>();
            // internal List<Band[]> bands = new List<Band[]>();  // Band对象的集合
            //internal List<int[][]> SrcRGB_band = new List<int[][]>(); //显示出来的对应影像的RGB值的集合

            //internal List<bool> ifGetRGB = new List<bool>();  //判断是否存储了影像的RGB到数组

            listImages.Remove(listImages[pNode.Index]);
            bands.Remove(bands[pNode.Index]);
            SrcRGB_band.Remove(SrcRGB_band[pNode.Index]);
            ifGetRGB.Remove(ifGetRGB[pNode.Index]);


        }
        private void 清除所有ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode pNode = CommonClass.u_GetNodebyName(this.treeView1.Nodes, strImageList);
            pNode.Nodes.Clear();
            listImages.Clear();
            bands.Clear();
            SrcRGB_band.Clear();
            ifGetRGB.Clear();
        }

        #endregion

        #region//属性

        public Bitmap Picbitmap
        {
            get
            {
                return bitmap;
            }
        }
        public PictureBox ShowPic
        {
            get
            {
                return pictureBox1;
            }
            set
            {
                pictureBox1.Refresh();
                value = pictureBox1;
            }
        }
        public double zoom 
        {
            get
            {
                return Zoom;
            }
            set
            {
                Zoom = value;
                
                
                //pMainForm.GetcurBitmap(pMainForm.index);
            }
        }
        public int[][] Src_PixelData  //镜像相关的操作需要使用，获取到的是显示到屏幕上灰度值
        {
            get
            {
                return SrcRGB_band[imageindex];
            }
            set
            {
                SrcRGB_band.Add(value);
            }
        }
        public int[,] PixelValue 
        {
            get
            {
                return GreyData;
            }
        }
        public double[] AdfGeoTransform
        {
            get
            {
                return adfGeoTransform;
            }
        }
        public Cursor MapCursor
        {
            set
            {
                mapCursor = value;
            }
        }

        #endregion


    }           
}

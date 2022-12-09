using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

using OSGeo.GDAL;
using AForge.Imaging.Filters;

namespace 遥感图像处理作业_1
{
    public partial class RSImageViewControl : System.Windows.Forms.PictureBox
    {
        private OSGeo.GDAL.Dataset __Geodataset;	//Where	the	raster	data	can	be	accessed									
        private int[] __DisplayBands;	//A	3-value	vector	which	correspond	to	RGB	band								
        private Bitmap __BitMap;	//the	figure	displayed	on	the	control										
        private float __Zoom;	//Variable	controled	the	zoom	in/out	of	the	displaying	figure	
        private float __prevZoom;
        private float __maxZoom;	//Variable	controled	the	zoom	in/out	of	the	displaying	figure						
        private PointF __DispRectCenter;	//Location	of	the	displayed	rectangle											
        private bool __Draging;	//An	indicator	for	whether	draging	image	is	allowed	or	not						
        private Point __LastMouse;	//Recording	the	last	location	of	the	mouse									
        private Rectangle __DrawRect;	//A	rectangle	for	displaying	the	image	
        private Rectangle __ImageRect;							
        private string __ImagePath;	//The	file	path	where	the	image	data	were	saved.

        private int __ImageDispalyWidth;
        private int __ImageDispalyHeight;

        private int __StretchType;  //0-NO,1-GreyStretch,2-HistogramEqualize;
        private double fStardDesv;

        //Map Events
        public delegate void MapMoveEventHandler(object sender, MapMoveEventArgs e);
        public delegate void MapZoomEventHandler(object sender, MapZoomEventArgs e);
        public event MapMoveEventHandler MapPosition;
        public event MapZoomEventHandler MapZoomChanged;

        #region	Constructors
        public RSImageViewControl()
        {
            InitializeComponent();
            __Geodataset = null;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            __Draging = false;

            __ImageDispalyWidth = 0;
            __ImageDispalyHeight = 0;
            __Zoom = 1;
            __maxZoom = 10;

            __StretchType = 0;
            fStardDesv = 3.0;
        }

        public RSImageViewControl(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            __Geodataset = null;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            __Draging = false;

            __ImageDispalyWidth = 0;
            __ImageDispalyHeight = 0;
            __Zoom = 1;
            __maxZoom = 10;

            __StretchType = 0;
            fStardDesv = 3.0;
        }
        #endregion

        #region	ATTRIBUTES
        public string ImagePath
        {
            get { return __ImagePath; }
            set
            {
                if (value != null)
                {
                    __ImagePath = value;
                    try
                    {
                        OSGeo.GDAL.Gdal.AllRegister();
                        //OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
                        //OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");

                        OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(__ImagePath, OSGeo.GDAL.Access.GA_ReadOnly);
                        __Geodataset = dataset;
                       
                        if (__Geodataset != null)
                        {
                            if (__Geodataset.RasterCount >= 3)
                                __DisplayBands = new int[3] { 3, 2, 1 };
                            else
                                __DisplayBands = new int[3] { 1, 1, 1 };
                        }

                        //InitialIMG();
                        InitialFullImage();
                    }
                    catch
                    {
                        throw new Exception("Can't recognize the image file!");
                    }
                }
            }
        }

        public OSGeo.GDAL.Dataset GeoDataset
        {
            set
            {
                __Geodataset = value;
                if (__Geodataset != null)
                {
                    if (__Geodataset.RasterCount >= 3)
                        __DisplayBands = new int[3] { 1, 2, 3 };
                    else
                        __DisplayBands = new int[3] { 1, 1, 1 };
                }
                InitialIMG();
            }
            get { return __Geodataset; }
        }

        public int[] DisplayBands
        {
            get { return __DisplayBands; }
            set
            {
                //__Zoom = 1;
                __DisplayBands = value;
                InitialFullImage();
                
                //Invalidate();


                //if (__Geodataset != null)
                //{
                //    //double	scaley	=	Extent.Height	/	this.Height;
                //    //double bufWidth = __Geodataset.RasterXSize / __Zoom;
                //    //double bufHeight = __Geodataset.RasterYSize / __Zoom;
                //    //Rectangle ExtentRect = new Rectangle(0, 0, (int)bufWidth, (int)bufHeight);
                //    //__BitMap = RSImg2BitMap(__Geodataset, ExtentRect, __DisplayBands);
                //    //Invalidate();                    

                //    int bufWidth = this.Width;
                //    int bufHeight = this.Height;

                //    Point pt = new Point(0, 0);
                //    double imageWidth = bufWidth * __Zoom;
                //    double imageHeight = bufHeight * __Zoom;

                //    Rectangle ExtentRectShow = new Rectangle(0, 0, bufWidth, bufHeight);
                //    Rectangle ExtentRectImage = new Rectangle(0, 0, (int)imageWidth, (int)imageHeight);

                //    __BitMap = RSImg2BitMap(__Geodataset, pt, ExtentRectImage,ExtentRectShow, __DisplayBands);
                //    Invalidate();
                //}
                //InitialIMG();
            }
        }

        public float ZOOM
        {
            get { return __Zoom; }
            set { __Zoom = value; }
        }

        public Bitmap DispBitMap
        {
            get { return __BitMap; }
            set { __BitMap = value; }
        }

        public int StretchType
        {
            get { return __StretchType; }
            set { __StretchType = value; }
        }
        public double StardDesv
        {
            get { return fStardDesv; }
            set { fStardDesv = value; }
        }

        public int DisplayWidth
        {
            get { return __ImageDispalyWidth; }
            set { __ImageDispalyWidth = value; }
        }

        public int DisplayHeight
        {
            get { return __ImageDispalyHeight; }
            set { __ImageDispalyHeight = value; }
        }

        public Rectangle DisplayRect
        {
            get { return __DrawRect; }
            set { __DrawRect = value; }
        }

        public Rectangle ImageRect
        {
            get { return __ImageRect; }
            set { __ImageRect = value; }
        }

        public PointF ImageCenter
        {
            get { return __DispRectCenter; }
            set { __DispRectCenter = value; }
        }

        #endregion

        #region	EVENTS
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        }
        ///	<summary>																	
        ///	OnPaint	event																
        ///	</summary>																	
        protected override void OnPaint(PaintEventArgs e)
        {
            //this.BorderStyle	=	System.Windows.Forms.BorderStyle.Fixed3D;																
            Graphics g = e.Graphics;
            PaintImage(g);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            //base.OnSizeChanged(e);																		
            __BitMap = null;

            //Invalidate();

            UpdateIMG();

            Invalidate();
        }

        //Map Move arguments
        public class MapMoveEventArgs : EventArgs
        {
            private PointF location;

            // Constructors
            public MapMoveEventArgs(PointF location)
            {
                this.location = location;
            }

            // Location property
            public PointF Location
            {
                get { return location; }
            }
        }

        //Map Zoom arguments
        public class MapZoomEventArgs : EventArgs
        {
            private float fZoom;

            // Constructors
            public MapZoomEventArgs(float fZoom)
            {
                this.fZoom = fZoom;
            }

            // Location property
            public float ZoomScale
            {
                get { return fZoom; }
            }
        }

        public void PaintImage(Graphics g)
        {
            if (__BitMap != null)
            {
                switch (__StretchType)
                {
                    case 1:
                        LinearStretchA(ref __BitMap);
                        break;
                    case 2:
                        HistogramEqualize(ref __BitMap);
                        break;
                    case 3:
                        StretchStardDesv(ref __BitMap, fStardDesv);
                        break;
                    case 4:
                        StretchGaussian(ref __BitMap);
                        break;
                    default:
                        break;
                }                

                BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
                BufferedGraphics bufg = currentContext.Allocate(g, new Rectangle(0, 0, this.Width, this.Height));
                Graphics newG = bufg.Graphics;
                newG.Clear(this.BackColor);
                //newG.DrawImage(__BitMap,"	"this.DisplayRectangle,"	"__DrawRect,"	GraphicsUnit.Pixel);
                newG.DrawImage(__BitMap, __DrawRect);
                bufg.Render(g);
                newG.Dispose();
                bufg.Dispose();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            __LastMouse = e.Location;

            if (e.Button == MouseButtons.Left)
                __Draging = true;

            //this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.Focus();
            if (e.Button == MouseButtons.Left)
            { 
                __Draging = false;

                if (__BitMap != null)
                {
                    double bufWidth = this.Width / __Zoom;
                    double bufHeight = this.Height / __Zoom;

                    __ImageRect = new Rectangle((int)(__DispRectCenter.X - bufWidth / 2.0), (int)(__DispRectCenter.Y - bufHeight / 2.0), (int)bufWidth, (int)bufHeight);
                    RectangleF newRect = GetImageRect(__Geodataset, __ImageRect);
                    if (newRect == null || newRect.Width < this.Width || newRect.Height < this.Height)
                        return;

                    __ImageRect = new Rectangle((int)newRect.X, (int)newRect.Y, (int)newRect.Width, (int)newRect.Height);  
                    Rectangle ExtentRect = new Rectangle(0, 0, this.Width, this.Height);
                    __DrawRect = GetShowRect(__ImageRect, ExtentRect);

                    __BitMap = RSImg2BitMap(__Geodataset, __ImageRect, __DrawRect, __DisplayBands);

                    Invalidate();

                    //double bufWidth = __Geodataset.RasterXSize / __Zoom;
                    //double bufHeight = __Geodataset.RasterYSize / __Zoom;
                    //__DrawRect = new Rectangle((int)(__DispRectCenter.X - bufWidth / 2.0), (int)(__DispRectCenter.Y - bufHeight / 2.0), (int)bufWidth, (int)bufHeight);
                    //Invalidate();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //this.Focus();	            

            if (__Draging)
            {
                if (__Zoom <= __maxZoom)
                    return;

                __DispRectCenter = new PointF(this.__DispRectCenter.X + ((__LastMouse.X - e.X) / __Zoom), this.__DispRectCenter.Y + ((__LastMouse.Y - e.Y) / __Zoom));
            }
            __LastMouse = e.Location;

            if (MapPosition != null)
            {
                // mouse is over the image
                MapPosition(this, new MapMoveEventArgs(
                    new PointF((float)e.Location.X, (float)e.Location.Y)));
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.Focus();            
            __Zoom += __Zoom * (e.Delta / 1200.0f);

            if (__Zoom >= 1)
                __Zoom = 1;

            if ( __Zoom < __maxZoom)
                __Zoom = __maxZoom;

            this.Cursor = Cursors.WaitCursor;

            if (__Geodataset != null)
            {
                //double bufWidth = __DrawRect.Width / __Zoom;
                //double bufHeight = __DrawRect.Height / __Zoom;
                double bufWidth = this.Width / __Zoom;
                double bufHeight = this.Height / __Zoom;

                __ImageRect = new Rectangle((int)(__DispRectCenter.X - bufWidth / 2.0), (int)(__DispRectCenter.Y - bufHeight / 2.0), (int)bufWidth, (int)bufHeight);

                RectangleF newRect = GetImageRect(__Geodataset, __ImageRect);

                if (newRect == null)
                    return;
                
                __ImageRect = new Rectangle((int)newRect.X, (int)newRect.Y, (int)newRect.Width, (int)newRect.Height);  

                Rectangle ExtentRect = new Rectangle(0, 0, this.Width, this.Height);
                __DrawRect = GetShowRect(__ImageRect, ExtentRect);

                //Rectangle ExtentRect = new Rectangle(0, 0, BufferWidth, BufferHeight);
                //__DrawRect = ExtentRect;

                __BitMap = RSImg2BitMap(__Geodataset, __ImageRect, __DrawRect, __DisplayBands);

                Invalidate();
            }

            __prevZoom = __Zoom;
            this.Cursor = Cursors.Default;

            //if (e.Delta > 0)
            //    __DispRectCenter = new PointF(__DispRectCenter.X + ((e.X - (this.Width / 2)) / (2 * __Zoom)),
            //        __DispRectCenter.Y + ((e.Y - (this.Height / 2)) / (2 * __Zoom)));
            //if (__Geodataset != null)
            //{
            //    double bufWidth = __Geodataset.RasterXSize / __Zoom;
            //    double bufHeight = __Geodataset.RasterYSize / __Zoom;
            //    __DrawRect = new Rectangle((int)(__DispRectCenter.X - bufWidth / 2.0), (int)(__DispRectCenter.Y - bufHeight / 2.0), (int)bufWidth, (int)bufHeight);
            //    Invalidate();
            //}

            // notify host
            if (MapZoomChanged != null)
                MapZoomChanged(this, new MapZoomEventArgs(__prevZoom));
        }
        #endregion


        #region	METHODS
        public void HistogramEqualize(ref Bitmap currentBitMap)
        {
            if (currentBitMap != null)
            {
                HistogramEqualization filter = new HistogramEqualization();
                filter.ApplyInPlace(currentBitMap);

                //Rectangle rec = new Rectangle(0, 0, currentBitMap.Width, currentBitMap.Height);
                //BitmapData bmpData = currentBitMap.LockBits(rec, System.Drawing.Imaging.ImageLockMode.ReadWrite, currentBitMap.PixelFormat);
                //IntPtr ptr = bmpData.Scan0;
                //int bytes = currentBitMap.Width * currentBitMap.Height * 3;   //Three bands total
                //byte[] grayValues = new byte[bytes];
                //System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);

                //byte temp;
                //int[] tempArray = new int[256];
                //int[] countPixel = new int[256];
                //byte[] pixelMap = new byte[256];
                ////Calculate the pixel numbers in each gray scale level
                //for (int i = 0; i < bytes; i++)
                //{
                //    temp = grayValues[i];
                //    countPixel[temp]++;
                //}
                ////Count the ratio acculation function in each gray level
                //for (int i = 0; i < 256; i++)
                //{
                //    if (i != 0)
                //    {
                //        tempArray[i] = tempArray[i - 1] + countPixel[i];
                //    }
                //    else
                //    {
                //        tempArray[0] = countPixel[0];
                //    }
                //    pixelMap[i] = (byte)(255.0 * tempArray[i] / bytes + 0.5);
                //}
                ////Change to gray level
                //for (int i = 0; i < bytes; i++)
                //{
                //    temp = grayValues[i];
                //    grayValues[i] = pixelMap[temp];
                //}
                //System.Runtime.InteropServices.Marshal.Copy(grayValues, 0, ptr, bytes);
                //currentBitMap.UnlockBits(bmpData);
            }

            //GrayStretch(ref currentBitMap);
        }

        public void StretchStardDesv(ref Bitmap currentBitMap,double nSD)
        {
            if (currentBitMap != null)
            {
                double mean = MeanOfBitmap(currentBitMap);
                double var = VarOfBitmap(currentBitMap, mean);

                if (mean <= 0 || var <= 0)
                    return;

                double min = mean - nSD * Math.Sqrt(var);
                double max = mean + nSD * Math.Sqrt(var);

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
                byte[] lut = new byte[256];
                for (int i = 0; i < 256; i++)
                {
                    value = p * (i - min) + 0.5;
                    if (value > 0 && value < 255)
                    {
                        lut[i] = Convert.ToByte(value);
                    }
                    else if (value <= 0)
                    { lut[i] = 0; }
                    else
                    { lut[i] = 255; }
                }

                for (int i = 0; i < bytes; i++)
                {
                    grayValues[i] = lut[grayValues[i]];
                }

                System.Runtime.InteropServices.Marshal.Copy(grayValues, 0, ptr, bytes);
                currentBitMap.UnlockBits(bmpData);
            }
        }

        public void StretchGaussian(ref Bitmap currentBitMap)
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
                if ( max > 255)
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

        //根据最大、最小值进行线性增强
        public void LinearStretchA(ref Bitmap currentBitMap)
        {
            if (currentBitMap != null)
            {
                AForge.Imaging.Filters.ContrastStretch filter = new ContrastStretch();
                filter.ApplyInPlace(currentBitMap);

                //Rectangle rec = new Rectangle(0, 0, currentBitMap.Width, currentBitMap.Height);
                //BitmapData bmpData = currentBitMap.LockBits(rec, System.Drawing.Imaging.ImageLockMode.ReadWrite, currentBitMap.PixelFormat);
                //IntPtr ptr = bmpData.Scan0;
                //int bytes = currentBitMap.Width * currentBitMap.Height * 3;   //Three bands total
                //byte[] grayValues = new byte[bytes];
                //System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);

                //byte a = 255;
                //byte b = 0;
                //double p = 0.0;
                //for (int i = 0; i < bytes; i++)
                //{
                //    if (a > grayValues[i])
                //        a = grayValues[i];
                //    if (b < grayValues[i])
                //        b = grayValues[i];
                //}
                //p = 255.0 / (b - a);
                //double value = 0.0;
                //for (int i = 0; i < bytes; i++)
                //{
                //    value = p * (grayValues[i] - a) + 0.5;
                //    if (value > 0 && value < 255)
                //    {
                //        grayValues[i] = Convert.ToByte(value);
                //    }
                //    else if (value <= 0)
                //    { grayValues[i] = 0; }
                //    else
                //    { grayValues[i] = 255; }
                //}

                //System.Runtime.InteropServices.Marshal.Copy(grayValues, 0, ptr, bytes);
                //currentBitMap.UnlockBits(bmpData);
            }
        }

        //根据均值和方差进行线性增强
        public void LinearStretchB(ref Bitmap currentBitMap)
        {
            if (currentBitMap != null)
            {
                //AForge.Imaging.Filters.ContrastCorrection filter = new ContrastCorrection();
                //filter.ApplyInPlace(currentBitMap);

                double mean = MeanOfBitmap(currentBitMap);
                double var = VarOfBitmap(currentBitMap,mean);

                if (mean <= 0 || var <= 0)
                    return;

                double min = mean - 3* Math.Sqrt(var);
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
                    { grayValues[i] =  0;   }
                    else
                    { grayValues[i] =  255; }
                }

                System.Runtime.InteropServices.Marshal.Copy(grayValues, 0, ptr, bytes);
                currentBitMap.UnlockBits(bmpData);
            }
        }

        //统计均值（RGB空间）
        public double MeanOfBitmap(Bitmap currentBitMap)
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

        //统计方差函数（RGB空间）
        public double VarOfBitmap(Bitmap currentBitMap, double meanvalue)
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

        public RectangleF ExtRect(Rectangle rect, float width, float height)
        {
            double midX = rect.X + rect.Width / 2.0;
            double midY = rect.Y + rect.Height / 2.0;
            double newh = 0.0;
            double neww = 0.0;
            //Adjust	according	to	"width,"	if														
            if (rect.Width * 1 / rect.Height > width / height)
            {
                newh = (height * 1 / width) * rect.Width;
                neww = rect.Width;
                //newh	=	(rect.Height*1.0	/	rect.Width)	*	height;												
                //neww	=	width;
            }
            else
            {
                //neww	=	(rect.Width*1.0	/	rect.Height)	*	width;												
                //newh	=	height;																
                neww = (width * 1 / height) * rect.Width;
                newh = rect.Height;
            }
            RectangleF newRect = new RectangleF((float)(midX - neww / 2.0), (float)(midY - newh / 2.0), (float)neww, (float)newh);
            return newRect;
        }

        public RectangleF ExtRect1(Rectangle rect, float width, float height)
        {
            double midX = rect.X + rect.Width / 2.0;
            double midY = rect.Y + rect.Height / 2.0;
            double newh = 0.0;
            double neww = 0.0;
			
			newh = rect.Height < height ? rect.Height : height;
            neww = rect.Width < width ? rect.Width : width;
            RectangleF newRect = new RectangleF((float)(midX - neww / 2.0), (float)(midY - newh / 2.0), (float)neww, (float)newh);
            return newRect;
        }

        public void InitialIMG()
        {
            if (__Geodataset != null)
            {
                Rectangle rect = new Rectangle(0, 0, __Geodataset.RasterXSize, __Geodataset.RasterYSize);
                float width = (float)this.Width;
                float height = (float)this.Height;
                //RectangleF Extent = ExtRect(rect, width, height);
                RectangleF Extent = ExtRect1(rect, width, height);
                double scale = Extent.Width / this.Width;
                //double	scaley	=	Extent.Height	/	this.Height;													
                double bufWidth = __Geodataset.RasterXSize / scale;
                double bufHeight = __Geodataset.RasterYSize / scale;

                double bufX = (this.Width - bufWidth) / 2.0;
                double bufY = (this.Height - bufHeight) / 2.0;
                __DrawRect = new Rectangle((int)bufX, (int)bufY, (int)bufWidth, (int)bufHeight);
                Rectangle ExtentRect = new Rectangle(0, 0, (int)bufWidth, (int)bufHeight);
                __DispRectCenter = new PointF((float)(bufX + bufWidth / 2.0), (float)(bufY + bufHeight / 2.0));
                __Zoom = (float)scale;
                //__Zoom=(float)(scalex>scaley?scalex:scaley);																		
                __BitMap = RSImg2BitMap(__Geodataset, ExtentRect, __DisplayBands);
                Invalidate();
            }
        }

        public void InitialFullImage()
        {
            if (__Geodataset != null)
            {
                int imgWidth = __Geodataset.RasterXSize;   //影像宽
                int imgHeight = __Geodataset.RasterYSize;  //影像高                 
                float ImgRatio = imgWidth / (float)imgHeight;  //影像宽高比  
                
                //获取显示控件大小  
                int showWidth = this.Width;  
                int showHeight = this.Height;
                float showRatio = showWidth / (float)showHeight;  //显示控件宽高比 
                
                //计算实际显示区域大小，防止影像畸变显示 
                int bufWidth, bufHeight;
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

                Rectangle ExtentRect = new Rectangle(0, 0, bufWidth, bufHeight);
                __DrawRect = ExtentRect;
                __DispRectCenter = new PointF((float)(imgWidth / 2.0), (float)(imgHeight / 2.0));
                __Zoom = (float)bufWidth / (float)imgWidth;

                __prevZoom = __Zoom;

                __maxZoom = __Zoom;

                __ImageRect = new Rectangle(0,0,imgWidth,imgHeight);

                __BitMap = RSImg2BitMap(__Geodataset, __ImageRect, __DrawRect, __DisplayBands);

                //Invalidate();
            }
        }

        public void UpdateIMG()
        {
            if (__Geodataset != null)
            {
                double bufWidth = this.Width / __Zoom;
                double bufHeight = this.Height / __Zoom;

                __ImageRect = new Rectangle((int)(__DispRectCenter.X - bufWidth / 2.0), (int)(__DispRectCenter.Y - bufHeight / 2.0), (int)bufWidth, (int)bufHeight);
                RectangleF newRect = GetImageRect(__Geodataset, __ImageRect);
                if (newRect == null || newRect.Width < this.Width || newRect.Height < this.Height)
                    return;

                __ImageRect = new Rectangle((int)newRect.X, (int)newRect.Y, (int)newRect.Width, (int)newRect.Height);
                Rectangle ExtentRect = new Rectangle(0, 0, this.Width, this.Height);
                __DrawRect = GetShowRect(__ImageRect, ExtentRect);

                __Zoom = (float)__DrawRect.Width / (float)__ImageRect.Width;

                __BitMap = RSImg2BitMap(__Geodataset, __ImageRect, __DrawRect, __DisplayBands);

                //Invalidate();
            }
        }

        public Rectangle GetImageRect(Dataset dataset, Rectangle ExtentRectImage)
        {
            Rectangle newRect = new Rectangle(0, 0, 0, 0);

            if (dataset != null)
            {
                Point pt = ExtentRectImage.Location;

                int imgwidth = 0;
                int imgheight = 0;
                //判断当前显示区域是否有效
                if (pt.X < 0) pt.X = 0;
                if (pt.Y < 0) pt.Y = 0;

                if ((pt.X + ExtentRectImage.Width) > dataset.RasterXSize)
                {
                    imgwidth = dataset.RasterXSize - pt.X;
                }
                else
                {
                    imgwidth = ExtentRectImage.Width;
                }

                if ((pt.Y + ExtentRectImage.Height) > dataset.RasterYSize)
                {
                    imgheight = dataset.RasterYSize - pt.Y;
                }
                else
                {
                    imgheight = ExtentRectImage.Height;
                }

                newRect = new Rectangle(pt.X, pt.Y, imgwidth, imgheight);
                return newRect;
            }

            return newRect;
        }

        public Rectangle GetShowRect(Rectangle ExtentRectImage, Rectangle ExtentRectShow)
        {
            int imgWidth = ExtentRectImage.Width;   //影像宽
            int imgHeight = ExtentRectImage.Height;  //影像高                 
            float ImgRatio = imgWidth / (float)imgHeight;  //影像宽高比  

            //获取显示控件大小  
            int showWidth = ExtentRectShow.Width;
            int showHeight = ExtentRectShow.Height;
            float showRatio = showWidth / (float)showHeight;  //显示控件宽高比 

            //计算实际显示区域大小，防止影像畸变显示 
            int BufferWidth, BufferHeight;
            if (showRatio >= ImgRatio)
            {
                BufferHeight = showHeight;
                BufferWidth = (int)(showHeight * ImgRatio);
            }
            else
            {
                BufferWidth = showWidth;
                BufferHeight = (int)(showWidth / ImgRatio);
            }

            Rectangle ExtentRect = new Rectangle(0, 0, BufferWidth, BufferHeight);
            return ExtentRect;
        }

        #endregion
        ///	<summary>																	
        ///	RSImg2BitMap	function:																
        ///	to	import	data	from	the	remote	sensing	image	to	a	Bitmap	object						
        ///	</summary>																	
        ///	<param	"name=""dataset"">"																
        ///	the	GDAL	dataset	where	the	image	data	were	saved.									
        ///	</param>																	
        ///	<param	"name=""ExtentRect"">"																
        ///	a	rectangle	of	the	image	where	data	was	extracted	from.								
        ///	</param>																	
        ///	<param	"name=""displayBands"">"																
        ///	a	3-value	vector	represents	bands	order	for	RGB	display.									
        ///	</param>																	
        ///	<returns>																	
        ///	a	Bitmap	object															
        ///	</returns>																	
        public Bitmap RSImg2BitMap(OSGeo.GDAL.Dataset dataset, Rectangle ExtentRectShow, int[] displayBands)
        {
            int x1width = ExtentRectShow.Width;
            int y1height = ExtentRectShow.Height;

            Bitmap image = new Bitmap(x1width, y1height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int iPixelSize = 3;

            if (dataset != null)
            {
                BitmapData bitmapdata = image.LockBits(new Rectangle(0, 0, x1width, y1height), ImageLockMode.ReadWrite, image.PixelFormat);
                int ch = 0;
                try
                {
                    unsafe
                    {
                        // 修改标识：2012-06-12 by sjb
                        // 修改说明：通过直接判断RGB波段号来区分是灰度显示还是彩色显示
                        bool bGray;
                        if(DisplayBands[0] == DisplayBands[1] && DisplayBands[0]==DisplayBands[2] && DisplayBands[1]==DisplayBands[2])
                        {
                            bGray = true;
                        }
                        else
                        {
                            bGray = false;
                        }

                        for (int i = 1; i <= displayBands.Length; ++i)
                        {
                            OSGeo.GDAL.Band band = dataset.GetRasterBand(displayBands[i - 1]);
                            int[] buffer = new int[x1width * y1height];
                            band.ReadRaster(0, 0, __Geodataset.RasterXSize,
                                __Geodataset.RasterYSize, buffer, x1width, y1height, 0, 0);
                            int p_indx = 0;

                            // 修改标识：2012-06-12
                            //int a = (int)band.GetRasterColorInterpretation();
                            //if ((int)band.GetRasterColorInterpretation() == 5)
                            //    ch = 0;
                            //if ((int)band.GetRasterColorInterpretation() == 4)
                            //    ch = 1;
                            //if ((int)band.GetRasterColorInterpretation() == 3)
                            //    ch = 2;
                            //if ((int)band.GetRasterColorInterpretation() != 2)

                            // 修改标识：2012-06-12 
                            // 修改说明：BITMAP对应的是BGR，所以在红波段时候设置ch为2，蓝波段设置ch为0
                            if(i==1)
                            {
                                ch = 2;
                            }
                            if(i==2)
                            {
                                ch = 1;
                            }
                            if(i==3)
                            {
                                ch = 0;
                            }

                            if(bGray == false)
                            {
                                double maxVal = 0.0;
                                double minVal = 0.0;
                                maxVal = GetMaxWithoutNoData(dataset,
                                         displayBands[i - 1], -9999.0);
                                minVal = GetMinWithoutNoData(dataset,
                                         displayBands[i - 1], -9999.0);
                                for (int y = 0; y < y1height; y++)
                                {
                                    byte* row = (byte*)bitmapdata.Scan0 +
                                                      (y * bitmapdata.Stride);
                                    for (int x = 0; x < x1width; x++, p_indx++)
                                    {
                                        byte tempVal = shift2Byte(buffer[p_indx], maxVal, minVal, -9999.0);
                                        row[x * iPixelSize + ch] = tempVal;
                                    }
                                }
                            }
                            else
                            {
                                double maxVal = 0.0;
                                double minVal = 0.0;
                                maxVal = GetMaxWithoutNoData(dataset,
                                         displayBands[i - 1], -9999.0);
                                minVal = GetMinWithoutNoData(dataset,
                                         displayBands[i - 1], -9999.0);
                                for (int y = 0; y < y1height; y++)
                                {
                                    byte* row = (byte*)bitmapdata.Scan0 +
                                                (y * bitmapdata.Stride);
                                    for (int x = 0; x < x1width; x++, p_indx++)
                                    {
                                        byte tempVal = shift2Byte<int>
                                                       (buffer[p_indx], maxVal, minVal, -9999.0);
                                        row[x * iPixelSize] = tempVal;
                                        row[x * iPixelSize + 1] = tempVal;
                                        row[x * iPixelSize + 2] = tempVal;
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    image.UnlockBits(bitmapdata);
                }
            }
            return image;
        }

        public Bitmap RSImg2BitMap(OSGeo.GDAL.Dataset dataset, Rectangle ExtentRectImage, Rectangle ExtentRectShow, int[] displayBands)
        {
            if (dataset != null)
            {
                int x1width = ExtentRectShow.Width;
                int y1height = ExtentRectShow.Height;

                Bitmap image = new Bitmap(x1width, y1height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                int iPixelSize = 3;

                Point pt = new Point(ExtentRectImage.X, ExtentRectImage.Y);
            
                int imgwidth = 0;
                int imgheight = 0;
                //判断当前显示区域是否有效
                if (pt.X < 0) pt.X = 0;
                if (pt.Y < 0) pt.Y = 0;

                if ((pt.X + ExtentRectImage.Width) > dataset.RasterXSize) 
                {
                    imgwidth = dataset.RasterXSize - pt.X;
                }
                else
                {
                    imgwidth = ExtentRectImage.Width;
                }

                if ((pt.Y + ExtentRectImage.Height) > dataset.RasterYSize)
                {
                    imgheight = dataset.RasterYSize - pt.Y;
                }
                else
                {
                    imgheight = ExtentRectImage.Height;
                }

                BitmapData bitmapdata = image.LockBits(new Rectangle(0, 0, x1width, y1height), ImageLockMode.ReadWrite, image.PixelFormat);
                int ch = 0;
                try
                {
                    unsafe
                    {
                        // 修改标识：2012-06-12 by sjb
                        // 修改说明：通过直接判断RGB波段号来区分是灰度显示还是彩色显示
                        bool bGray;
                        if (displayBands[0] == displayBands[1] && displayBands[0] == displayBands[2] && DisplayBands[1] == displayBands[2])
                        {
                            bGray = true;
                        }
                        else
                        {
                            bGray = false;
                        } 
 
                        if (bGray == false)
                        {
                            for (int i = 1; i <= displayBands.Length; ++i)
                            {
                                // 修改说明：BITMAP对应的是BGR，所以在红波段时候设置ch为2，蓝波段设置ch为0
                                if (i == 1)
                                {
                                    ch = 2;
                                }
                                if (i == 2)
                                {
                                    ch = 1;
                                }
                                if (i == 3)
                                {
                                    ch = 0;
                                }

                                //读取影像数据
                                OSGeo.GDAL.Band band = dataset.GetRasterBand(displayBands[i - 1]);
                                double pdfMin, pdfMax, pdfMean, pdfStdDev;
                                int p_indx = 0;

                                switch (band.DataType)
                                {
                                    case DataType.GDT_Byte:                                   
                                        byte[] bytebuffer = new byte[x1width * y1height];
                                        band.ReadRaster(pt.X, pt.Y, imgwidth, imgheight, bytebuffer, x1width, y1height, 0, 0);
                                        p_indx = 0;
                                        //band.ComputeStatistics(false, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev,null,null);
                                        band.GetStatistics(0, 1, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev);

                                        for (int y = 0; y < y1height; y++)
                                        {
                                            byte* row = (byte*)bitmapdata.Scan0 +
                                                              (y * bitmapdata.Stride);
                                            for (int x = 0; x < x1width; x++, p_indx++)
                                            {
                                                byte tempVal = shift2Byte<byte>(bytebuffer[p_indx], pdfMax, pdfMin);
                                                row[x * iPixelSize + ch] = tempVal;
                                            }
                                        }
                                        break;

                                    case DataType.GDT_UInt16:
                                        Int16[] ushortbuffer = new Int16[x1width * y1height];
                                        band.ReadRaster(pt.X, pt.Y, imgwidth, imgheight, ushortbuffer, x1width, y1height, 0, 0);
                                        p_indx = 0;
                                        //band.ComputeStatistics(false, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev,null,null);
                                        band.GetStatistics(0, 1, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev);

                                        for (int y = 0; y < y1height; y++)
                                        {
                                            byte* row = (byte*)bitmapdata.Scan0 +
                                                              (y * bitmapdata.Stride);
                                            for (int x = 0; x < x1width; x++, p_indx++)
                                            {
                                                byte tempVal = shift2Byte<Int16>(ushortbuffer[p_indx], pdfMax, pdfMin);
                                                row[x * iPixelSize + ch] = tempVal;
                                            }
                                        }
                                        break;
                                    case DataType.GDT_Int16:
                                        Int16[] shortbuffer = new Int16[x1width * y1height];
                                        band.ReadRaster(pt.X, pt.Y, imgwidth, imgheight, shortbuffer, x1width, y1height, 0, 0);
                                        p_indx = 0;
                                        //band.ComputeStatistics(false, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev,null,null);
                                        band.GetStatistics(0, 1, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev);

                                        for (int y = 0; y < y1height; y++)
                                        {
                                            byte* row = (byte*)bitmapdata.Scan0 +
                                                              (y * bitmapdata.Stride);
                                            for (int x = 0; x < x1width; x++, p_indx++)
                                            {
                                                byte tempVal = shift2Byte<Int16>(shortbuffer[p_indx], pdfMax, pdfMin);
                                                row[x * iPixelSize + ch] = tempVal;
                                            }
                                        }
                                        break;

                                    case DataType.GDT_UInt32:
                                        int[] ubuffer = new int[x1width * y1height];
                                        band.ReadRaster(pt.X, pt.Y, imgwidth, imgheight, ubuffer, x1width, y1height, 0, 0);
                                        p_indx = 0;
                                        //band.ComputeStatistics(false, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev,null,null);
                                        band.GetStatistics(0, 1, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev);

                                        for (int y = 0; y < y1height; y++)
                                        {
                                            byte* row = (byte*)bitmapdata.Scan0 +
                                                              (y * bitmapdata.Stride);
                                            for (int x = 0; x < x1width; x++, p_indx++)
                                            {
                                                byte tempVal = shift2Byte<int>(ubuffer[p_indx], pdfMax, pdfMin);
                                                row[x * iPixelSize + ch] = tempVal;
                                            }
                                        }
                                        break;

                                    case DataType.GDT_Int32:
                                        int[] buffer = new int[x1width * y1height];
                                        band.ReadRaster(pt.X, pt.Y, imgwidth, imgheight, buffer, x1width, y1height, 0, 0);
                                        p_indx = 0;
                                        //band.ComputeStatistics(false, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev,null,null);
                                        band.GetStatistics(0, 1, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev);

                                        for (int y = 0; y < y1height; y++)
                                        {
                                            byte* row = (byte*)bitmapdata.Scan0 +
                                                              (y * bitmapdata.Stride);
                                            for (int x = 0; x < x1width; x++, p_indx++)
                                            {
                                                byte tempVal = shift2Byte<int>(buffer[p_indx], pdfMax, pdfMin);
                                                row[x * iPixelSize + ch] = tempVal;
                                            }
                                        }
                                        break;

                                    case DataType.GDT_Float32:
                                        float[] floatbuffer = new float[x1width * y1height];
                                        band.ReadRaster(pt.X, pt.Y, imgwidth, imgheight, floatbuffer, x1width, y1height, 0, 0);
                                        p_indx = 0;  

                                        band.GetStatistics(0, 1, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev);

                                        for (int y = 0; y < y1height; y++)
                                        {
                                            byte* row = (byte*)bitmapdata.Scan0 +
                                                              (y * bitmapdata.Stride);
                                            for (int x = 0; x < x1width; x++, p_indx++)
                                            {
                                                byte tempVal = shift2Byte<float>(floatbuffer[p_indx], pdfMax, pdfMin);
                                                row[x * iPixelSize + ch] = tempVal;
                                            }
                                        }
                                        break;
                                    case DataType.GDT_Float64:
                                        double[] doublebuffer = new double[x1width * y1height];
                                        band.ReadRaster(pt.X, pt.Y, imgwidth, imgheight, doublebuffer, x1width, y1height, 0, 0);
                                        p_indx = 0;  

                                        band.GetStatistics(0, 1, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev);

                                        for (int y = 0; y < y1height; y++)
                                        {
                                            byte* row = (byte*)bitmapdata.Scan0 +
                                                              (y * bitmapdata.Stride);
                                            for (int x = 0; x < x1width; x++, p_indx++)
                                            {
                                                byte tempVal = shift2Byte<double>(doublebuffer[p_indx], pdfMax, pdfMin);
                                                row[x * iPixelSize + ch] = tempVal;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            OSGeo.GDAL.Band band = dataset.GetRasterBand(displayBands[0]);
                            int[] buffer = new int[x1width * y1height];
                            //band.ReadRaster(0, 0, __Geodataset.RasterXSize,
                            //    __Geodataset.RasterYSize, buffer, x1width, y1height, 0, 0);
                            //band.ReadRaster(pt.X, pt.Y, ExtentRectImage.Width, ExtentRectImage.Height, buffer, x1width, y1height, 0, 0);
                            band.ReadRaster(pt.X, pt.Y, imgwidth, imgheight, buffer, x1width, y1height, 0, 0);
                            int p_indx = 0;

                            double maxVal = 0.0;
                            double minVal = 0.0;
                            maxVal = GetMaxWithoutNoData(dataset,
                                        displayBands[0], -9999.0);
                            minVal = GetMinWithoutNoData(dataset,
                                        displayBands[0], -9999.0);
                            for (int y = 0; y < y1height; y++)
                            {
                                byte* row = (byte*)bitmapdata.Scan0 +
                                            (y * bitmapdata.Stride);
                                for (int x = 0; x < x1width; x++, p_indx++)
                                {
                                    byte tempVal = shift2Byte<int>
                                                    (buffer[p_indx], maxVal, minVal, -9999.0);
                                    row[x * iPixelSize] = tempVal;
                                    row[x * iPixelSize + 1] = tempVal;
                                    row[x * iPixelSize + 2] = tempVal;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    image.UnlockBits(bitmapdata);
                }

                return image;
            }

            return null;
        }

        #region	RASTERoperations
        ///	<summary>																	
        ///	Function	of	shift2Byte															
        ///	</summary>																	
        ///	<remarks>this	function	will	shift	a	value	into	a	range	of	byte:	0~255	to	be	displayed	in	the	graphics.</remarks>
        ///	<typeparam	"name=""T"">the"	type	of	the	value</typeparam>												
        ///	<param	"name=""val"">the"	value	that	will	be	converted	to	byte</param>									
        ///	<param	"name=""Maximum"">the"	maximum	value	range</param>													
        ///	<param	"name=""Minimum"">the"	minimum	value	range</param>													
        ///	<returns>a	value	within	the	byte	range</returns>												
        public byte shift2Byte<T>(T val, double Maximum, double Minimum)
        {
            // 修改标识：20120612 by sjb
            //double a = 255 / (Maximum - Minimum);
            //double b = 255 - a * Maximum;

            //double tempVal = Convert.ToDouble(val);
            //double value = a * tempVal + b;

            //double b = 255 - (255 / (Maximum - Minimum)) * Maximum;
            //double tempVal = Convert.ToDouble(val);
            //byte value = Convert.ToByte(a * tempVal + b);

            double a = 255 / (Maximum - Minimum);
            double tempVal = Convert.ToDouble(val);
            double value = (tempVal - Minimum) * a;

            if (value > 0 && value < 255)
            {
                return Convert.ToByte(value);
            }
            else if (value <= 0)
            { return 0;   }
            else
            { return 255; }
        }
        ///	<summary>																	
        ///	Function	of	shift2Byte															
        ///	</summary>																	
        ///	<remarks>this	function	will	shift	a	value	into	a	range	of	byte:	0~255	to	be	displayed	in	the	graphics.</remarks>
        ///	<typeparam	"name=""T"">the"	type	of	the	value</typeparam>												
        ///	<param	"name=""val"">the"	value	that	will	be	converted	to	byte</param>									
        ///	<param	"name=""Maximum"">the"	maximum	value	range</param>													
        ///	<param	"name=""Minimum"">the"	minimum	value	range</param>													
        ///	<param	"name=""noData"">the"	value	for	the	non-sens	pixel</param>											
        ///	<returns>a	value	within	the	byte	range</returns>												
        public byte shift2Byte<T>(T val, double Maximum, double Minimum, double noData)
        {
            double a = 0.0;
            double b = 0.0;
            double tempVal = Convert.ToDouble(val);
            a = 254 / (Maximum - Minimum);
            b = 255 - (254 / (Maximum - Minimum)) * Maximum;
            if (Math.Abs(tempVal) > Math.Abs(noData))
                return 0;
            try
            {
                //return Convert.ToByte(a * tempVal + b);
                double tt = a * (tempVal - Minimum);
                if (tt > 0 && tt < 255)
                    return Convert.ToByte(tt);
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }
        ///	<summary>																	
        ///	Function	of	GetMaxWithoutNoData															
        ///	</summary>																	
        ///	<remarks>Get	the	maximum	data	of	certain	band	without	the	nodata	values.</remarks>							
        ///	<param	"name=""band"">the"	band	that	will	be	statistically	checked.</param>										
        ///	<returns>the	maximum	values.</returns>															
        public double GetMaxWithoutNoData(OSGeo.GDAL.Dataset ds, int bandNumb, double __NoData)
        {
            double max = 0.0;
            double tempMax = 0.0;
            int index = 0;
            Band tempBand = ds.GetRasterBand(bandNumb);
            tempBand.GetMaximum(out	tempMax, out index);
            if (Math.Abs(tempMax) < Math.Abs(__NoData))
                max = tempMax;
            else
            {
                OSGeo.GDAL.Band band;
                band = ds.GetRasterBand(bandNumb);
                //the	number	of	columns															
                int xSize = ds.RasterXSize;
                //the	number	of	rows															
                int ySize = ds.RasterYSize;
                double[] bandData = new double[xSize * ySize];
                //Read	the	data	into	the	bandData	matrix.												
                OSGeo.GDAL.CPLErr err = band.ReadRaster(0, 0, xSize, ySize, bandData, xSize, ySize, 0, 0);
                for (long i = 0; i < xSize * ySize; i++)
                {
                    if (bandData[i] > max & (Math.Abs(bandData[i]) < Math.Abs(__NoData)))
                        max = bandData[i];
                }
            }
            return max;
        }
        ///	<summary>																	
        ///	Function	of	GetMinWithoutNoData															
        ///	</summary>																	
        ///	<remarks>Get	the	maximum	data	of	certain	band	without	the	nodata	values.</remarks>							
        ///	<param	"name=""band"">the"	band	that	will	be	statistically	checked</param>										
        ///	<returns>the	maximum	values.</returns>															
        public double GetMinWithoutNoData(OSGeo.GDAL.Dataset ds, int bandNumb, double __NoData)
        {
            double min = Math.Abs(__NoData);
            double tempMin = 0.0;
            int index = 0;
            Band tempBand = ds.GetRasterBand(bandNumb);
            tempBand.GetMinimum(out	tempMin, out index);
            if (Math.Abs(tempMin) < Math.Abs(__NoData))
                min = tempMin;
            else
            {
                OSGeo.GDAL.Band band;
                band = ds.GetRasterBand(bandNumb);
                //the	number	of	columns															
                int xSize = ds.RasterXSize;
                //the	number	of	rows															
                int ySize = ds.RasterYSize;
                double[] bandData = new double[xSize * ySize];
                //Read	the	data	into	the	bandData	matrix.												
                OSGeo.GDAL.CPLErr err = band.ReadRaster(0, 0, xSize, ySize, bandData, xSize, ySize, 0, 0);
                for (long i = 0; i < xSize * ySize; i++)
                {
                    if (bandData[i] < min & (Math.Abs(bandData[i]) < Math.Abs(__NoData)))
                        min = bandData[i];
                }
            }

            return min;
        }
        ///	<summary>																	
        ///	Funcion	of	GetDatasetType															
        ///	</summary>																	
        ///	<param	"name=""band"">the"	band	where	the	data	type	will	be	defined.</param>								
        ///	<returns>0	is	the	"byte,"	1	is	"int,"	2	is	"double,"	and	3	is	unknown.</returns>				
        public byte GetDatasetType(OSGeo.GDAL.Band band)
        {
            switch (band.DataType)
            {
                case OSGeo.GDAL.DataType.GDT_Byte:
                    return 0;
                case OSGeo.GDAL.DataType.GDT_CFloat32:
                case OSGeo.GDAL.DataType.GDT_CFloat64:
                case OSGeo.GDAL.DataType.GDT_Float32:
                case OSGeo.GDAL.DataType.GDT_Float64:
                    return 2;
                case OSGeo.GDAL.DataType.GDT_TypeCount:
                case OSGeo.GDAL.DataType.GDT_Unknown:
                    return 3;
                default:
                    return 1;
            }
        }

        #endregion
    }														
}																		



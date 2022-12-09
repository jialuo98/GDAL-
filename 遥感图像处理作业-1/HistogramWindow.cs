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
using AForge.Imaging;
using 遥感图像处理作业_1.Controls;
using WeifenLuo.WinFormsUI.Docking;
namespace 遥感图像处理作业_1
{
    public partial class HistogramWindow : DockContent
    {
        private static Color[] colors = new Color[] 
        {
			Color.FromArgb(192, 0, 0),
			Color.FromArgb(0, 192, 0),
			Color.FromArgb(0, 0, 192),
			Color.FromArgb(128, 128, 128),
		};

        private ImageStatistics stat;
        private AForge.Math.Histogram activeHistogram = null;

        private int currentImageHash = 0;

        private OSGeo.GDAL.Dataset __Geodataset;	//Where	the	raster	data	can	be	accessed	
        private string __strFilename;

        public HistogramWindow()
        {
            InitializeComponent();

            __Geodataset = null;
        }
        public OSGeo.GDAL.Dataset GeoDataset
        {
            set
            {
                __Geodataset = value;
            }
            get { return __Geodataset; }
        }
        public string Filename
        {
            set
            {
                __strFilename = value;
            }
            get { return __strFilename; }
        }
        public void GatherStatistics(Bitmap image)      //收集统计数值 
        {
            // avoid calculation in the case of the same image
            if (image != null)
            {
                if (currentImageHash == image.GetHashCode())  //current 当前的 currentImageHash当前图片的哈希表
                    return;
                currentImageHash = image.GetHashCode();
            }

            if (image != null)
                System.Diagnostics.Debug.WriteLine("=== Gathering histogram");

            // busy
            Capture = true;
            Cursor = Cursors.WaitCursor;

            // get statistics
            stat = (image == null) ? null : new ImageStatistics(image);

            // free
            Cursor = Cursors.Arrow;
            Capture = false;

            // clean combo
            channelCombo.Items.Clear();
            channelCombo.Enabled = false;

            if (stat != null)
            {
                if (!stat.IsGrayscale)
                {
                    // RGB picture
                    channelCombo.Items.AddRange(new object[] { "Red", "Green", "Blue" });
                    channelCombo.Enabled = true;
                }
                else
                {
                    // grayscale picture
                    channelCombo.Items.Add("Gray");
                }
                channelCombo.SelectedIndex = 0;
            }
            else
            {
                histogram.Values = null;
                meanLabel.Text = String.Empty;
                stdDevLabel.Text = String.Empty;
                medianLabel.Text = String.Empty;
                minLabel.Text = String.Empty;
                maxLabel.Text = String.Empty;
                levelLabel.Text = String.Empty;
                countLabel.Text = String.Empty;
                percentileLabel.Text = String.Empty;
            }
        }
        private void channelCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ss = channelCombo.SelectedItem.ToString();
            int nBandnum = Convert.ToInt32(GetBandNum(ss));
            OSGeo.GDAL.Gdal.AllRegister();
            OSGeo.GDAL.Dataset dataset = OSGeo.GDAL.Gdal.Open(__strFilename, OSGeo.GDAL.Access.GA_ReadOnly);
            OSGeo.GDAL.Band band = dataset.GetRasterBand(nBandnum);

            double pdfMin, pdfMax, pdfMean, pdfStdDev;
            band.GetStatistics(0, 1, out pdfMin, out pdfMax, out pdfMean, out pdfStdDev);

            //再进行直方图统计
            int[] histData = new int[256];
            band.GetHistogram(pdfMin - 0.5, pdfMax + 0.5, 256, histData, 0, 1, null, null);
            histogram.Color = colors[2];
            histogram.Values = histData;
            meanLabel.Text = pdfMean.ToString("F2");
            stdDevLabel.Text = pdfStdDev.ToString("F2");
            //medianLabel.Text = activeHistogram.Median.ToString();
            minLabel.Text = pdfMin.ToString();
            maxLabel.Text = pdfMax.ToString();
        }
        public static string GetBandNum(string bandname)
        {
            string name = "";
            int n = bandname.LastIndexOf("_");
            if (n >= 0)
            {
                name = bandname.Remove(0, n + 1);   //从0开始删除n+1个字符后并返回
            }
            else
            {
                name = bandname;
            }
            return name;
        }
        public void ShowChannel(int channel)
        {
            if ((channel >= 0) && (channel <= 2))
            {
                if (!stat.IsGrayscale)
                {
                    histogram.Color = colors[channel];
                    activeHistogram = (channel == 0) ? stat.Red : (channel == 1) ? stat.Green : stat.Blue;
                }
            }
            else if (channel == 3)
            {
                if (stat.IsGrayscale)
                {
                    histogram.Color = colors[3];
                    activeHistogram = stat.Gray;
                }
            }

            if (activeHistogram != null)
            {
                histogram.Values = activeHistogram.Values;

                meanLabel.Text = activeHistogram.Mean.ToString("F2");
                stdDevLabel.Text = activeHistogram.StdDev.ToString("F2");
                medianLabel.Text = activeHistogram.Median.ToString();
                minLabel.Text = activeHistogram.Min.ToString();
                maxLabel.Text = activeHistogram.Max.ToString();
            }
        }
        public void SwitchChannel(int channel)
        {
            if ((channel >= 0) && (channel <= 2))
            {
                if (!stat.IsGrayscale)
                {
                    histogram.Color = colors[channel];
                    activeHistogram = (channel == 0) ? stat.Red : (channel == 1) ? stat.Green : stat.Blue;
                }
            }
            else if (channel == 3)
            {
                if (stat.IsGrayscale)
                {
                    histogram.Color = colors[3];
                    activeHistogram = stat.Gray;
                }
            }

            if (activeHistogram != null)
            {
                histogram.Values = activeHistogram.Values;

                meanLabel.Text = activeHistogram.Mean.ToString("F2");
                stdDevLabel.Text = activeHistogram.StdDev.ToString("F2");
                medianLabel.Text = activeHistogram.Median.ToString();
                minLabel.Text = activeHistogram.Min.ToString();
                maxLabel.Text = activeHistogram.Max.ToString();
            }
        }
        private void histogram_PositionChanged(object sender, HistogramEventArgs e)
        {
            int pos = e.Position;

            if (pos != -1)
            {
                levelLabel.Text = pos.ToString();
                countLabel.Text = activeHistogram.Values[pos].ToString();
                percentileLabel.Text = ((float)activeHistogram.Values[pos] * 100 / stat.PixelsCount).ToString("F2");
            }
            else
            {
                levelLabel.Text = "";
                countLabel.Text = "";
                percentileLabel.Text = "";
            }
        }
        private void histogram_SelectionChanged(object sender, HistogramEventArgs e)
        {
            int min = e.Min;
            int max = e.Max;
            int count = 0;

            levelLabel.Text = min.ToString() + "..." + max.ToString();

            // count pixels
            for (int i = min; i <= max; i++)
            {
                count += activeHistogram.Values[i];
            }
            countLabel.Text = count.ToString();
            percentileLabel.Text = ((float)count * 100 / stat.PixelsCount).ToString("F2");
        }
        private void logCheck_CheckedChanged(object sender, EventArgs e)
        {
            histogram.LogView = logCheck.Checked;
        }
    }
}

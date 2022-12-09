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
    public partial class 影像显示Form : Form
    {
        public 影像显示Form()
        {
            InitializeComponent();
        }
        public void ShowBitmap(Bitmap b)
        {
            pictureBox1.Image = b;
        }
    }
}

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
    public partial class 影像粗校正Form : Form
    {
        public 影像粗校正Form()
        {
            InitializeComponent();
        }

        double[] 影像参数 = { 0, 105.1846987, 30.6211769, 824.501, -4, -1, 358.5, 36, 24, 24, 0.1 };
        //double[] xjcs = {36,24,24 };
      


        //确定
        private void button1_Click(object sender, EventArgs e)
        {
            影像参数[0] = 1;
            影像参数[1]=Convert.ToDouble(Xs.Text);
            影像参数[2] = Convert.ToDouble(Ys.Text);
            影像参数[3] = Convert.ToDouble(Zs.Text);
            影像参数[4] = Convert.ToDouble(φ.Text);
            影像参数[5] = Convert.ToDouble(w.Text);
            影像参数[6] = Convert.ToDouble(k.Text);
            影像参数[7] = Convert.ToDouble(w1.Text);
            影像参数[8] = Convert.ToDouble(h1.Text);
            影像参数[9] = Convert.ToDouble(f.Text);
            影像参数[10] = Convert.ToDouble(u.Text);

            this.Close();
        }
        //取消
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Jhcjz_Load(object sender, EventArgs e)
        {
            Xs.Text = 影像参数[1].ToString("0.0000");
            Ys.Text = 影像参数[2].ToString("0.0000");
            Zs.Text = 影像参数[3].ToString("0.0000");
            φ.Text = 影像参数[4].ToString("0.00");
            w.Text = 影像参数[5].ToString("0.00");
            k.Text = 影像参数[6].ToString("0.00");
            w1.Text = 影像参数[7].ToString();
            h1.Text = 影像参数[8].ToString();
            f.Text = 影像参数[9].ToString();
            u.Text = 影像参数[10].ToString("0.00");

        }

        public double[] rcs()
        {
            return 影像参数;
        }
    }
}

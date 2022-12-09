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
    public partial class 辐射定标Form : Form
    {
        public 辐射定标Form()
        {
            InitializeComponent();
        }
        double[] a = { 0, 0.1585, 0.8765, 0.1883, 0.9742, 0.1740, 0.7652 };

        private void button1_Click(object sender, EventArgs e)
        {
            a[0] = 1;
            a[1] = Convert.ToDouble(textBox1.Text);
            a[2] = Convert.ToDouble(textBox2.Text);
            a[3] = Convert.ToDouble(textBox3.Text);
            a[4] = Convert.ToDouble(textBox4.Text);
            a[5] = Convert.ToDouble(textBox5.Text);
            a[6] = Convert.ToDouble(textBox6.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 辐射定标_Load(object sender, EventArgs e)
        {
            textBox1.Text = a[1].ToString("0.0000");
            textBox2.Text = a[2].ToString("0.0000");
            textBox3.Text = a[3].ToString("0.0000");
            textBox4.Text = a[4].ToString("0.0000");
            textBox5.Text = a[5].ToString("0.0000");
            textBox6.Text = a[6].ToString("0.0000");
        }

        public double[] rcs()
        {
            return a;
        }
    }
}

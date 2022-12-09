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
    public partial class 保存Form : Form
    {
        public 保存Form()
        {
            InitializeComponent();
        }

        private void 选择路径_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "remote sensing image|*.tif";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = dlg.FileName;
            }
        }
    }
}

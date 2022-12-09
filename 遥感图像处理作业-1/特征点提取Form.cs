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
    public partial class 特征点提取Form : Form
    {
        public 特征点提取Form()
        {
            InitializeComponent();
        }
        public void xs(int[,] a, int c)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("序号", typeof(String));
            dt.Columns.Add("图一X", typeof(String));
            dt.Columns.Add("图一Y", typeof(String));
            dt.Columns.Add("图二X", typeof(String));
            dt.Columns.Add("图二Y", typeof(String));
            for (int i = 1; i < c; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = i;                        //将列数据赋给表
                dr[1] = (int)(a[i, 1]);                         //将行数据赋给表
                dr[2] = (int)(a[i, 2]);
                dr[3] = (int)(a[i, 3]);
                dr[4] = (int)(a[i, 4]);
                dt.Rows.Add(dr);                                    //将这行数据加入到datatable中
            }
            this.dataGridView1.DataSource = dt;
        }
    }
}

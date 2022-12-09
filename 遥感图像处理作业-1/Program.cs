using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 遥感图像处理作业_1
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
    class CommonClass
    {
        static public TreeNode u_GetNodebyName(TreeNodeCollection pTreeNode, string strNodeName)
        {
            foreach (TreeNode node in pTreeNode)
            {
                if (node.Text == strNodeName)
                {
                    return node;
                }
            }

            return null;
        }
    }
}

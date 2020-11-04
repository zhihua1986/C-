using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_doem_
{

    public partial class Form1 : Form
    {
        private UserControl1 uc1;
        //private UserControl2 uc2;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        //private UserControl3 uc3;
        public Form1()
        {
            InitializeComponent();
            uc1 = new UserControl1();
            //uc2 = new UserControl2();
            //uc3 = new UserControl3();
        }

        //if (treeView1.SelectedNode == null)
        //{
        //    MessageBox.Show("请选择一个节点","提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}
        //else
        //{
        //    Form2 Myform2 = new Form2();//实例化窗体
        //    Myform2.TopLevel = false;
        //    this.panel1.Controls.Add(Myform2);
        //    Myform2.FormBorderStyle = FormBorderStyle.None;
        //    Myform2.Show();
        //}
        //节点选择属性的方法
        //treeView1.SelectedNode.IsSelected.CompareTo();
        //treeView1.SelectedNode = new TreeNode();

        private void button1_Click(object sender, EventArgs e)
        {

            

            
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.splitContainer1.Panel2.Controls.Clear();
            
            switch (this.treeView1.SelectedNode.Text)
            {
                case "用户界面1":
                    this.splitContainer1.Panel2.Controls.Add(uc1);
                    break;
                //case "用户界面2":
                //    this.splitContainer1.Panel2.Controls.Add(uc2);
                //    break;
                //case "用户界面3":
                //    this.splitContainer1.Panel2.Controls.Add(uc3);
                //    break;
                default:
                    break;
            }
        }
}

//使用命名空间
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//用户项目工程自定义命名空间HelloWorld
namespace WindowsFormsApplication1
{
    //定义了一个名称为Form1的公共类，并且在定义类的同事创建了一个这个类的对象，名为Form
    //partial关键字
    public partial class Form1 : Form
    {
        //与类同名的构造方法
        public Form1()
        {
            InitializeComponent();
        }
        //用户自定义方法，双体加载时由Form对象调用，双击窗体自动创建（方法，就是在初始化窗口时，通过具体对象Form调用）
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //按下Send按钮
            textBox1.Text = "^_^Hello,World^_^";    //文本框显示
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //按下Clear按钮
            textBox1.Text = "";                      //文本框清空
        }
    }
}

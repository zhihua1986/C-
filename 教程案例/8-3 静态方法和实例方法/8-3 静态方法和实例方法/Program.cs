using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8_3_静态方法和实例方法
{
    class Program
    {
        int exampleVar = 0;//实例成员
        static int staticVar = 0;//静态成员

        static void staticMethod()//静态方法
        {
          //  exampleVar = 1;   无法在静态方法中调用实例类成员
            staticVar = 1;
        }

        void exampleMethod()
        {
            //实例中可以调用任何程勇，包括静态与实例类的
            staticVar = 1;
            exampleVar = 1;
        }

        static void Main(string[] args)
        {
            progam
           

        }
    }
}

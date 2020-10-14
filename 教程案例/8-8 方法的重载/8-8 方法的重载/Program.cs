using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8_8_方法的重载
{
    class Program
    {
        //重载写在同一个类中
        //求元
        static void WriteAea(int radius)
        {
            double b = System.Math.PI * radius * radius;
            Console.WriteLine("你求的面积是：" + b);

        }
        //矩形
        static void WriteAea(int width, int length)
        {
            int a = width * length;
            Console.WriteLine("你求的矩形的面积是；" + a);

        }
        //三角形
        static void WriteAea(int q, int w, int e）
        {
            int p = (q + w + e) / 2;
            double area = System.Math.Sqrt(p * (p - q) * (p - w) * (p - e);
            Console.WriteLine("你求的矩形的面积是；" + area);

        }
        static void Main(string[] args)
        {
            //方法调用
            WriteAea（3,4,,5);
            WriteAea（25,24);
            WriteAea（10,5);
            Console.ReadKey();


        }
    }
}

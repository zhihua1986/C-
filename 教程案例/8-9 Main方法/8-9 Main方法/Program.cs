using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8_9_Main方法
{
    class Program
    {
        //常见的4中MAIN方法
       // 1.public static void main()
       //2. public static int main()
       // 3.public static void Main(string{[] args)
       // 4.public static int Main(string{[] args)
        static void Main(string[] args)
        {
            //参数领航数组的长度
            Console.WriteLine("{0}个命令行被指定",args.Length);
            foreach (string outstr in args) Console.WriteLine(outstr);
            Console.ReadKey();
        }
    }
}

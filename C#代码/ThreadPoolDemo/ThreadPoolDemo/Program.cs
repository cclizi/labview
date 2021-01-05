using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPoolDemo
{
    class Program
    {
       static double A = 1;
       static  double B =1;

        static void Main(string[] args)
        {
            int maxThreadNum, portThreadNum, minThreadNum;
            ThreadPool.GetMaxThreads(out maxThreadNum,out portThreadNum);
            ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);
            Console.WriteLine("最大线程数" + maxThreadNum);
            Console.WriteLine("最小线程数" + minThreadNum);
            int x = 1230;
            Console.WriteLine("启动第一个计算");
            ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc1), x);
            Console.WriteLine("启动第二个计算");
            ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc2), x);
            while (A == 1 || B == 1)
            {
              
               
            }
            Console.WriteLine("A:" + A + "B:" + B);
            Console.ReadLine();
        }
        static void TaskProc1(object i)
        {
            A = Math.Pow(Convert.ToDouble(i), 6);
        }
        static void TaskProc2(object J)
        {
            B = Math.Pow(Convert.ToDouble(J), 1.0/6.0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace testTgreadDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("主线程开始");
            Thread t = new Thread(new ThreadStart(ThreadMethod));
            t.Start(); //开启辅助线程
            Thread.Sleep(3000);//休眠3秒
            t.Resume(); //恢复被挂起线程
            for(char i = 'a'; i < 'i'; i++)
            {
                Console.WriteLine("主线程挂起");
                Thread.Sleep(1000);
            }
            t.Join();//辅助线程加入
            Console.WriteLine("主线程结束");
            Console.ReadLine();
        }
        static void ThreadMethod()
        {
            Console.WriteLine("辅助线程1");
            Thread.CurrentThread.Suspend(); // 挂起当前线程
            Console.WriteLine("辅助线程开启");
            for(int i = 1; i < 6; i++)
            {
                Console.WriteLine("辅助线程" + i);
                Thread.Sleep(2000);
            }
            Console.WriteLine("辅助线程结束");
        }
    }
}

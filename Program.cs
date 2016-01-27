using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/**
 *Mutex互斥锁Demo
 *两个线程竞争锁，一个做加法，另一个做减法 
 */

namespace BeginningCSharp
{
    class ShareRes
    {
        public static int count = 0;
        public static Mutex mutex = new Mutex();
    }

    class IncThread
    {
        int number;
        public Thread thread;

        public IncThread(string name, int n)
        {
            thread = new Thread(this.run);
            number = n;
            thread.Name = name;
            thread.Start();
        }

        void run()
        {
            Thread.Sleep(new Random().Next(1,3));
            Console.WriteLine(thread.Name + "正在等待 the mutex");
            ShareRes.mutex.WaitOne();
            Console.WriteLine(thread.Name + "申请到 the mutex");
            do
            {
                Thread.Sleep(100);
                ShareRes.count++;
                Console.WriteLine("In " + thread.Name + " ShareRes.count is " + ShareRes.count);
                number--;
            } while (number > 0);
            Console.WriteLine(thread.Name + " 释放 the mutex");
            ShareRes.mutex.ReleaseMutex();
        }
    }

    class DecThread
    {
        int number;
        public Thread thrd;
        public DecThread(string name, int n)
        {
            thrd = new Thread(this.run);
            number = n;
            thrd.Name = name;
            thrd.Start();
        }
        void run()
        {
            Console.WriteLine(thrd.Name + "正在等待 the mutex");
            //申请
            ShareRes.mutex.WaitOne();
            Console.WriteLine(thrd.Name + "申请到 the mutex");
            do
            {
                Thread.Sleep(100);
                ShareRes.count--;
                Console.WriteLine("In " + thrd.Name + "ShareRes.count is " + ShareRes.count);
                number--;
            } while (number > 0);
            Console.WriteLine(thrd.Name + "释放 the nmutex");
            // 释放
            ShareRes.mutex.ReleaseMutex();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IncThread t1 = new IncThread("IncThread thread",5);
            DecThread t2 = new DecThread("DecThread thread",5);
            t1.thread.Join();
            t2.thrd.Join();
            Console.ReadKey();
        }
    }
}

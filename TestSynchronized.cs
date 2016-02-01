using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestSynchronized
{
    class TestSynchronized
    {
        Thread thread1,thread2 = null;
        Mutex mutex = new Mutex();

        public TestSynchronized()
        {
            thread1 = new Thread(func1);
            thread1.Start();
            thread2 = new Thread(func2);
            thread2.Start();
            Console.ReadLine();
        }

        public void func1()
        {
            for (int i = 0; i < 10; i++)
            {
                lock (this)
                {
                    mutex.WaitOne();
                    print("thread1 run " + i + " times");
                    Thread.Sleep(200);
                    mutex.ReleaseMutex(); 
                }
            }
        }

        public void func2()
        {
            for (int i = 0; i < 10; i++)
            {
                lock (this)
                {
                    mutex.WaitOne();
                    print("thread2 run " + i + " times");
                    Thread.Sleep(100);
                    mutex.ReleaseMutex(); 
                }
            }
        }

        public void print(string str)
        {
            Console.WriteLine("{0} {1}",str,System.DateTime.Now.ToString()+" "+System.DateTime.Now.Millisecond.ToString());
            Thread.Sleep(50);
        }
        static void Main(string[] args)
        {
            new TestSynchronized();
        }
    }

}

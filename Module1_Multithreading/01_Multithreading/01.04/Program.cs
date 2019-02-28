using System;

namespace _04
{
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            CreateThread(10);
            Console.ReadKey();
        }

        public static void CreateThread(int i)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} thread is started. i = {i}");
            i--;
            if (i>0)
            {
                var thread = new Thread(()=>CreateThread(i));
                thread.Start();
                thread.Join();
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} joined {thread.ManagedThreadId}");
            }
        }
    }
}

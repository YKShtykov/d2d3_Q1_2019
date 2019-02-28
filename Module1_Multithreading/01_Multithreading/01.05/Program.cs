using System;

namespace _05
{
    using System.Threading;

    class Program
    {
        private static Semaphore _semaphore = new Semaphore(10,10);
        static void Main(string[] args)
        {
            CreateThread(10);
            Console.ReadKey();
        }

        public static void CreateThread(object state)
        {
            _semaphore.WaitOne();
            int i = Convert.ToInt32(state);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} thread is started. i = {i}");
            i--;
            if (i > 0)
            {
                ThreadPool.QueueUserWorkItem(CreateThread,i);
                Console.WriteLine($"Realize semaphore. Prev count:{_semaphore.Release()}");
            }
        }
    }
}

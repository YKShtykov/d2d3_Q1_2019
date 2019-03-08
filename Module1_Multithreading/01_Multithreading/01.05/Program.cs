using System;

namespace _05
{
    using System.Threading;

    class Program
    {
        private static readonly Semaphore Sph = new Semaphore(1, 1);

        static void Main()
        {
            CreateTaskRecursive(10);
            Console.ReadLine();
        }

        public static void CreateTaskRecursive(int number)
        {
            ThreadPool.QueueUserWorkItem(num =>
            {
                Sph.WaitOne();
                if (number <= 0) return;

                var intNum = (int)num;
                Console.WriteLine(intNum);

                Sph.Release();
                CreateTaskRecursive(--intNum);
            }, number);
        }
    }
}

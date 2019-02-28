namespace _06
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Program
    {
        private static readonly List<int> _list = new List<int>();
        private static readonly Semaphore _readSemaphore = new Semaphore(0, 1);
        private static readonly Semaphore _writeSemaphore = new Semaphore(1, 1);

        private static void Main(string[] args)
        {
            Task.Run(() =>
            {
                for (var i = 0; i < 10; i++)
                {
                    _writeSemaphore.WaitOne();
                    _list.Add(i);
                    Console.WriteLine($"{i} added to list");
                    _readSemaphore.Release();
                }
            });
            Task.Run(() =>
            {
                while (true)
                {
                    _readSemaphore.WaitOne();
                    Console.WriteLine(string.Join(',', _list));
                    _writeSemaphore.Release();
                }
            });

            Console.ReadKey();
        }
    }
}
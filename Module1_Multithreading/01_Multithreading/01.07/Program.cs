using System;
using System.Threading;

namespace _07
{
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => DoSuccessfulWorkWith50PercentChance())
                .ContinueWith((arg) =>
                {
                    Console.WriteLine("Task continued anyway");
                    DoFailedWork();
                }) //a
                .ContinueWith((arg) =>
                    {
                        Console.WriteLine("Task continued only if previous failed");
                        DoFailedWork();
                    },
                    TaskContinuationOptions.OnlyOnFaulted) //b
                .ContinueWith((arg) =>
                    {
                        Console.WriteLine("Task continued in the same thread and only if previous failed");
                        DoFailedWork();
                    }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted) //c
                .ContinueWith((arg) =>
                    {
                        Console.WriteLine("Task continued outside the threadpool if the previous one is canceled");
                        DoFailedWork();
                    }, TaskContinuationOptions.OnlyOnCanceled); //d

            Console.ReadKey();
        }

        private static void DoFailedWork()
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            throw new NotImplementedException();
        }

        private static void DoSuccessfulWorkWith50PercentChance()
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

            int rndNum = 0;
            var rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                rndNum = rnd.Next(0, 2);
            }

            if (rndNum == 0)
            {
                Console.WriteLine("Method completed successfuly");
            }
            else
            {
                Console.WriteLine("Method throwed exception");
                throw new Exception("Method failed");
            }
        }


    }
}

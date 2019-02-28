using System;

namespace _01
{
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            var taskArray = new Task[100];

            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((taskNum) =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        Console.WriteLine($"Task #{taskNum} – {j}");
                    }
                }, i);
            }

            Task.WaitAll(taskArray);
            Console.ReadKey();
        }
    }
}

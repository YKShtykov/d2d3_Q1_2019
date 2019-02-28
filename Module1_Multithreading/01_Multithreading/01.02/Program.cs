using System;

namespace _02
{
    using System.Linq;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                var array = new int[10];
                var rand = new Random();
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = rand.Next(0, 100);
                }
                Console.WriteLine($"Array: {string.Join(',',array)}");
                return array;

            })
            .ContinueWith((prevTask) =>
            {
                int[] array = prevTask.Result;
                var randMultiplier = new Random().Next(0, 100);
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = array[i] * randMultiplier;
                }
                Console.WriteLine($"Random multiplier: {randMultiplier}. Array: {string.Join(',', array)}");
                return array;
            })
            .ContinueWith(prevTask =>
            {
                var list = prevTask.Result.ToList();
                list.Sort();
                Console.WriteLine($"Sorted array: {string.Join(',', list)}");
                return list;

            })
            .ContinueWith((prevTask) =>
            {
                var list = prevTask.Result;
                Console.WriteLine($"Average: {list.Average()}");
            });

            Console.ReadKey();
        }
    }
}

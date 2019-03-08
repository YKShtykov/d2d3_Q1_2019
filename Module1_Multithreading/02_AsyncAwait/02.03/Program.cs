using System;

namespace _02._03
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        private static readonly Dictionary<string,decimal> _bin = new Dictionary<string, decimal>();
        private static decimal _totalprice = 0;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Write product name and price splited by space");
                var input = Console.ReadLine()?.Trim().Split(' ');
                if (string.IsNullOrEmpty(input?[0]) || !decimal.TryParse(input?[1], out decimal price))
                {
                    Console.WriteLine("incorrect input. try again");
                    continue;
                }

                AddProductToBinAsync(input[0], price);
            }
        }

        static async Task AddProductToBinAsync(string product, decimal price)
        {
            try
            {
                await Task.Run(() => AddProductToBin(product,price)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't add {product} with price {price}. "+ e.Message);
            }
        }

        private static void AddProductToBin(string product, decimal price)
        {
            if (price <0)
            {
                throw new Exception("Price cant be less than 0");
            }
            Console.WriteLine($"Adding {product} to bin");
            Thread.Sleep(10000);

            if (_bin.ContainsKey(product))
            {
                _bin[product] += price;
            }
            else
            {
                _bin.Add(product,price);
            }

            _totalprice = _bin.Values.Sum();
            Console.WriteLine($"{product} was added to bin. Total price is {_totalprice}");
        }
    }
}

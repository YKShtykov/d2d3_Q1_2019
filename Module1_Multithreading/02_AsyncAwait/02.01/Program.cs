using System;

namespace _02._01
{
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {

        private static CancellationTokenSource _tokenSource;
        static void Main(string[] args)
        {
            while (true)
            {
                var inputString = Console.ReadLine();

                if (!int.TryParse(inputString, out int n) || n < 0)
                {
                    Console.WriteLine("Incorrect N");
                    continue;
                }
                _tokenSource?.Cancel();
                _tokenSource?.Dispose();
                _tokenSource = new CancellationTokenSource();
                Task.Run(() => CalculateSum(n, _tokenSource.Token));
            }
        }

        private static void CalculateSum(int n, CancellationToken token)
        {
            try
            {
                int sum = 0;
                for (int i = 0; i < Convert.ToInt32(n); i++)
                {
                    sum += i;
                    Thread.Sleep(100);
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                }

                Console.WriteLine(sum);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

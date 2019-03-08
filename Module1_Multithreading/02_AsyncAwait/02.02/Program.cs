namespace _02._02
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Program
    {
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine(
                    "Enter page address. Separate multiple Uri by ';'. To abort previous loading - press c");
                var addresses = Console.ReadLine();
                if (addresses.Equals("c", StringComparison.InvariantCultureIgnoreCase))
                {
                    AbortPagesLoading();
                    continue;
                }

                foreach (var address in addresses.Split(';')) LoadPageAsync(address, _cancellationTokenSource.Token);
            }
        }

        private static async Task LoadPageAsync(string address, CancellationToken token)
        {
            try
            {
                await Task.Run(() => LoadPage(address, token), token);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"{address} page loading canceled");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{address} -"+e.Message);
            }
        }

        private static void LoadPage(string address, CancellationToken token)
        {
            string page;
            if (!Uri.TryCreate(address, UriKind.Absolute,out Uri resultUri))
            {
                throw  new ArgumentException("Invalid address");
            }
            var request = (HttpWebRequest) WebRequest.Create(resultUri);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (var response = (HttpWebResponse) request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                page = reader.ReadToEnd();
            }

            Thread.Sleep(10000);
            if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();

            Console.WriteLine(page);
        }

        private static void AbortPagesLoading()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}
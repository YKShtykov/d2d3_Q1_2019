using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace TestCU
{
    class Program
    {
        class KeyChecker
        {
            public byte[] a;
            public int[] b;



            public int DoCheck(byte a0, int a1)
            {
                return a0 ^ a[a1];
            }

            public int DoCheck(int a0, int a1)
            {
                return a0;
            }
        }

        static void Main(string[] args)
        {
            NetworkInterface nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
            var addressBytes = nic.GetPhysicalAddress().GetAddressBytes();
            var kc = new KeyChecker();
            kc.a = BitConverter.GetBytes(DateTime.Now.Date.ToBinary());

            int i = 0;
            Func<byte, int, int> selector1 = new Func<byte, int, int>(kc.DoCheck);
            var source = addressBytes.Select(a => selector1(a, i++)).Select(n => n < 999 ? n * 10 : n);

            int j = 0;

            //kc.b = Console.ReadLine().Split('-').Select(n => int.Parse(n)).ToArray();
            Func<int, int, int> selector2 = new Func<int, int, int>(kc.DoCheck);
            var resultNums = source.Select(n => selector2(n, j++)).ToArray();
            Console.WriteLine($"Your key is {string.Join("-", resultNums)}");
            Console.ReadKey();
        }
    }
}


using System;
using System.IO;
using System.Linq;
using FileProvider;

namespace CUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = string.Empty;

            //while (true)
            //{
            //    Console.WriteLine("Write the root path");
            //    path = Console.ReadLine();
            //    if (!Directory.Exists(path))
            //    {
            //        Console.WriteLine($"{path} directory does not exist");
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
            path = @"C:\Users\Yakov_Shtykov\Desktop\CertificateTool";
            var fileQueryable = new FileQueryable(path);

            var files = fileQueryable.Where(f => f.Name.Contains("Program")&& f.Length<1000).Select(f=>f.Name).ToList();

            Console.ReadKey();
        }
    }
}

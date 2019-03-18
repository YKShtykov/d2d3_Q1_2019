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

            while (true)
            {
                Console.WriteLine("Write the root path");
                path = Console.ReadLine();
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"{path} directory does not exist");
                }
                else
                {
                    break;
                }
            }

            var fileInfos = new FileInfoSet<FileInfo>(path);

            var lenghts = fileInfos.Where(f => f.Name == "1.txt" && f.IsReadOnly).Select(f => f.Length).GetEnumerator();

            Console.ReadKey();
        }
    }
}

namespace _03
{
    using System;
    using System.Diagnostics;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var a = new Matrix(2000, 30, true);
            var b = new Matrix(30, 2000, true);

            var sw = Stopwatch.StartNew();
            var c = Matrix.Multiply(a, b);
            var time = sw.ElapsedTicks;

            sw = Stopwatch.StartNew();
            var d = Matrix.MultiplyByTpl(a, b);
            var tplTime = sw.ElapsedTicks;

            //Console.WriteLine($"Matrix A:{a}Matrix B:{b}Result:{c}Result:{d}");
            Console.WriteLine($" Multiply time {time}.  Multiply by TPL time {tplTime}");
            Console.ReadKey();
        }
    }
}
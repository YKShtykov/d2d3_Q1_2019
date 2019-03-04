using System;

namespace _07
{
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(DoSomeWork)
                .ContinueWith((arg) => { })
                .ContinueWith((arg) => DoSomeWork(),
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.NotOnCanceled)
                .ContinueWith((arg) => DoSomeWork(),
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.RunContinuationsAsynchronously);

        }

        private static object DoOtherWork()
        {
            throw new NotImplementedException();
        }

        private static Task DoSomeWork()
        {
            throw new NotImplementedException();
        }
    }
}

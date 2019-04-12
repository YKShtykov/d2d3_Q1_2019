namespace CentralServer
{
    using Topshelf;

    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(
                conf => conf.Service<CentralServer>(
                    s =>
                    {
                        s.ConstructUsing(() => new CentralServer());
                        s.WhenStarted(serv => serv.Start());
                        s.WhenStopped(serv => serv.Stop());
                    }
                )
            );
        }
    }
}
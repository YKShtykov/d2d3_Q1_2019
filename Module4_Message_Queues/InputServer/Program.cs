namespace InputServer
{
    using System.Diagnostics;
    using System.IO;
    using NLog;
    using NLog.Config;
    using NLog.Targets;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var logFactory = ConfigureLogFactory(folder);

            var server = new InputServer();
            server.Start();
            //HostFactory.Run(
            //    conf => conf.Service<InputServer>(
            //        s =>
            //        {
            //            s.ConstructUsing(() => new InputServer());
            //            s.WhenStarted(serv => serv.Start());
            //            s.WhenStopped(serv => serv.Stop());
            //        }
            //    ).UseNLog(logFactory)
            //);
        }


        private static LogFactory ConfigureLogFactory(string folder)
        {
            var logConf = new LoggingConfiguration();
            var fileTarget = new FileTarget
            {
                FileName = Path.Combine(folder, "log.txt"),
                CreateDirs = true,
                Name = "TargetLog",
                Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
            };
            logConf.AddTarget(fileTarget);
            logConf.AddRuleForAllLevels(fileTarget);

            return new LogFactory(logConf);
        }
    }
}
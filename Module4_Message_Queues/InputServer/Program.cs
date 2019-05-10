namespace InputServer
{
    using Autofac;
    using Autofac.Extras.DynamicProxy;
    using Common;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = ConfigureBuilder();
            var server = container.Resolve<IServer>();
            server.Start();
        }


        private static IContainer ConfigureBuilder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<InputServer>()
                .As<IServer>()
                .EnableInterfaceInterceptors();
            builder.Register(c => new CallLogger());
            return builder.Build();
        }
    }
}
namespace Common
{
    using System.IO;
    using System.Linq;
    using Castle.DynamicProxy;
    using NLog;

    public class CallLogger : IInterceptor
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void Intercept(IInvocation invocation)
        {
            logger.Info("Calling method {0} with parameters {1}... ",
                invocation.Method.Name,
                string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray()));

            invocation.Proceed();

            logger.Info("Done: result was {0}.", invocation.ReturnValue);
        }
    }
}
namespace Common
{
    using System;
    using System.Linq;
    using NLog;
    using PostSharp.Aspects;

    [Serializable]
    public class LoggingPostSharpAspect : OnMethodBoundaryAspect
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override void OnEntry(MethodExecutionArgs args)
        {
            logger.Info("Calling method {0} with parameters {1}... ",
                args.Method.Name,
                string.Join(", ", args.Arguments.Select(a => (a ?? "").ToString()).ToArray()));

            args.FlowBehavior = FlowBehavior.Default;
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            logger.Info("Done: result was {0}.", args.ReturnValue);
        }
    }
}
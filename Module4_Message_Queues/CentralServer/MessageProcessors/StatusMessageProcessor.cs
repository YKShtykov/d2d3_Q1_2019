namespace CentralServer.MessageProcessors
{
    using System;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Newtonsoft.Json;
    using NLog;

    internal class StatusMessageProcessor : IMessageProcessor
    {
        private readonly object _locker = new object();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IQueueClient _queueClient;
        private readonly string _settingsPath;

        public StatusMessageProcessor(QueueClient queueClient)
        {
            _queueClient = queueClient;
            _settingsPath = AppDomain.CurrentDomain.BaseDirectory + "cms\\statuses\\statuses.xlsx";
        }

        public void StartProcessing()
        {
            RegisterOnMessageHandlerAndReceiveMessages();
        }

        public void CancelProcessing()
        {
            _queueClient.CloseAsync();
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            try
            {
                var messageBody = message.Body;
                var statusObject = JsonConvert.DeserializeObject(
                    Encoding.UTF8.GetString(messageBody), typeof(ServerStatus));

                var status = (ServerStatus) statusObject;

                SaveServerStatus(status);
                Console.WriteLine(
                    $"{status.Address}: {status.Code} - {status.Description} - {message.SystemProperties.EnqueuedTimeUtc}");
            }
            catch (SerializationException e)
            {
                throw new Exception("Message corrupted.", e);
            }

            if (token.IsCancellationRequested)
                await _queueClient.AbandonAsync(message.SystemProperties.LockToken);
            else
                await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private void SaveServerStatus(ServerStatus status)
        {
            _logger.Info(JsonConvert.SerializeObject(status));
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.Error(exceptionReceivedEventArgs.Exception,
                $"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}." +
                $"Exception message: {exceptionReceivedEventArgs.Exception.Message}." +
                $" Inner exception: {exceptionReceivedEventArgs.Exception.InnerException}." +
                $" Stack trace: {exceptionReceivedEventArgs.Exception.StackTrace}");

            return Task.CompletedTask;
        }
    }
}
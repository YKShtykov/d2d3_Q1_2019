namespace InputServer.MessageProcessors
{
    using System.IO;
    using System.Text;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;
    using Newtonsoft.Json;

    internal class StatusMessageSender
    {
        private readonly QueueClient _queueClient;

        public StatusMessageSender()
        {
            var connectionString =
                CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists("statusqueue"))
                namespaceManager.CreateQueue("statusqueue");

            _queueClient = QueueClient.Create("statusqueue");
        }

        public async void SendMessage(ServerStatus status)
        {
            var serializedStatus = JsonConvert.SerializeObject(status);
            var bytesStatus = Encoding.UTF8.GetBytes(serializedStatus);

            using (var subMessageStream = new MemoryStream(bytesStatus))
            {
                var message = new BrokeredMessage(subMessageStream, false);
                await _queueClient.SendAsync(message);
            }
        }
    }
}
namespace CentralServer
{
    using MessageProcessors;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.ServiceBus;
    using Microsoft.WindowsAzure;

    internal class CentralServer
    {
        private readonly string _connectionString;
        private IMessageProcessor _fileMessageProcessor;
        private IMessageProcessor _settingsMessageProcessor;
        private IMessageProcessor _statusMessageProcessor;

        public CentralServer()
        {
            _connectionString =
                CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);

            if (!namespaceManager.QueueExists("pdffilequeue"))
                namespaceManager.CreateQueue("pdffilequeue");

            if (!namespaceManager.QueueExists("statusqueue"))
                namespaceManager.CreateQueue("statusqueue");

            if (!namespaceManager.TopicExists("settingtopic"))
                namespaceManager.CreateTopic("settingtopic");
        }


        public void StartFileMessageProcessing()
        {
            var queueClient = new QueueClient(_connectionString, "pdffilequeue");
            _fileMessageProcessor = new FileMessageProcessor(queueClient);

            _fileMessageProcessor.StartProcessing();
        }

        public void StartStatusMessageProcessing()
        {
            var queueClient = new QueueClient(_connectionString, "statusqueue");
            _statusMessageProcessor = new StatusMessageProcessor(queueClient);

            _statusMessageProcessor.StartProcessing();
        }

        public void StartSettingsMessageProcessing()
        {
            var topicClient = new TopicClient(_connectionString, "settingtopic");
            _settingsMessageProcessor = new SettingsProcessor(topicClient);

            _settingsMessageProcessor.StartProcessing();
        }

        public void StopFileMessageProcessing()
        {
            _fileMessageProcessor.CancelProcessing();
        }

        public void StopStatusMessageProcessing()
        {
            _statusMessageProcessor.CancelProcessing();
        }

        public void StopSettingsMessageProcessing()
        {
            _settingsMessageProcessor.CancelProcessing();
        }

        public void Start()
        {
            StartFileMessageProcessing();
            StartStatusMessageProcessing();
            StartSettingsMessageProcessing();
        }

        public void Stop()
        {
            StopFileMessageProcessing();
            StopStatusMessageProcessing();
            StopSettingsMessageProcessing();
        }
    }
}
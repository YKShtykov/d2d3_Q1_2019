namespace CentralServer
{
    using Common;
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

        [LoggingPostSharpAspect]
        public void StartFileMessageProcessing()
        {
            var queueClient = new QueueClient(_connectionString, "pdffilequeue");
            _fileMessageProcessor = new FileMessageProcessor(queueClient);

            _fileMessageProcessor.StartProcessing();
        }
        [LoggingPostSharpAspect]
        public void StartStatusMessageProcessing()
        {
            var queueClient = new QueueClient(_connectionString, "statusqueue");
            _statusMessageProcessor = new StatusMessageProcessor(queueClient);

            _statusMessageProcessor.StartProcessing();
        }
        [LoggingPostSharpAspect]
        public void StartSettingsMessageProcessing()
        {
            var topicClient = new TopicClient(_connectionString, "settingtopic");
            _settingsMessageProcessor = new SettingsProcessor(topicClient);

            _settingsMessageProcessor.StartProcessing();
        }
        [LoggingPostSharpAspect]
        public void StopFileMessageProcessing()
        {
            _fileMessageProcessor.CancelProcessing();
        }
        [LoggingPostSharpAspect]
        public void StopStatusMessageProcessing()
        {
            _statusMessageProcessor.CancelProcessing();
        }
        [LoggingPostSharpAspect]
        public void StopSettingsMessageProcessing()
        {
            _settingsMessageProcessor.CancelProcessing();
        }

        [LoggingPostSharpAspect]
        public void Start()
        {
            StartFileMessageProcessing();
            StartStatusMessageProcessing();
            StartSettingsMessageProcessing();
        }
        [LoggingPostSharpAspect]
        public void Stop()
        {
            StopFileMessageProcessing();
            StopStatusMessageProcessing();
            StopSettingsMessageProcessing();
        }
    }
}
﻿namespace InputServer.MessageProcessors
{
    using System;
    using System.IO;
    using System.Transactions;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;

    internal class FileMessageSender : IMessageSender
    {
        private readonly int _messageBodySize = 192 * 1024;
        private readonly QueueClient _queueClient;

        public FileMessageSender()
        {
            var connectionString =
                CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists("pdffilequeue"))
                namespaceManager.CreateQueue("pdffilequeue");

            _queueClient = QueueClient.Create("pdffilequeue");
        }

        public async void SendMessage(Stream stream)
        {
            var positionNumber = 1;
            var streamSize = stream.Length;
            var sequence = Guid.NewGuid().ToString();
            var numberSubMessages = CountChunks(streamSize, _messageBodySize);

            if (streamSize <= _messageBodySize)
            {
                var subMessage = CreateMessage(stream, sequence, numberSubMessages, numberSubMessages);
                _queueClient.Send(subMessage);
                return;
            }

            for (var sOffset = 0; sOffset < streamSize; sOffset += _messageBodySize)
            {
                long bufferSize;

                if (streamSize - sOffset > _messageBodySize)
                    bufferSize = _messageBodySize;
                else
                    bufferSize = streamSize - sOffset;

                var buffer = new byte[bufferSize];
                stream.Read(buffer, 0, (int) bufferSize);

                using (var trScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var subMessageStream = new MemoryStream(buffer))
                    {
                        var subMessage = CreateMessage(subMessageStream, sequence, positionNumber, numberSubMessages);
                        positionNumber++;

                        await _queueClient.SendAsync(subMessage);
                    }

                    trScope.Complete();
                }
            }
        }

        private BrokeredMessage CreateMessage(Stream stream, string sequence, int position, int size)
        {
            var message = new BrokeredMessage(stream, true);

            message.Properties.Add("Sequence", sequence);
            message.Properties.Add("Position", position);
            message.Properties.Add("Size", size);

            return message;
        }

        private static int CountChunks(long streamSize, int messageBodySize)
        {
            var numberSubMessages = (int) streamSize / messageBodySize;

            if (streamSize % messageBodySize != 0)
                numberSubMessages++;

            return numberSubMessages;
        }
    }
}
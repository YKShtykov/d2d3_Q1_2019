namespace CentralServer.MessageProcessors
{
    internal interface IMessageProcessor
    {
        void StartProcessing();
        void CancelProcessing();
    }
}
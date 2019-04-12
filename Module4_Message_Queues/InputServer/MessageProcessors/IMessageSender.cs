namespace InputServer.MessageProcessors
{
    using System.IO;

    internal interface IMessageSender
    {
        void SendMessage(Stream stream);
    }
}
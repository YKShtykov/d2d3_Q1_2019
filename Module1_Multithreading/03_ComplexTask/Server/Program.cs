using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";
            var server = new Server();
        }

        private class Server
        {
            private const int _bufferSize = 1024;
            private List<Message> _messages = new List<Message>();
            public Server()
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(10);

                    while (true)
                    {
                        Socket handler = listener.Accept();
                        HandleConnection(handler);
                        handler.Send(Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(_messages)));
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            private void HandleConnection(Socket handler)
            {
                var buffer = new byte[_bufferSize];
                var sb = new StringBuilder();

                while (handler.Available>0)
                {
                    var bytes = handler.Receive(buffer);
                    sb.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                }

                if (string.IsNullOrEmpty(sb.ToString()))
                {
                    return;
                }
                var message = JsonConvert.DeserializeObject<Message>(sb.ToString());
                Console.WriteLine(message);
                _messages.Add(message);
            } 
        }
    }
}

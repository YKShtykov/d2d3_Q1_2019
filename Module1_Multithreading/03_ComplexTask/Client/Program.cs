using System;

namespace Client
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    class Program
    {

        static void Main(string[] args)
        {
            Console.Title = "Client";
            var client = new Client("client1");
            Console.ReadLine();
        }

        private class Client
        {
            private const int _bufferSize = 10240;
            private List<Message> _messages = new List<Message>();
            private string _name;

            public Client(string name)
            {
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
                        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        _name = name;
                        listener.Connect(localEndPoint);
                        SendMessage(listener);
                        ListenToServer(listener);
                        listener.Shutdown(SocketShutdown.Both);
                        listener.Close(); 
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            private void SendMessage(Socket listener)
            {
                var message = new Message
                {
                    Author = _name,
                    Text = new Random().Next(0, 1000).ToString(),
                    Time = DateTime.Now
                };
                Console.WriteLine($"-> {message}");
                listener.Send(Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(message)));
            }

            private void ListenToServer(Socket handler)
            {
                var buffer = new byte[_bufferSize];
                var sb = new StringBuilder();

                while (handler.Available > 0)
                {
                    var bytes = handler.Receive(buffer);
                    sb.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                }

                Console.WriteLine(sb);
                //if (string.IsNullOrEmpty(sb.ToString()))
                //{
                //    return;
                //}

                //var messages = JsonConvert.DeserializeObject<List<Message>>(sb.ToString());
                //_messages.AddRange(messages.Except(_messages));
                //_messages = _messages.OrderBy(m => m.Time).ToList();
            }
        }
    }
}

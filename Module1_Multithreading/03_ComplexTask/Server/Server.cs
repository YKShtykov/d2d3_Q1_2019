using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientServer.Common;
using Newtonsoft.Json;

namespace Server
{
    class Server
    {
        private readonly List<Message> _messages = new List<Message>();
        private readonly NamedPipeServerStream _server;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        public Server()
        {
            _server = new NamedPipeServerStream("SomePipe");
            _reader = new StreamReader(_server);
            _writer = new StreamWriter(_server);
        }

        public async Task ListenToClient()
        {
            await Task.Run(() =>
            {
                Console.WriteLine("Wait for client");
                _server.WaitForConnection();
                Console.WriteLine($"Client connected");

                var line = _reader.ReadLine();
                Console.WriteLine($"->{line}");
                var message = JsonConvert.DeserializeObject<Message>(line);
                _messages.Add(message);
                Console.WriteLine($"<-{JsonConvert.SerializeObject(_messages)}");
                _writer.WriteLine(JsonConvert.SerializeObject(_messages));
                _writer.Flush();

                _server.Disconnect();
            });
        }
    }
}

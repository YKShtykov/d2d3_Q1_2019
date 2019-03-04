using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using ClientServer.Common;
using Newtonsoft.Json;

namespace Client
{
    class Client
    {
        private string _name;
        private StreamReader _reader;
        private StreamWriter _writer;
        private List<Message> _messages = new List<Message>();
        private NamedPipeClientStream _client;

        public Client(string name)
        {
            _name = name;
        }

        public void SendMessageToServer()
        {
            _client = new NamedPipeClientStream("SomePipe");
            _reader = new StreamReader(_client);
            _writer = new StreamWriter(_client);
            _client.Connect();
            for (int i = 0; i < 5; i++)
            {
                var message = new Message
                {
                    Author = _name,
                    Text = "Message" + new Random().Next(0, 100),
                    Time = DateTime.Now
                };
                _writer.WriteLine(JsonConvert.SerializeObject(message));
                _writer.Flush();
                var messages = JsonConvert.DeserializeObject<List<Message>>(_reader.ReadLine());
                _messages.AddRange(messages);
                _messages = _messages.Distinct().ToList();
                Console.Clear();
                foreach (var item in _messages)
                {
                    Console.WriteLine(item);
                } 
            }
            _client.Close();
        }
    }
}

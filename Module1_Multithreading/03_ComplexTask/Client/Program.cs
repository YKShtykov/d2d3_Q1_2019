using System;
using System.IO;
using System.IO.Pipes;
using ClientServer.Common;

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
        private static string _name;

        static void Main(string[] args)
        {

            Console.Title = "Client";
            var client = new Client("Client"+ new Random().Next(0,100));
            while (true)
            {
                client.SendMessageToServer();
            }
        }
    }
}

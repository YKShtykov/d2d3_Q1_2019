using System;
using System.IO;
using System.IO.Pipes;
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

            server.ListenToClient();
            Console.ReadKey();
        }
    }
}

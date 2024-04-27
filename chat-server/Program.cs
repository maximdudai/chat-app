using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using chat_server.connection;
using chat_server.models;
using EI.SI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pqc.Crypto.Utilities;

namespace chat_server
{
    internal class Program
    {
        private const int PORT = 5555;
        static async Task Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            TcpListener listener = new TcpListener(endPoint);
            listener.Start();

            Console.WriteLine("Server started on port " + PORT);
            Console.WriteLine("Waiting for clients...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                ClientCount clientCount = new ClientCount();

                clientCount.Increment();

                ClientHandler clientHandler = new ClientHandler(client, clientCount.GetCount());

                Task task = Task.Run(() => clientHandler.Handle());
            }
        }
    }
}

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

        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            ClientCount clientCount = new ClientCount();

            TcpListener listener = new TcpListener(endPoint);
            listener.Start();

            Console.WriteLine("Server started on port " + PORT);
            Console.WriteLine("Waiting for clients...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();


                clientCount.Increment();
                ClientHandler clientHandler = new ClientHandler(client, clientCount.GetCount());
                Console.WriteLine("Client " + clientCount.GetCount() + " connected");

                // test new comment

                clientHandler.Handle();
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using chat_server.models;

namespace chat_server
{
    internal class Program
    {
        private const int PORT = 5555;
        private static readonly ConcurrentDictionary<int, TcpClient> ConnectedClients = new ConcurrentDictionary<int, TcpClient>();
        private static int ClientIdCounter = 0;

        static async Task Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);
            TcpListener listener = new TcpListener(endPoint);
            listener.Start();

            Console.WriteLine("Server started on port " + PORT);
            Console.WriteLine("Waiting for clients...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                int clientId = ClientIdCounter++;

                if (ConnectedClients.TryAdd(clientId, client))
                {
                    ClientHandler clientHandler = new ClientHandler(client, clientId, ConnectedClients);
                    Task task = Task.Run(() => clientHandler.Handle());
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace chat_server
{
    internal class Program
    {
        private const int PORT = 5555;

        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);
            
            TcpListener listener = new TcpListener(endPoint);
            listener.Start();

            Console.WriteLine("Server started on port " + PORT);

            int clientCount = 0;

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clientCount++;
                Console.WriteLine("Client " + clientCount + " connected");

            }
        }
    }
}

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
            
            TcpListener listener = new TcpListener(endPoint);
            listener.Start();

            Console.WriteLine("Server started on port " + PORT);
            Console.WriteLine("Waiting for clients...");
            int clientCount = 0;


            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clientCount++;

                ClientHandler clientHandler = new ClientHandler(client, clientCount);
                Console.WriteLine("Client " + clientCount + " connected");

                clientHandler.Handle();

            }
        }
    }
    class ClientHandler
    {
        private TcpClient client;
        private int id;
        private string username;
        private string password;

        public ClientHandler(TcpClient client, int id)
        {
            this.client = client;
            this.id = id;
        }

        public void Handle()
        {
            Task.Run(async () => await HandleClient());
        }

        private async Task HandleClient()
        {
            NetworkStream networkStream = client.GetStream();
            ProtocolSI protocolSI = new ProtocolSI();

            try
            {
                while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
                {
                    Database database = new Database(); // Make sure Database methods are async as well

                    // Asynchronously read from the network stream
                    int bytesRead = await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (bytesRead == 0)
                        break; // If 0 bytes read, the client has disconnected

                    byte[] ack;

                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.DATA:
                            string data = Encoding.UTF8.GetString(protocolSI.GetData());
                            string[] dataSplit = data.Split(':');

                            username = dataSplit[1];
                            password = dataSplit[2];

                            switch (dataSplit[0])
                            {
                                case "login":
                                    // Replace with an async version of the database login method
                                    bool loginSuccess = await database.LoginAsync(username, password);

                                    ack = protocolSI.Make(ProtocolSICmdType.ACK, Encoding.UTF8.GetBytes(loginSuccess ? "success" : "fail"));
                                    await networkStream.WriteAsync(ack, 0, ack.Length);
                                    break;
                                case "register":
                                    // Replace with an async version of the database register method
                                    bool registerSuccess = await database.RegisterAsync(username, password, "");

                                    ack = protocolSI.Make(ProtocolSICmdType.ACK, Encoding.UTF8.GetBytes(registerSuccess ? "success" : "fail"));
                                    await networkStream.WriteAsync(ack, 0, ack.Length);
                                    break;
                            }
                            break;

                        case ProtocolSICmdType.EOT:
                            Console.WriteLine("[SERVER]: Client " + id + " disconnected");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERVER]: An error occurred with client {id}: {ex.Message}");
            }
            finally
            {
                networkStream.Close();
                client.Close();
            }
        }

    }
}

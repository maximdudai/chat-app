using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using chat_server.connection;
using EI.SI;

namespace chat_server.models
{

    internal class ClientHandler
    {
        private TcpClient client;

        private int id;
        private string username;
        private string password;
        private string command;

        public ClientHandler(TcpClient client, int id)
        {
            this.client = client;
            this.id = id;
        }

        public void Handle()
        {
            // Run the HandleClient method asynchronously
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

                            // Extract the username and password from the data packet
                            string data = Encoding.UTF8.GetString(protocolSI.GetData());

                            // Split the data into an array to separate the command, username, and password
                            string[] dataSplit = data.Split(':');

                            command = dataSplit[0];
                            username = dataSplit[1];
                            password = dataSplit[2];

                            switch (command)
                            {
                                case "login":
                                    // Replace with an async version of the database login method
                                    bool loginSuccess = await database.LoginAsync(username, password);

                                    ack = protocolSI.Make(ProtocolSICmdType.ACK, Encoding.UTF8.GetBytes(loginSuccess ? "success" : "fail"));
                                    Console.WriteLine("[SERVER]: " + username + " authentication: " + (loginSuccess ? "success" : "fail"));

                                    await networkStream.WriteAsync(ack, 0, ack.Length);
                                    break;
                                case "register":
                                    // Replace with an async version of the database register method
                                    bool registerSuccess = await database.RegisterAsync(username, password, "");

                                    ack = protocolSI.Make(ProtocolSICmdType.ACK, Encoding.UTF8.GetBytes(registerSuccess ? "success" : "fail"));

                                    Console.WriteLine("[SERVER]: " + username + " registration: " + (registerSuccess ? "success" : "fail"));

                                    await networkStream.WriteAsync(ack, 0, ack.Length);
                                    break;
                            }

                            break;

                        case ProtocolSICmdType.EOT:
                            ClientCount clientCount = new ClientCount();
                            clientCount.Decrement();

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

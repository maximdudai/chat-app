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
        private string email;

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
            ClientCount clientCount = new ClientCount();

            try
            {
                while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
                {
                    int bytesRead = await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (bytesRead == 0)
                        break;

                    // Extract the username and password from the data packet
                    string data = Encoding.UTF8.GetString(protocolSI.GetData());

                    byte[] ack;

                    if (data.Length == 0)
                        return;

                    // Split the data into an array to separate the command, username, and password
                    string[] dataSplit = data.Split(':');

                    // Get the command from the data packet
                    command = dataSplit[0];
                    this.username = dataSplit[1];

                    Database database = new Database(); // Make sure Database methods are async as 

                    switch (command)
                    {
                        case "chat":
                            this.id = int.Parse(dataSplit[2]);
                            string message = dataSplit[3];

                            // Send the message to the database
                            await database.InsertChatLog(id, this.username, message);
                            break;
                        case "login":

                            this.password = dataSplit[2];
                            // Verify if user exists in the database
                            // if method return is not null, user exists
                            var userID = await database.LoginAsync(username, password);

                            // Convert the ID to a byte array
                            byte[] user_id = userID.HasValue ? Encoding.UTF8.GetBytes(userID.Value.ToString()) : Encoding.UTF8.GetBytes("fail");

                            // Send the ID back to the client
                            ack = protocolSI.Make(ProtocolSICmdType.ACK, user_id);
                            await networkStream.WriteAsync(ack, 0, ack.Length);

                            Console.WriteLine("[SERVER]: " + username + " authentication: " + (userID != null ? "success" : "fail"));
                            break;
                        case "register":
                            try
                            {
                                this.password = dataSplit[2];
                                //Check if the email is present in the data
                                if (dataSplit.Length > 3)
                                    email = dataSplit[3];
                                // Execute the register method in the database
                                // if method return is not null, user already exists
                                var registerID = await database.RegisterAsync(username, password, email);

                                // Verify if the registerID is not null
                                byte[] register_id = registerID.HasValue ? Encoding.UTF8.GetBytes(registerID.Value.ToString()) : Encoding.UTF8.GetBytes("fail");

                                // Send the ID back to the client
                                ack = protocolSI.Make(ProtocolSICmdType.ACK, register_id);
                                await networkStream.WriteAsync(ack, 0, ack.Length);

                                Console.WriteLine("[SERVER]: " + username + " authentication: " + (registerID != null ? "success" : "fail"));

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("[SERVER]: Error in sending registration ID - " + ex.Message);
                            }
                            break;

                        default:
                            Console.WriteLine("[SERVER]: Invalid command received");
                            break;
                            // END OF COMMAND SWITCH
                    }
                    break;
                }

                if(protocolSI.GetCmdType() == ProtocolSICmdType.EOT)
                {
                    clientCount.Decrement();
                    Console.WriteLine("[SERVER]: Client " + id + " disconnected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERVER]: An error occurred with client {id}: {ex.Message}");
            }
            finally
            {
                networkStream?.Close();
                client?.Close();
            }
        }
    }
}

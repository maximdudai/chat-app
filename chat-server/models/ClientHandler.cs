using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using chat_log;
using chat_server.connection;
using chat_server.models;
using EI.SI;
using Org.BouncyCastle.Tls;

namespace chat_server
{
    public class ClientHandler
    {
        private readonly TcpClient client;
        private readonly ConcurrentDictionary<int, TcpClient> connectedClients;
        
        private int id;
        private string username;
        private string password;
        private string email;

        private RSACryptoServiceProvider rsaServer;
        private AesCryptoServiceProvider aesServer;

        private byte[] privateKey;
        private byte[] privateIV;

        public ClientHandler(TcpClient client, int id, ConcurrentDictionary<int, TcpClient> connectedClients)
        {
            this.client = client;
            this.id = id;
            this.connectedClients = connectedClients;
        }

        public void Handle()
        {
            // Run the HandleClient method asynchronously
            Task.Run(async () => await HandleClient());
        }

        public async Task HandleClient()
        {
            using (NetworkStream networkStream = client.GetStream())
            {
                ProtocolSI protocolSI = new ProtocolSI();
                try
                {
                    while (true)
                    {
                        int bytesRead = await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                        // If the client disconnected, close the network stream and client
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        // Set the buffer to the protocolSI object
                        protocolSI.SetBuffer(protocolSI.Buffer);

                        // Log the received command type for debugging
                        var cmdType = protocolSI.GetCmdType();
                        Console.WriteLine("[SERVER]: Command type received: " + cmdType);

                        string data = Encoding.UTF8.GetString(protocolSI.GetData());
                        string[] dataSplit = data.Split(':');

                        Console.WriteLine("[SERVER]: Data received: " + data);

                        switch (cmdType)
                        {
                            case ProtocolSICmdType.DATA:
                                // Ensure the data is in the correct format
                                if (dataSplit.Length < 2)
                                    continue;

                                string command = dataSplit[0];
                                Console.WriteLine("[SERVER]: Command received: " + command);

                                switch (command)
                                {
                                    case "chat":
                                        await this.ChatCommand(data);
                                        break;

                                    case "login":
                                        await this.LoginCommand(data, networkStream); // Pass the networkStream
                                        break;

                                    case "register":
                                        await this.RegisterCommand(data, networkStream);
                                        break;

                                    default:
                                        Console.WriteLine("[SERVER]: Invalid command received");
                                        break;
                                }
                                break;

                            case ProtocolSICmdType.PUBLIC_KEY:
                                // receive the public key from the client
                                await this.ReceivePublicKey(protocolSI);
                                break;

                            case ProtocolSICmdType.SECRET_KEY:
                                this.ReceiveSecretKey(protocolSI.GetData());
                                break;

                            case ProtocolSICmdType.IV:
                                this.ReceiveIV(protocolSI.GetData());
                                break;

                            case ProtocolSICmdType.EOT:
                                Console.WriteLine("[SERVER]: Client " + id + " disconnected (EOT)");
                                break;

                            default:
                                Console.WriteLine("[SERVER]: Invalid command received");
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
                    // Ensure client is removed from connectedClients
                    connectedClients.TryRemove(id, out _);
                    networkStream?.Close();
                    client?.Close();
                }
            }
        }

        private async Task ChatCommand(string data)
        {
            try
            {
                string[] dataSplit = data.Split(':');

                if (dataSplit.Length < 4) return; // Ensure all parts are present

                this.username = dataSplit[1];
                this.id = int.Parse(dataSplit[2]);
                string encodedMessage = dataSplit[3];
                
                // Decode the message

                Console.WriteLine("[SERVER]: " + this.username + " sent message: " + encodedMessage);

                // Send the message to the database
                Database database = new Database();
                await database.InsertChatLog(this.id, this.username, encodedMessage);

                // Log to verify correct sender ID
                Console.WriteLine($"[SERVER]: Sending to clients, excluding sender ID: {this.id}");

                // Send the message to all clients
                await this.SendToClients("servermessage", $"{this.id}:{this.username}:{encodedMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SERVER]: Error in sending chat message - " + ex.Message);
            }
        }


        private async Task LoginCommand(string data, NetworkStream networkStream)
        {
            try
            {
                string[] dataSplit = data.Split(':');
                //if (dataSplit.Length < 3) return; // Ensure username and password are present
                if (dataSplit.Length < 3)
                {
                    Console.WriteLine("[SERVER]: Invalid data format");
                    return; // Ensure username and password are present
                }

                this.username = dataSplit[1];
                this.password = dataSplit[2];

                Database database = new Database();
                // Check if the login credentials are correct
                var userID = await database.LoginAsync(username, password);

                // Convert the user ID to a string
                string user_id = userID.HasValue ? userID.Value.ToString() : "fail";

                // Format the data to send to the client
                string dataToSend = $"serverlogin:{user_id}";

                ProtocolSI protocolSI = new ProtocolSI();
                // Format the login information to send to the client
                byte[] dataPacket = protocolSI.Make(ProtocolSICmdType.DATA, Encoding.UTF8.GetBytes(dataToSend));

                // Send the data to the client side
                await networkStream.WriteAsync(dataPacket, 0, dataPacket.Length);


                //connection information to the client if the login is successful
                if (userID.HasValue)
                {
                    string connectionToSend = $"serverconnection:{userID.Value}:{this.username}:true";
                    // Format the connection information to send to the client
                    byte[] conn = protocolSI.Make(ProtocolSICmdType.DATA, Encoding.UTF8.GetBytes(connectionToSend));

                    Console.WriteLine("sending new connection to the client");

                    // send connection information to the client
                    await networkStream.WriteAsync(conn, 0, conn.Length);
                }

                // Save login attempt to logs
                Log log = new Log("server");
                log.AddLog($"Login attempt with username: {username}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SERVER]: Error in sending login ID - " + ex.Message);
            }
        }


        private async Task RegisterCommand(string data, NetworkStream networkStream)
        {
            try
            {
                string[] dataSplit = data.Split(':');
                if (dataSplit.Length < 4) return; // Ensure all parts are present
               
                this.username = dataSplit[1];
                this.password = dataSplit[2];
                this.email = dataSplit[3];

                Database database = new Database();
                var registerID = await database.RegisterAsync(username, password, email);
                byte[] register_id = registerID.HasValue ? Encoding.UTF8.GetBytes(registerID.Value.ToString()) : Encoding.UTF8.GetBytes("fail");

                byte[] ack;
                ProtocolSI protocolSI = new ProtocolSI();
                ack = protocolSI.Make(ProtocolSICmdType.ACK, register_id);


                await networkStream.WriteAsync(ack, 0, ack.Length);
                Console.WriteLine("[SERVER]: " + username + " registration: " + (registerID.HasValue ? "success" : "fail"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SERVER]: Error in sending registration ID - " + ex.Message);
            }
        }

        private async Task SendToClients(string command, string message)
        {
            string formatMessageToClient = $"{command}:{message}";
            byte[] data = Encoding.UTF8.GetBytes(formatMessageToClient);

            ProtocolSI protocolSI = new ProtocolSI();
            byte[] encryptData = protocolSI.Make(ProtocolSICmdType.DATA, data);

            foreach (var kvp in connectedClients)
            {
                var clientId = kvp.Key;
                var client = kvp.Value;

                if (client != null && client.Connected)
                {
                    NetworkStream networkStream = client.GetStream();
                    try
                    {
                        await networkStream.WriteAsync(encryptData, 0, encryptData.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[SERVER]: Error sending to client {clientId}: {e.Message}");
                    }
                }
            }
        }

        private async Task ReceivePublicKey(ProtocolSI protocolSI)
        {
            NetworkStream networkStream = client.GetStream();
            rsaServer = new RSACryptoServiceProvider();
               
            byte[] packet = protocolSI.Make(ProtocolSICmdType.PUBLIC_KEY, rsaServer.ToXmlString(false));
            await networkStream.WriteAsync(packet, 0, packet.Length);
        }

        private void ReceiveSecretKey(byte[] data)
        {
            aesServer = new AesCryptoServiceProvider();

            aesServer.Key = rsaServer.Decrypt(data, true);
            this.privateKey = aesServer.Key;

            Console.WriteLine("[SERVER]: Received secret key from client");

        }

        private void ReceiveIV(byte[] data)
        {
            aesServer = new AesCryptoServiceProvider();

            aesServer.IV = rsaServer.Decrypt(data, true);
            this.privateIV = aesServer.IV;

            Console.WriteLine("[SERVER]: Received secret IV from client");
        }
    }
}

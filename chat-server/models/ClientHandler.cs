﻿using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using chat_server.connection;
using chat_server.models;
using EI.SI;

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
                        byte[] ack;

                        // If the client disconnected, close the network stream and client
                        if (bytesRead == 0)
                        {
                            Console.WriteLine("[SERVER]: Client " + id + " disconnected");
                            break;
                        }

                        // Set the buffer to the protocolSI object
                        protocolSI.SetBuffer(protocolSI.Buffer);

                        string data = Encoding.UTF8.GetString(protocolSI.GetData());
                        string[] dataSplit = data.Split(':');

                        // Ensure the data is in the correct format
                        if (dataSplit.Length < 2)
                            continue;

                        string command = dataSplit[0];
                        Console.WriteLine("[SERVER]: Command received: " + command);

                        switch (command)
                        {
                            case "chat":
                                if (dataSplit.Length < 4) continue; // Ensure all parts are present

                                this.username = dataSplit[1];
                                this.id = int.Parse(dataSplit[2]);
                                string message = dataSplit[3];

                                Console.WriteLine("[SERVER]: " + this.username + " sent message: " + message);

                                // Send the message to the database
                                Database database = new Database();
                                await database.InsertChatLog(this.id, this.username, message);

                                // Send the message to all clients
                                await this.SendToClients("servermessage", $"{this.username}:{message}");
                                break;

                            case "login":
                                if (dataSplit.Length < 3) continue; // Ensure username and password are present
                                this.username = dataSplit[1];
                                this.password = dataSplit[2];
                                database = new Database();

                                var userID = await database.LoginAsync(username, password);
                                byte[] user_id = userID.HasValue ? Encoding.UTF8.GetBytes(userID.Value.ToString()) : Encoding.UTF8.GetBytes("fail");

                                ack = protocolSI.Make(ProtocolSICmdType.ACK, user_id);

                                await networkStream.WriteAsync(ack, 0, ack.Length);
                                Console.WriteLine("[SERVER]: " + username + " authentication: " + (userID.HasValue ? "success" : "fail"));
                                break;

                            case "register":
                                try
                                {
                                    if (dataSplit.Length < 4) continue; // Ensure all parts are present
                                    this.username = dataSplit[1];
                                    this.password = dataSplit[2];
                                    this.email = dataSplit[3];

                                    database = new Database();
                                    var registerID = await database.RegisterAsync(username, password, email);
                                    byte[] register_id = registerID.HasValue ? Encoding.UTF8.GetBytes(registerID.Value.ToString()) : Encoding.UTF8.GetBytes("fail");

                                    ack = protocolSI.Make(ProtocolSICmdType.ACK, register_id);

                                    await networkStream.WriteAsync(ack, 0, ack.Length);
                                    Console.WriteLine("[SERVER]: " + username + " registration: " + (registerID.HasValue ? "success" : "fail"));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("[SERVER]: Error in sending registration ID - " + ex.Message);
                                }
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

        private async Task SendToClients(string command, string message)
        {
            string formatMessageToClient = $"{command}:{message}";
            byte[] data = Encoding.UTF8.GetBytes(formatMessageToClient);
            ProtocolSI protocolSI = new ProtocolSI();
            byte[] encryptData = protocolSI.Make(ProtocolSICmdType.ACK, data);

            // Send the message to all connected clients
            foreach (var kvp in connectedClients)
            {
                // prevent sending to the client that sent the message

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
                        Console.WriteLine($"[SERVER]: Error sending to client {kvp.Key}: {e.Message}");
                    }
                }
            }
        }
    }
}

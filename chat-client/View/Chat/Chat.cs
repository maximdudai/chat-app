using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EI.SI;
using chat_client.models;
using chat_client.View.Chat.Emoji;
using System.Drawing;
using System.IO;

namespace chat_client.View.Chat
{
    public partial class Chat : Form
    {
        private int id { get; }
        private string username { get; }
        private List<ChatModel> messageList;
        private List<Connection> connectionList;

        private const int PORT = 5555;
        private static NetworkStream networkStream;
        private static TcpClient tcpClient;
        private ProtocolSI protocolSI;

        public Chat(int id, string username)
        {
            InitializeComponent();
            if (tcpClient == null || !tcpClient.Connected)
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
                tcpClient = new TcpClient();
                tcpClient.Connect(endPoint);
                networkStream = tcpClient.GetStream();
            }
            protocolSI = new ProtocolSI();

            messageList = new List<ChatModel>();
            connectionList = new List<Connection>();

            this.id = id;
            this.username = username;

            // Set the logged username
            loggedUsername.Text = username;

            // Receive messages from the server
            Task.Run(async () => await this.ReceiveDataFromServer());
            Task.Run(async () => await this.SendNewConnection(id, username));
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            int retryCount = 0;
            int maxRetries = 3; // Maximum number of retries
            bool messageSent = false; // Flag to check if the message has been sent successfully

            // Retry sending the message if it fails
            while (!messageSent && retryCount < maxRetries)
            {
                try
                {
                    // Get the message written by the client
                    string msg = messageTextBox.Text;
                    messageTextBox.Clear();

                    string message = this.EncodeMessage(msg);

                    // Format the message to be sent to the server
                    string sendToServer = $"chat:{this.username}:{this.id}:{message}";

                    byte[] dataPacket = protocolSI.Make(ProtocolSICmdType.DATA, sendToServer);

                    if (networkStream.CanWrite)
                    {
                        await networkStream.WriteAsync(dataPacket, 0, dataPacket.Length);
                        messageSent = true; // Set the flag if  is successful
                    }
                    else
                    {
                        throw new InvalidOperationException("Network stream is not writable.");
                    }
                }
                catch (IOException ex)
                {
                    retryCount++; // Increment retry count
                    Console.WriteLine($"[CLIENT]: Retry {retryCount} after IO exception: {ex.Message}");
                    // Optionally add a delay here if needed
                    await Task.Delay(1000); // Delay before retrying
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[CLIENT]: An error occurred: " + ex.Message);
                    break; // Break the loop on non-recoverable errors
                }
            }

            if (!messageSent)
            {
                Console.WriteLine("[CLIENT]: Failed to send message after retries.");
            }
        }

        private void CloseClient()
        {
            try
            {
                // Send an EOT packet to the server
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                networkStream.Write(eot, 0, eot.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("[CLIENT]: An error occurred: " + e.Message);
            }
            finally
            {
                networkStream?.Close();
                tcpClient?.Close();
                tcpClient = null; // Ensure new connection is created if needed
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string message = messageTextBox.Text;

            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Write your message!", "Empty field", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // Send the message to the server
            this.buttonSend_Click(sender, e);
        }

        private void UpdateChatMListBox()
        {
            // Update the ListBox with the updated messages
            chatMessageListBox.DataSource = null;
            chatMessageListBox.DataSource = messageList;
        }
        private void UpdateConnectionList()
        {
            // Update the ListBox with the updated messages
            chatConnectionListBox.DataSource = null;
            chatConnectionListBox.DataSource = connectionList;
        }

        private void HandleLoginAccount(object sender, EventArgs e)
        {
            this.CloseClient();

            Login.Login loginForm = new Login.Login();
            loginForm.Show();

            this.Hide();
        }

        private void HandleFormClosing(object sender, FormClosingEventArgs e)
        {
            this.CloseClient();
        }

        private void EmojiListButton_Click(object sender, EventArgs e)
        {
            EmojiForm emojiForm = new EmojiForm();

            emojiForm.StartPosition = FormStartPosition.Manual;
            // Position Form2 to open at the lower right corner of emojiListButton

            //get chat form location
            int screen_x = this.Location.X + this.Width - 725;
            int screen_y = this.Location.Y - 50;

            // set emoji form location
            Point location = this.PointToScreen(new Point(screen_x, screen_y));

            emojiForm.Location = location;

            // Add event listener to get the selected emoji
            emojiForm.EmojiSelected += EmojiForm_EmojiSelected;

            // Remove event listener to avoid memory leaks
            emojiForm.FormClosed += (s, args) => emojiForm.EmojiSelected -= EmojiForm_EmojiSelected;

            emojiForm.Show();
        }

        private void EmojiForm_EmojiSelected(string emoji)
        {
            // Append the selected emoji to the messageTextBox
            messageTextBox.AppendText(emoji);
        }

        // Receive messages from the server
        private async Task ReceiveDataFromServer()
        {
            try
            {
                while (true)
                {
                    int bytesRead = await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("[CLIENT]: Server disconnected");
                        break;
                    }
                    // Set the buffer to the protocolSI object
                    protocolSI.SetBuffer(protocolSI.Buffer);
                    if (protocolSI.GetDataLength() > 1400)
                    { 
                        Console.WriteLine("[CLIENT]: Received data exceeds allowable limits. " + protocolSI.GetDataLength());
                        continue;
                    }

                    var cmdType = protocolSI.GetCmdType();

                    string data = Encoding.UTF8.GetString(protocolSI.GetData(), 0, protocolSI.GetDataLength());
                    string[] dataSplit = data.Split(':');

                    Console.WriteLine("[CLIENT]: Command type received: " + cmdType);

                    switch (cmdType)
                    {
                        case ProtocolSICmdType.DATA:
                            // Ensure the data is in the correct format
                            if (dataSplit.Length < 1)
                                continue;

                            string command = dataSplit[0];
                            Console.WriteLine("[CLIENT]: Command received: " + command);

                            switch (command)
                            {
                                case "servermessage":
                                    this.TypeMessage(data);
                                    break;

                                case "clientconnection":
                                    this.TypeConnection(data);
                                    break;

                                default:
                                    Console.WriteLine("[CLIENT]: Invalid command received");
                                    break;
                            }
                            break;

                        case ProtocolSICmdType.EOT:
                            Console.WriteLine("[CLIENT]: Client " + this.id + " disconnected (EOT)");
                            break;

                        default:
                            Console.WriteLine("[CLIENT]: Invalid command received");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[CLIENT]: " + e.Message);
            }
        }

        private void TypeMessage(string data)
        {
            try
            {
                string[] dataSplit = data.Split(':');

                // Save received data from there as a variable
                int client_id = int.Parse(dataSplit[1]);
                string client_username = dataSplit[2];
                string client_message = dataSplit[3];

                string decodedMessage = this.DecodeMessage(client_message);

                // Format: servermessage:username:message
                ChatModel chatModel = new ChatModel(client_id, client_username, decodedMessage);
                messageList.Add(chatModel);
                Invoke(new Action(UpdateChatMListBox)); // Ensure UI updates on the main thread

                // Scroll to the last message
            }
            catch (Exception e)
            {
                Console.WriteLine("[CLIENT]: " + e.Message);
            }
       
        }

        private async Task SendNewConnection(int id, string username)
        {
            string sendToServer = $"serverconnection:{id}:{username}:true";
            byte[] dataPacket = protocolSI.Make(ProtocolSICmdType.DATA, sendToServer);

            await networkStream.WriteAsync(dataPacket, 0, dataPacket.Length);
        }

        private void TypeConnection(string data)
        {
            try
            {
                string[] dataSplit = data.Split(':');

                if (dataSplit.Length < 4)
                {
                    Console.WriteLine("Received data is not in the expected format.");
                    return;
                }

                // Save received data from there as a variable
                int client_id = int.Parse(dataSplit[1]);
                string client_username = dataSplit[2];

                // when user connects option = true
                // when user disconnects option = false
                bool option = bool.Parse(dataSplit[3]);

                Connection conn = new Connection(client_username, client_id, option);
                connectionList.Add(conn);
                Invoke(new Action(UpdateConnectionList)); // Ensure UI updates on the main thread
            }
            catch (Exception e)
            {
                Console.WriteLine("[CLIENT]: " + e.Message);
            }
        }

        private string DecodeMessage(string encodedMessage)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedMessage));
        }
        private string EncodeMessage(string message)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
        }
    }
}

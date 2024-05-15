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
            this.id = id;
            this.username = username;

            // Send connecting message to listBox
            this.handleChatConnection();

            // Receive messages from the server
            Task.Run(async () => await this.ReceiveMessages());
        }

        public void handleChatConnection(bool option = true)
        {
            try
            {
                Connection conn = new Connection(this.username, this.id, option);
                chatConnectionListBox.Items.Add(conn);
            }
            catch (Exception e)
            {
                Console.WriteLine("[CLIENT]: " + e.Message);
            }
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

                    // Format the message to be sent to the server
                    string sendToServer = $"chat:{this.username}:{this.id}:{msg}";
                    byte[] dataPacket = protocolSI.Make(ProtocolSICmdType.DATA, Encoding.UTF8.GetBytes(sendToServer));

                    if (networkStream.CanWrite)
                    {
                        await networkStream.WriteAsync(dataPacket, 0, dataPacket.Length);
                        messageSent = true; // Set the flag if send is successful
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

        private void sendButton_Click(object sender, EventArgs e)
        {
            string message = messageTextBox.Text;

            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Write your message!", "Empty field", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create a ChatModel object with the client's ID, username, and message
            ChatModel chatModel = new ChatModel(this.id, this.username, message);

            // Add the message to the message list
            messageList.Add(chatModel);

            // Update the ListBox with the updated messages
            updateChatMListBox();

            // Send the message to the server
            this.buttonSend_Click(sender, e);
        }

        private void updateChatMListBox()
        {
            // Update the ListBox with the updated messages
            chatMessageListBox.DataSource = null;
            chatMessageListBox.DataSource = messageList;
        }

        private void handleLoginAccount(object sender, EventArgs e)
        {
            this.CloseClient();

            Login.Login loginForm = new Login.Login();
            loginForm.Show();

            this.Hide();
        }

        private void handleFormClosing(object sender, FormClosingEventArgs e)
        {
            this.CloseClient();
        }

        private void emojiListButton_Click(object sender, EventArgs e)
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
        private async Task ReceiveMessages()
        {
            Console.WriteLine("[CLIENT]: Receiving messages from the server - " + this.username);
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

                    protocolSI.SetBuffer(protocolSI.Buffer);

                    if (protocolSI.GetDataLength() > 1400)
                    {
                        Console.WriteLine("[CLIENT]: Received data exceeds allowable limits. " + protocolSI.GetDataLength());
                        continue;
                    }

                    string data = Encoding.UTF8.GetString(protocolSI.GetData(), 0, protocolSI.GetDataLength());
                    Console.WriteLine("[CLIENT]: Raw data received: " + data);

                    string[] dataSplit = data.Split(':');

                    if (dataSplit[0] == "servermessage")
                    {
                        // Format: servermessage:username:message
                        ChatModel chatModel = new ChatModel(this.id, dataSplit[1], dataSplit[2]);
                        messageList.Add(chatModel);
                        Invoke(new Action(updateChatMListBox)); // Ensure UI updates on the main thread
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[CLIENT]: " + e.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EI.SI;
using chat_client.models;
using chat_client.View.Chat.Emoji;
using System.IO;

namespace chat_client.View.Chat
{
    public partial class Chat : Form
    {
        private int id { get; }
        private string username { get; }
        private List<ChatModel> messageList;

        private const int PORT = 5555;
        NetworkStream networkStream;
        TcpClient tcpClient;
        ProtocolSI protocolSI;

        public Chat(int id, string username)
        {
            InitializeComponent();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            tcpClient = new TcpClient();
            tcpClient.Connect(endPoint);
            networkStream = tcpClient.GetStream();
            protocolSI = new ProtocolSI();

            messageList = new List<ChatModel>();
            this.id = id;
            this.username = username;
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
                // Enviar um pacote EOT para o servidor
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                networkStream.Write(eot, 0, eot.Length);
                
            } 
            catch (Exception e)
            {
                Console.WriteLine("[CLIENT]: An error ocured: " + e.Message);
            }
            finally
            {
                networkStream?.Close();
                tcpClient?.Close();
            }

        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string mensagem = messageTextBox.Text;

            if (string.IsNullOrEmpty(mensagem))
            {
                MessageBox.Show("Write your message!", "Empty field", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //MessageBox.Show("Empty field: write your message");
                return;
            }

            // Cria um objeto ChatModel com o id do cliente, username e a mensagem
            ChatModel chatModel = new ChatModel(this.id, this.username, mensagem);

            // Adiciona a mensagem à lista de mensagens
            messageList.Add(chatModel);

            // Atualiza a ListBox com as mensagens atualizadas
            updateChatMListBox();

            // Enviar a mensagem para o servidor
            this.buttonSend_Click(sender, e);
        }
        private void updateChatMListBox()
        {
            // Atualiza a ListBox com as mensagens atualizadas
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
    }
}

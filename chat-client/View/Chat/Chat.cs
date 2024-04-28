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
            try
            {
                // Obter a mensagem escrita pelo cliente
                string msg = messageTextBox.Text;
                messageTextBox.Clear();

                // formatar a mensagem para ser enviada ao servidor
                string sendToServer = $"chat:{this.username}:{this.id}:{msg}";
                byte[] dataPacket = protocolSI.Make(ProtocolSICmdType.DATA, Encoding.UTF8.GetBytes(sendToServer));
                await networkStream.WriteAsync(dataPacket, 0, dataPacket.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CLIENT]: An error ocured: " + ex.Message);
            }
        }

        private void CloseClient()
        {
            try
            {
                // Enviar um pacote EOT para o servidor
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                networkStream.Write(eot, 0, eot.Length);

                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
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
                MessageBox.Show("Escreva sua mensagem, a caixa de texto não pode estar vazia");
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
            Login.Login loginForm = new Login.Login();

            this.CloseClient();
            this.Hide();
            
            loginForm.Show();
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
            emojiForm.EmojiSelected += EmojiForm_EmojiSelected;
            emojiForm.FormClosed += (s, args) => emojiForm.EmojiSelected -= EmojiForm_EmojiSelected; // Unsubscribe to avoid memory leaks
            emojiForm.Show(); // Use Show instead of ShowDialog to keep focus on the text box

        }

        private void EmojiForm_EmojiSelected(string emoji)
        {
            Console.WriteLine("emoji: " + emoji);
            messageTextBox.AppendText(emoji);
        }
    }
}

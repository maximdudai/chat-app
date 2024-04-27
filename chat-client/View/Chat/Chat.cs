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

namespace chat_client.View.Chat
{
    public partial class Chat : Form
    {

        private const int PORT = 5555;
        NetworkStream networkStream;
        TcpClient tcpClient;
        ProtocolSI protocolSI;
        private List<string> messageList;

        public Chat()
        {
            InitializeComponent();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            tcpClient = new TcpClient();
            tcpClient.Connect(endPoint);
            networkStream = tcpClient.GetStream();
            protocolSI = new ProtocolSI();
            messageList = new List<string>();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string msg = messageTextBox.Text;
            messageTextBox.Clear();
            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg);
            networkStream.Write(packet, 0, packet.Length);
            while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                networkStream.Read(protocolSI.Buffer, 0,
                    protocolSI.Buffer.Length);
            }
        }

        private void CloseClient()
        {
            byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
            networkStream.Write(eot, 0, eot.Length);
            networkStream.Read(protocolSI.Buffer, 0,
                protocolSI.Buffer.Length);
            networkStream.Close();
            tcpClient.Close();
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            CloseClient();
            this.Close();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string mensagem = messageTextBox.Text;

            if (string.IsNullOrEmpty(mensagem))
            {
                MessageBox.Show("Escreva sua mensagem, a caixa de texto não pode estar vazia");
                return;
            }

            // Adiciona a mensagem à lista de mensagens
            messageList.Add(mensagem);

            // Atualiza a ListBox com as mensagens atualizadas
            updateChatMListBox();

            // Limpa a TextBox depois do envio da mensagem
            messageTextBox.Clear();
        }
        private void updateChatMListBox()
        {
            chatMessageListBox.DataSource = null;
            chatMessageListBox.DataSource = messageList;
        }

        private void handleLoginAccount(object sender, EventArgs e)
        {
            Login.Login loginForm = new Login.Login();

            this.Hide();
            loginForm.Show();
        }
    }
}

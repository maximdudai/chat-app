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

namespace chat_client.View.Chat
{
    public partial class Chat : Form
    {

        private const int PORT = 10000;
        NetworkStream networkStream;
        TcpClient tcpClient;
        ProtocolSI protocolSI;

        public Chat()
        {
            InitializeComponent();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            tcpClient = new TcpClient();
            tcpClient.Connect(endPoint);
            networkStream = tcpClient.GetStream();
            protocolSI = new ProtocolSI();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string msg = textBoxMessage.Text;
            textBoxMessage.Clear();
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseClient();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using chat_server.connection;
using EI.SI;
using MySqlX.XDevAPI;

namespace chat_server.models
{
    public class ChatHandler
    {
        private int userid { get; set; }
        private string message { get; set; }
        private TcpClient client;


        public ChatHandler(int userid, string message)
        {
            this.userid = userid;
            this.message = message;
        }

        public void ReceiveMessageFromClient()
        {
            // Run the ReceiveMessage method asynchronously
            Task.Run(async () => await ReceiveMessage());
        }

        private async Task ReceiveMessage()
        {
            NetworkStream networkStream = client.GetStream();
            ProtocolSI protocolSI = new ProtocolSI();
            Database database = new Database();

            //Asynchronously read from the network stream
            int bytesRead = await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            // Check if the client has disconnected
            if (bytesRead == 0)
                return;

            byte[] ack;

            switch (protocolSI.GetCmdType())
            {
                case ProtocolSICmdType.DATA:

                    //Extract the username and password from the data packet
                    string data = Encoding.UTF8.GetString(protocolSI.GetData());

                    //Split the data into username and password
                    string[] credentials = data.Split(':');
                    
                    this.userid = int.Parse(credentials[0]);
                    this.message = credentials[1];

                    //Insert the message into the database
                    await database.SendMessageAsync(this.userid, this.message);
                    break;

                case ProtocolSICmdType.EOT:
                    break;
            }

            database.CloseConnection();
        }
    }
}

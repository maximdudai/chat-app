using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EI.SI;

namespace chat_server.models
{
    internal class ClientChat
    {
        private string username;
        private string command;
        private string message;

        private ProtocolSI protocolSI;

        public ClientChat(string command, string username, string message)
        {
            this.command = command;
            this.username = username;
            this.message = message;
        }

        
    }
}

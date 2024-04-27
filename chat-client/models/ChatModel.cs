using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat_client.models
{
    internal class ChatModel
    {
        public int userid { get; set; }
        public string username { get; set; }
        public string message { get; set; }

        public ChatModel(int userid, string username, string message)
        {
            this.userid = userid;
            this.username = username;
            this.message = message;
        }

        public override string ToString()
        {
            return $"({userid}) {username}: {message}";
        }
    }
}

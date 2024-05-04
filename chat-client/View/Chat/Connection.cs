using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat_client.View.Chat
{
    internal class Connection
    {
        public string username { get; set; }
        public int id { get; set; }

        public bool option { get; set; }

        public Connection(string username, int id, bool option)
        {
            this.username = username;
            this.id = id;
            // if connected option = true
            // if disconnected option = false
            this.option = option;
        }

        public override string ToString()
        {
            DateTime now = DateTime.Now;

            string connTime = now.ToString("H:mm:ss");

            if (option)
                return $"({this.id}) {this.username} has connected! {connTime}";
            else
                return $"({this.id}) {this.username} has disconnected! {connTime}";
        }
    }
}

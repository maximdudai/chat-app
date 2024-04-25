using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat_server.models
{
    internal class ClientCount
    {
        private int count;

        public int GetCount()
        {
            return this.count;
        }

        public void Increment()
        {
            count++;
        }

        public void Decrement()
        {
            count--;
        }
    }
}

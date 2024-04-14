using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat_client
{
    public partial class chatApp : Form
    {
        public chatApp()
        {
            InitializeComponent();

            // prevent resizing of form
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }
    }
}

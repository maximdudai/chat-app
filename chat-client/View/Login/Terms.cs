﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat_client.View.Login
{
    public partial class Terms : Form
    {
        public Terms()
        {
            InitializeComponent();
        }

        private void CloseTerms_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

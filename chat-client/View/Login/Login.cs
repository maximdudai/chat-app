using System;
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
    public partial class Login : Form
    {
        private string username;
        private string password;

        private bool isPasswordVisible = false;
        public Login()
        {
            InitializeComponent();

        }

        private void togglePasswordVisibility(object sender, EventArgs e)
        {
            //update input label visibility
            loginPassword.UseSystemPasswordChar = !isPasswordVisible;

            //update variable state
            isPasswordVisible = !isPasswordVisible;
        }

        private void handleCreateAccount(object sender, EventArgs e)
        {
            Register.Register registerForm = new Register.Register();

            this.Hide();
            registerForm.ShowDialog();
        }
    }
}

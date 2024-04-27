using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat_client.View.Register
{
    public partial class Register : Form
    {
        private const int PORT = 5555;
        NetworkStream networkStream;
        TcpClient tcpClient;
        ProtocolSI protocolSI;
        public Register()
        {
            InitializeComponent();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            tcpClient = new TcpClient();

            //error handling when server is not running
            tcpClient.Connect(endPoint);

            networkStream = tcpClient.GetStream();
            protocolSI = new ProtocolSI();
        }

        //go to login form
        private void handleLoginAccount(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login.Login loginForm = new Login.Login();

            this.Hide();
            loginForm.Show();
        }

        //create a new account
        private async void registerButton_Click(object sender, EventArgs e)
        {
            //registerButton.Enabled = false; // Prevent further clicks

            string email = registerEmail.Text;
            string username = registerUsername.Text;
            string password = registerPassword.Text;
            string passConfirmation = passwordConfirmation.Text;
            bool checkBoxAgree = checkBoxRegister.Checked;

            //check that all fields are filled
            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(passConfirmation))
            {
                MessageBox.Show("All fields are required!");
                return;
            }

            if (!checkBoxAgree)

            {
                MessageBox.Show("You must accept the Terms of Use and Privacy Policy");
                return;
            }

            //check that is an email
            try
            {
                MailAddress address = new MailAddress(email);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Confirm that the email is correct");
                return;
            }

            //validations for password
            if (password != passConfirmation)
            {
                MessageBox.Show("Password mismatch!");
                return;
            }

            if (password.Count() < 6)
            {
                MessageBox.Show("Your password must be over 6 chars");
                return;
            }

            //Validar se já existe o username e email na bd

            


            //DO LOGIN--------------------------------------------------------------------------------------
            try
            {
                // Assemble the data packet for register
                string dataToSend = $"register:{username}:{password}:{email}";
                byte[] dataPacket = protocolSI.Make(ProtocolSICmdType.DATA, Encoding.UTF8.GetBytes(dataToSend));

                // Send register data to the server asynchronously
                await networkStream.WriteAsync(dataPacket, 0, dataPacket.Length);

                // Start receiving data from the server asynchronously
                await ReceiveDataFromServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during register: {ex.Message}");
            }
            finally
            {
                //registerButton.Enabled = true; // Re-enable the button after operation is complete
            }

        }

        private async Task ReceiveDataFromServer()
        {
            NetworkStream networkStream = tcpClient.GetStream();
            ProtocolSI protocolSI = new ProtocolSI();
            try
            {
                // Read and process data from the server asynchronously
                int bytesRead = await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                if (bytesRead > 0)
                {

                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.ACK:
                            string data = Encoding.UTF8.GetString(protocolSI.GetData());

                            switch (data)
                            {
                                case "success":
                                    // Use Invoke to update the UI on the UI thread
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        Chat.Chat chatForm = new Chat.Chat();
                                        this.Hide();
                                        chatForm.ShowDialog();
                                    });
                                    break;
                                case "fail":
                                    // Use Invoke to show the message box on the UI thread
                                    //separar por erros
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        MessageBox.Show("Email or Username already used, please retry!", "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    });
                                    break;
                            }
                            break;

                        case ProtocolSICmdType.EOT:
                            Console.WriteLine("Client disconnected");
                            break;

                        default:
                            Console.WriteLine("Invalid command received");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions from asynchronous operations
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Error receiving data from server: {ex.Message}");
                });
            }
        }
        
        //DO LOGIN--------------------------------------------------------------------------------------
    }
}

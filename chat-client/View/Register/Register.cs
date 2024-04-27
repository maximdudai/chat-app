﻿using EI.SI;
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

        private string email { get; set; }
        private string username { get; set; }

        private int id { get; set; }
        private string password { get; set; }

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

            this.email = registerEmail.Text;
            this.username = registerUsername.Text;
            this.password = registerPassword.Text;

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
                            var data = Encoding.UTF8.GetString(protocolSI.GetData());

                            if (!int.TryParse(data, out int result))
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    MessageBox.Show("A user with this username already exists!", "Authenticate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                });
                                return;
                            }

                            this.id = int.Parse(data);

                            this.Invoke((MethodInvoker)delegate
                            {
                                Chat.Chat chat = new Chat.Chat(id, this.username);
                                chat.Show();
                                this.Hide();
                            });

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

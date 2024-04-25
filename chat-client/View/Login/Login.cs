using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using chat_client.View.Chat;
using EI.SI;

namespace chat_client.View.Login
{
    public partial class Login : Form
    {
        private const int PORT = 5555;
        NetworkStream networkStream;
        TcpClient tcpClient;
        ProtocolSI protocolSI;

        private bool isPasswordVisible = false;
        public Login()
        {
            InitializeComponent();

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            tcpClient = new TcpClient();

            //error handling when server is not running
            tcpClient.Connect(endPoint);

            networkStream = tcpClient.GetStream();
            protocolSI = new ProtocolSI();
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
        private async void loginButton_Click(object sender, EventArgs e)
        {
            loginButton.Enabled = false; // Prevent further clicks
            string username = loginUsername.Text;
            string password = loginPassword.Text;

            try
            {
                // Assemble the data packet for login
                string dataToSend = $"login:{username}:{password}";
                byte[] dataPacket = protocolSI.Make(ProtocolSICmdType.DATA, Encoding.UTF8.GetBytes(dataToSend));

                // Send login data to the server asynchronously
                await networkStream.WriteAsync(dataPacket, 0, dataPacket.Length);

                // Start receiving data from the server asynchronously
                await ReceiveDataFromServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during login: {ex.Message}");
            }
            finally
            {
                loginButton.Enabled = true; // Re-enable the button after operation is complete
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
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        MessageBox.Show("Invalid Username or Password, please retry!", "Authenticate", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private void OnClientClose(object sender, FormClosedEventArgs e)
        {
            try
            {
                // Send EOT to the server to close the connection
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                if (networkStream.CanWrite)
                {
                    networkStream.Write(eot, 0, eot.Length);
                }
                // The server will close the connection upon receiving EOT
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while closing connection: {ex.Message}");
            }
            finally
            {
                // Close the network stream and the TCP client
                networkStream?.Close();
                tcpClient?.Close();
            }
        }


        private void HandleCloseClient(object sender, FormClosingEventArgs e)
        {
            this.OnClientClose(sender, null);
        }
    }
}

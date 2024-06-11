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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using chat_client.View.Chat;
using chat_log;
using EI.SI;

namespace chat_client.View.Login
{
    public partial class Login : Form
    {
        private const int PORT = 5555;
        NetworkStream networkStream;
        TcpClient tcpClient;
        ProtocolSI protocolSI;
        private bool isPasswordVisible;

        private int id { get; set; }
        private string username { get; set; }
        private string password { get; set; }
        private string publicKey { get; set; }

        private RSACryptoServiceProvider rsaClient;
        private AesCryptoServiceProvider aesClient;

        public Login()
        {
            InitializeComponent();

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            tcpClient = new TcpClient();

            //error handling when server is not running
            tcpClient.Connect(endPoint);

            networkStream = tcpClient.GetStream();
            protocolSI = new ProtocolSI();

            Task.Run(async () => await this.ReceiveDataFromServer());
            Task.Run(async () => await this.GeneratePublicKey());

            this.isPasswordVisible = false;
    }

    private void togglePasswordVisibility(object sender, EventArgs e)
        {
            this.isPasswordVisible = !this.isPasswordVisible;

            //update input label visibility
            loginPassword.UseSystemPasswordChar = this.isPasswordVisible;
        }

        private void handleCreateAccount(object sender, EventArgs e)
        {
            Register.Register registerForm = new Register.Register();

            this.Hide();
            registerForm.Show();
        }
        private async void loginButton_Click(object sender, EventArgs e)
        {
            // prevent client to force login multiple times
            loginButton.Enabled = false;
            this.username = loginUsername.Text;
            this.password = loginPassword.Text;

            var authRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_]+$");


            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loginButton.Enabled = true;
                return;
            }

            if (!authRegex.IsMatch(username) || !authRegex.IsMatch(password))
            {
                MessageBox.Show("Username or Password can only contain letters, numbers, and underscores", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loginButton.Enabled = true;
                return;
            }

            try
            {
                // format data to send to server
                string dataToSend = $"login:{username}:{password}";
                byte[] dataPacket = protocolSI.Make(ProtocolSICmdType.DATA, Encoding.UTF8.GetBytes(dataToSend));

                // Send login data to the server asynchronously
                await networkStream.WriteAsync(dataPacket, 0, dataPacket.Length);

                // Log the login attempt
                Log log = new Log("client");
                log.AddLog($"Login attempt with username: {username}");

                // Start receiving data from the server asynchronously
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
                while (true)
                {
                    int bytesRead = await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if(bytesRead == 0)
                    {
                        break;
                    }

                string data = Encoding.UTF8.GetString(protocolSI.GetData());
                string[] dataSplit = data.Split(':');

                Console.WriteLine($"Command received: " + protocolSI.GetCmdType() + " datasplit0: " + dataSplit[0]);

                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.DATA:
                            {
                                switch (dataSplit[0])
                                {
                                    case "serverlogin":
                                        this.OnClientLogin(data);
                                        break;

                                    default:
                                        break;

                                }
                                break;
                            }

                        case ProtocolSICmdType.PUBLIC_KEY:
                            await this.OnClientReceivePublicKey(protocolSI.GetData());
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

        private void OnClientLogin(string data)
        {
            string[] dataSplit = data.Split(':');

            Console.WriteLine($"Received data from server: {data}");

            // Await the server response to the login request
            // if the response is an string, it means the login failed
            string user_id = dataSplit[1];

            if (!int.TryParse(user_id, out int result))
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Invalid Username or Password, please retry!", "Authenticate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
                return;
            }

            // If the response is an integer, it means the login was successful
            // Assign received user_id to the id property
            this.id = int.Parse(user_id);

            this.Invoke((MethodInvoker)delegate
            {
                Chat.Chat chat = new Chat.Chat(this.id, this.username);
                chat.Show();
                this.Hide();
            });
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

        private async Task GeneratePublicKey()
        {
            Console.WriteLine("Generating public key");
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                this.publicKey = rsa.ToXmlString(false);
            }

            ProtocolSI protocolSI = new ProtocolSI();
            byte[] keyToServer = protocolSI.Make(ProtocolSICmdType.PUBLIC_KEY, Encoding.UTF8.GetBytes(this.publicKey));

            Console.WriteLine("Sending public key to client");

            await networkStream.WriteAsync(keyToServer, 0, keyToServer.Length);
        }
        private async Task OnClientReceivePublicKey(byte[] data)
        {
            Console.WriteLine("Received public key from client");

            //receber pub key do server
            rsaClient = new RSACryptoServiceProvider();

            string publicKey = Encoding.UTF8.GetString(data);
            rsaClient.FromXmlString(publicKey);

            aesClient = new AesCryptoServiceProvider();
            byte[] encryptedKey = rsaClient.Encrypt(aesClient.Key, true);

            ProtocolSI protocolSI = new ProtocolSI();
            byte[] privateKey = protocolSI.Make(ProtocolSICmdType.SECRET_KEY, encryptedKey);

            Console.WriteLine("Sending private key to client");

            await networkStream.WriteAsync(privateKey, 0, privateKey.Length);
        }
    }
}

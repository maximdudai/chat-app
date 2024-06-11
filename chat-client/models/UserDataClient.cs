using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EI.SI;

namespace chat_client.models
{
    internal class UserDataClient
    {
        private string publicKey { get; set; }
        private RSACryptoServiceProvider rsaClient;
        private AesCryptoServiceProvider aesClient;

        private NetworkStream networkStream;
        private ProtocolSI protocolSI;

        public UserDataClient(NetworkStream networkStream, ProtocolSI protocolSI)
        {
            this.networkStream = networkStream;
            this.protocolSI = protocolSI;

            this.aesClient = new AesCryptoServiceProvider();
            this.rsaClient = new RSACryptoServiceProvider();
        }

        public async Task GeneratePublicKey()
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

        public async Task OnClientReceivePublicKey(byte[] data)
        {
            Console.WriteLine("Received public key from client");

            // Initialize RSA client
            rsaClient = new RSACryptoServiceProvider();
            string publicKey = Encoding.UTF8.GetString(data);
            rsaClient.FromXmlString(publicKey);

            // Initialize AES client and generate key and IV
            aesClient = new AesCryptoServiceProvider();
            aesClient.GenerateKey();
            aesClient.GenerateIV();

            // Encrypt AES key and IV with RSA public key
            byte[] encryptedKey = rsaClient.Encrypt(aesClient.Key, RSAEncryptionPadding.OaepSHA1);
            byte[] encryptedIV = rsaClient.Encrypt(aesClient.IV, RSAEncryptionPadding.OaepSHA1);

            // Send the encrypted AES key and IV to the server
            ProtocolSI protocolSI = new ProtocolSI();
            byte[] encryptedKeyMessage = protocolSI.Make(ProtocolSICmdType.SECRET_KEY, encryptedKey);
            byte[] encryptedIVMessage = protocolSI.Make(ProtocolSICmdType.IV, encryptedIV);

            Console.WriteLine("Sending encrypted AES key and IV to server");

            await networkStream.WriteAsync(encryptedKeyMessage, 0, encryptedKeyMessage.Length);
            await networkStream.WriteAsync(encryptedIVMessage, 0, encryptedIVMessage.Length);
        }

        // Method to encrypt data using AesCryptoServiceProvider
        public byte[] EncryptData(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            using (var encryptor = aesClient.CreateEncryptor())
            {
                byte[] encryptedData = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                return encryptedData;
            }
        }

        // Method to encrypt and send chat message
        public async Task EncryptAndSendMessage(string username, int id, string message)
        {
            string sendToServer = $"chat:{username}:{id}:{message}";
            byte[] encryptedMessage = EncryptData(sendToServer);

            // Send the encrypted message to the server
            ProtocolSI protocolSI = new ProtocolSI();
            byte[] encryptedMessagePacket = protocolSI.Make(ProtocolSICmdType.DATA, encryptedMessage);

            Console.WriteLine("Sending encrypted message to server");

            await networkStream.WriteAsync(encryptedMessagePacket, 0, encryptedMessagePacket.Length);
        }

        public async Task SendAuthData(string username, string password)
        {
            string dataToSend = $"login:{username}:{password}";
            byte[] encryptedAuth = EncryptData(dataToSend);

            // Send AES key and IV encrypted with RSA
            byte[] encryptedKey = rsaClient.Encrypt(aesClient.Key, RSAEncryptionPadding.OaepSHA1);
            byte[] encryptedIV = rsaClient.Encrypt(aesClient.IV, RSAEncryptionPadding.OaepSHA1);

            byte[] keyPacket = protocolSI.Make(ProtocolSICmdType.SECRET_KEY, encryptedKey);
            byte[] ivPacket = protocolSI.Make(ProtocolSICmdType.IV, encryptedIV);

            await networkStream.WriteAsync(keyPacket, 0, keyPacket.Length);
            await networkStream.WriteAsync(ivPacket, 0, ivPacket.Length);

            // Send encrypted authentication data
            byte[] dataPacket = protocolSI.Make(ProtocolSICmdType.DATA, encryptedAuth);
            await networkStream.WriteAsync(dataPacket, 0, dataPacket.Length);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EI.SI;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1.Crmf;

namespace chat_server.models
{
    internal class UserDataServer
    {
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;
        private TcpClient client;

        private RSACryptoServiceProvider rsaServer;
        private AesCryptoServiceProvider aesServer;

        public UserDataServer(NetworkStream networkStream, ProtocolSI protocolSI, TcpClient client)
        {
            this.networkStream = networkStream;
            this.protocolSI = protocolSI;
            this.client = client;

            aesServer = new AesCryptoServiceProvider();
        }

        public string ReceiveAndDecryptData(byte[] encryptedData)
        {
            try
            {
                byte[] decryptedData = aesServer.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                string dataString = Encoding.UTF8.GetString(decryptedData);

                return dataString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decrypting data: {ex.Message}");
                return null;
            }
        }

        public async Task ReceivePublicKey(ProtocolSI protocolSI)
        {
            NetworkStream networkStream = client.GetStream();
            rsaServer = new RSACryptoServiceProvider();

            byte[] packet = protocolSI.Make(ProtocolSICmdType.PUBLIC_KEY, rsaServer.ToXmlString(false));
            await networkStream.WriteAsync(packet, 0, packet.Length);
        }

        public void ReceiveSecretKey(byte[] data)
        {
            aesServer = new AesCryptoServiceProvider();
            aesServer.Key = rsaServer.Decrypt(data, RSAEncryptionPadding.OaepSHA1);

            Console.WriteLine("[SERVER]: Received secret key from client");

        }

        public void ReceiveEncrypterIV(byte[] data)
        {
            aesServer = new AesCryptoServiceProvider();
            aesServer.IV = rsaServer.Decrypt(data, RSAEncryptionPadding.OaepSHA1);


            Console.WriteLine("[SERVER]: Received IV from client");
        }
    }

}

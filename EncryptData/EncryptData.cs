using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptData
{
    public class EncryptData
    {
        private AesCryptoServiceProvider aes;

        public EncryptData(byte[] key, byte[] iv)
        {
            if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            {
                throw new ArgumentException("Invalid key size. Key must be 128, 192, or 256 bits.");
            }

            if (iv.Length != 16)
            {
                throw new ArgumentException("Invalid IV size. IV must be 128 bits.");
            }

            aes = new AesCryptoServiceProvider
            {
                Key = key,
                IV = iv,
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC
            };
        }

        public string CifrarTexto(string txt)
        {
            byte[] txtDecifrado = Encoding.UTF8.GetBytes(txt);
            byte[] txtCifrado;

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(txtDecifrado, 0, txtDecifrado.Length);
                cs.FlushFinalBlock();
                txtCifrado = ms.ToArray();
            }

            return Convert.ToBase64String(txtCifrado);
        }

        public string DecifrarTexto(byte[] txtCifrado)
        {
            byte[] txtDecifrado;

            using (MemoryStream ms = new MemoryStream(txtCifrado))
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (MemoryStream plainText = new MemoryStream())
            {
                cs.CopyTo(plainText);
                txtDecifrado = plainText.ToArray();
            }

            return Encoding.UTF8.GetString(txtDecifrado);
        }
    }


}

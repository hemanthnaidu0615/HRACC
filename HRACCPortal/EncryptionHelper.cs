using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HRACCPortal
{
    public static class EncryptionHelper
    {
        private const string Key = "3ce445b4f2e58720b5f0b065ff774447"; // Change this to your own encryption key

        public static string Encrypt(string plainText)
        {
            byte[] encryptedBytes;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = new byte[16]; // Using IV of all zeros for simplicity, ideally generate a random IV for each encryption

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedBytes);
        }
       
        public static string Decrypt(string cipherText)
        {
            try
            {
                if (cipherText == null)
                {
                    // Handle null input appropriately
                    throw new ArgumentNullException(nameof(cipherText), "Input cannot be null");
                }

                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                string decryptedText;

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                    aesAlg.IV = new byte[16]; // Using IV of all zeros for simplicity, should be same as the one used during encryption

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(cipherTextBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                decryptedText = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return decryptedText;
            }
            catch (FormatException ex)
            {
                // Handle the format exception gracefully
                Console.WriteLine("Error: Invalid Base64 string. " + ex.Message);
                return null; // Or return a default value, depending on your requirements
            }
            
        }
        
    }
}
//to be included in view code script tag
//<script src="https://cdnjs.cloudflare.com/ajax/libs/crypto-js/4.1.1/crypto-js.min.js"></script>

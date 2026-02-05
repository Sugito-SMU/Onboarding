using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;

namespace Onboarding.Common
{
    public class AESEncryption
    {
        private byte[] _KEY = null;
        private byte[] _IV = null;
        private const int _KEYSIZE = 16;

        public AESEncryption()
        {
            string acctFilePlain = ConfigurationManager.AppSettings["AESKeyFilePlain"];
            string acctFile = ConfigurationManager.AppSettings["AESKeyFile"];

            Dictionary<string, string> acct = null;

            if (acctFilePlain != null && acctFilePlain != string.Empty)
            {
                acct = Utility.GetSecuredKeys(acctFilePlain);
                
            }
            else
            {
                acct = Utility.GetSecuredEncrytedKeys(acctFile);
            }
                
            string key = acct["AESKey"];
            if (key.Length != _KEYSIZE)
            {
                throw new ArgumentException(string.Format("AES key size must be {0} characters"));
            }
            _KEY = System.Text.Encoding.ASCII.GetBytes(key);
            _IV = System.Text.Encoding.ASCII.GetBytes(new string('0', 16));
        }

        public string Encrypt(string clearText)
        {
            if (clearText == null || clearText == string.Empty)
                return string.Empty;
            else
            {
                byte[] encryptedStr = encryptStringToBytes_AES(clearText, _KEY, _IV);
                string output = BitConverter.ToString(encryptedStr);
                return output;
            }
        }

        public string Decrypt(string encrypted)
        {
            if (encrypted == null || encrypted == string.Empty)
                return string.Empty;
            else
            {
                byte[] byteArr = ToByteArray(encrypted);
                return decryptStringFromBytes_AES(byteArr, _KEY, _IV);
            }
        }

        private byte[] ToByteArray(string byteArrayString)
        {
            string[] parts = byteArrayString.Split('-');
            byte[] byteArray = new byte[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                byteArray[i] = Convert.ToByte(Convert.ToInt32(parts[i], 16));
            }
            return byteArray;
        }

        private byte[] encryptStringToBytes_AES(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Max plainText field length is 200
            if (plainText != null && plainText.Length > 200)
                throw new ArgumentException("plainText");

            // Declare the stream used to encrypt to an in memory
            // array of bytes.
            MemoryStream msEncrypt = null;

            // Declare the RijndaelManaged object
            // used to encrypt the data.
            RijndaelManaged aesAlg = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.BlockSize = 128;
                aesAlg.KeySize = 256;
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                msEncrypt = new MemoryStream();
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {

                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return msEncrypt.ToArray();
        }

        private string decryptStringFromBytes_AES(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Max cipherText field length is 400
            if (cipherText != null && cipherText.Length > 400)
                throw new ArgumentException("cipherText");

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.BlockSize = 128;
                aesAlg.KeySize = 256;
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }


    }

}
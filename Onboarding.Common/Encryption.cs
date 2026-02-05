using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Onboarding.Common
{
    public class Encryption
    {
        private static X509Certificate2 getCertificate(string certificateName)
        {
            X509Store my = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            my.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection collection = my.Certificates.Find(X509FindType.FindBySubjectName, certificateName, false);

            if (collection.Count == 1)
            {              
                return collection[0];
            }
            else if (collection.Count > 1)
            {
                throw new Exception(string.Format("More than one certificate with name '{0}' found in store LocalMachine/My.", certificateName));
            }
            else
            {
                throw new Exception(string.Format("Certificate '{0}' not found in store LocalMachine/My.", certificateName));
            }
        }

        public static string EncryptRsa(string certificateName, string input)
        {
            string output = string.Empty;
            if (!string.IsNullOrEmpty(input))
            {
                X509Certificate2 cert = getCertificate(certificateName);
                using (RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key)
                {
                    byte[] bytesData = Encoding.UTF8.GetBytes(input);
                    byte[] bytesEncrypted = csp.Encrypt(bytesData, true);
                    output = Convert.ToBase64String(bytesEncrypted);
                }
            }
            return output;
        }

        public static string decryptRsa(string certificateName, string encrypted)
        {
            string text = string.Empty;
            if (!string.IsNullOrEmpty(encrypted))
            {
                X509Certificate2 cert = getCertificate(certificateName);
                using (RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PrivateKey)
                {
                    byte[] bytesEncrypted = Convert.FromBase64String(encrypted);
                    byte[] bytesDecrypted = csp.Decrypt(bytesEncrypted, true);
                    text = Encoding.UTF8.GetString(bytesDecrypted);
                }
            }
             return text;
        }
    }
}

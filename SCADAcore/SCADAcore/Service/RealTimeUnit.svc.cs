using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.IO;
using RealTimeDriver;

namespace SCADAcore.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RealTimeUnit" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RealTimeUnit.svc or RealTimeUnit.svc.cs at the Solution Explorer and start debugging.
    public class RealTimeUnit : IRealTimeUnit
    {
        static private CspParameters csp;
        static private RSACryptoServiceProvider rsa;
        static private string PUBLIC_KEY_FILE;

        readonly object locker = new object();

        public void InitService(string keyPath)
        {
            PUBLIC_KEY_FILE = keyPath;
        }

        private static void ImportPublicKey()
        {
            FileInfo fi = new FileInfo(PUBLIC_KEY_FILE);
            if (fi.Exists)
            {
                using (StreamReader reader = new StreamReader(PUBLIC_KEY_FILE))
                {
                    csp = new CspParameters();
                    rsa = new RSACryptoServiceProvider(csp);
                    string publicKeyText = reader.ReadToEnd();
                    rsa.FromXmlString(publicKeyText);
                }
            }
        }

        private static bool VerifySignedMessage(string message, byte[] signature)
        {
            using (SHA256 sha = SHA256Managed.Create())
            {
                var hashValue = sha.ComputeHash(Encoding.UTF8.GetBytes(message));
                var deformatter = new RSAPKCS1SignatureDeformatter(rsa);
                deformatter.SetHashAlgorithm("SHA256");
                return deformatter.VerifySignature(hashValue, signature);
            }
        }

        public void SendData(string address, string data, byte[] signature)
        {
            ImportPublicKey();
            lock (locker)
            {
                if (!VerifySignedMessage(data, signature))
                {
                    Console.WriteLine("Message verification failed.");
                    return;
                }
            }
            RealTimeDriver.RealTimeDriver.WriteValue(address, Convert.ToDouble(data));
        }

        
    }
}

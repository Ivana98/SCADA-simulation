using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace RealTimeUnit
{
    class Program
    {
        static RTUServiceReference.RealTimeUnitClient service = new RTUServiceReference.RealTimeUnitClient();
        static private CspParameters csp;
        static private RSACryptoServiceProvider rsa;

        public static void CreateAsmKeys()
        {
            csp = new CspParameters();
            rsa = new RSACryptoServiceProvider(csp);
        }

        private static byte[] SignMessage(string message)
        {
            using (SHA256 sha = SHA256Managed.Create())
            {
                var hashValue = sha.ComputeHash(Encoding.UTF8.GetBytes(message));
                var formatter = new RSAPKCS1SignatureFormatter(rsa);
                formatter.SetHashAlgorithm("SHA256");
                return formatter.CreateSignature(hashValue);
            }
        }

        private static void ExportPublicKey(string PUBLIC_KEY_FILE)
        {
            using (StreamWriter writer = new StreamWriter(PUBLIC_KEY_FILE))
            {
                writer.Write(rsa.ToXmlString(false));
            }
        }

        private static void UserInput(out string id, out string address, out int lowLimit, out int highLimit)
        {
            Console.WriteLine("RTU id: ");
            id = Console.ReadLine();
            Console.WriteLine("Address: ");
            address = Console.ReadLine();
            LimitInput("Low limit: ", out lowLimit);
            LimitInput("High limit: ", out highLimit);
        }

        private static void LimitInput(string message, out int input)
        {
            while (true)
            {
                Console.WriteLine(message);
                string userInput = Console.ReadLine();
                bool success = Int32.TryParse(userInput, out input);

                if (!success)
                {
                    Console.WriteLine("Conversion failed. Must be int.");
                    continue;
                }
                return;
            }
        }

        static void Main(string[] args)
        {
            //System.AppDomain.CurrentDomain.BaseDirectory returns path = C:\ ... \SCADAcore\RealTimeUnit\bin\Debug\
            string PUBLIC_KEY_FILE = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\", "publicKey.txt"));
            //PUBLIC_KEY_FILE = C:\ ... \SCADAcore\publicKey.txt

            CreateAsmKeys();
            ExportPublicKey(PUBLIC_KEY_FILE);
            service.InitService(PUBLIC_KEY_FILE);

            //user input
            Console.WriteLine("Starting RTU configuration...");
            UserInput(out string id, out string address, out int lowLimit, out int highLimit);
            Console.WriteLine("Finished RTU configuration successfully. Started sending data.");

            //generate and send data
            while (true)
            {
                Random rand = new Random();
                int r = rand.Next(lowLimit, highLimit);
                string data = Convert.ToString(r);
                service.SendData(address, data, SignMessage(data));
                Console.WriteLine($"Sent value: {data}");
                Thread.Sleep(1000);
            }
        }

    }
}

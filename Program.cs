using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Zakres adresów IP
            string startIpAddress = "startIpAddress";
            string endIpAddress = "endIpAddress";

            // Ustawienia połączenia SSH do AP Ubiquiti
            string username = "username";
            string password = "password";

            // Iteracja przez zakres adresów IP
            IPAddress startIp = IPAddress.Parse(startIpAddress);
            IPAddress endIp = IPAddress.Parse(endIpAddress);

            IPAddress currentIp = startIp;

            while (currentIp.Address <= endIp.Address)
            {
                string currentIpAddress = currentIp.ToString();
                Console.WriteLine($"Wykonywanie restartu dla adresu IP: {currentIpAddress}");

                // Wywołanie funkcji restartującej AP dla bieżącego adresu IP
                RebootUbiquitiAP(currentIpAddress, username, password);

                // Oczekiwanie przed przejściem do następnego adresu IP
                Thread.Sleep(5000); // Dajemy czas na zakończenie restartu urządzenia

                currentIp = GetNextIpAddress(currentIp);
            }
        }
        static void RebootUbiquitiAP(string ipAddress, string username, string password)
        {
            // Tworzenie połączenia SSH
            using (var client = new SshClient(ipAddress, username, password))
            {
                try
                {
                    client.Connect();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                //Restartowanie urządzenia AP
                try
                {
                    var command = client.CreateCommand("reboot");
                    command.Execute();
                }
                catch (Renci.SshNet.Common.SshConnectionException)
                {
                    Console.WriteLine("Brak połączenia, urządzenie nie odpowiada");
                }
                // Zamykanie połączenia SSH
                client.Disconnect();
            }
        }

        static IPAddress GetNextIpAddress(IPAddress currentIpAddress)
        {
            byte[] bytes = currentIpAddress.GetAddressBytes();

            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i] < 255)
                {
                    bytes[i]++;
                    return new IPAddress(bytes);
                }
            }
            throw new ArgumentOutOfRangeException("Nie można uzyskać następnego adresu IP w zakresie.");
        }
    }
}


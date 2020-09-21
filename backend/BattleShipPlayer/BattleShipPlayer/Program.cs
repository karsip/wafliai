using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BattleShipPlayer
{
    class Program
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            Console.Title = "BattleShip Player";
            StartConnect();
            SendData();
            Console.ReadKey();
        }
        private static void SendData() 
        {
            while (true) 
            {
                Console.Write("Enter a request: ");
                string request = Console.ReadLine();
                byte[] buffer = Encoding.ASCII.GetBytes(request);

                _clientSocket.Send(buffer);
                byte[] responseBuffer = new byte[1024];
                int rec = _clientSocket.Receive(responseBuffer);

                byte[] data = new byte[rec];
                Array.Copy(responseBuffer, data, rec);
                Console.WriteLine("Received: " + Encoding.ASCII.GetString(data));
            }
        }
        private static void StartConnect() 
        {
            int attempts = 0;
            while (!_clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    _clientSocket.Connect(IPAddress.Loopback, 100);
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Connection attempts: " + attempts.ToString());
                }
            }
            Console.Clear();
            Console.WriteLine("Connected");
           
        }
    }
}

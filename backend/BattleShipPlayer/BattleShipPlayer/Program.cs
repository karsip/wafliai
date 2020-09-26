using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BattleShipPlayer
{
    class Program
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static bool connected = false;
        static void Main(string[] args)
        {
            Console.Title = "BattleShip Player";
            string username;
            Console.Write("Enter your username: ");
            username = Console.ReadLine();
            if (username != "")
            {
                StartConnect(username);
                SendData(username);
                Console.ReadKey();
            } else
            {
                Console.Write("You cant leave empty field! Username must contain at least one character");
                Console.ReadKey();
            }
        }
        private static void SendData(string username) 
        {
            while (true) 
            {
                if (connected)
                {
                    string updateUsername = "username: " + username; 
                    byte[] usernameBuffer = Encoding.ASCII.GetBytes(updateUsername);
                    _clientSocket.Send(usernameBuffer);

                    connected = false;
                }
                Console.Write("Enter a request: ");
                string request = Console.ReadLine();

                if(request.ToLower() == "end game")
                {
                    Console.WriteLine("You ended the game");
                    break;
                }
                byte[] buffer = Encoding.ASCII.GetBytes(request);


                _clientSocket.Send(buffer);
                byte[] responseBuffer = new byte[1024];
                int rec = _clientSocket.Receive(responseBuffer);

                byte[] data = new byte[rec];
                Array.Copy(responseBuffer, data, rec);
                Console.WriteLine("Received: " + Encoding.ASCII.GetString(data));
            }
        }
        private static void StartConnect(string username) 
        {
            int attempts = 0;
            while (!_clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    _clientSocket.Connect(IPAddress.Loopback, 100);

                    if (_clientSocket.Connected)
                    {
                        connected = true;
                    }
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Connection attempts: " + attempts.ToString());
                }
            }
            Console.Clear();
            Console.WriteLine(username + " is connected");
           
        }
    }
}

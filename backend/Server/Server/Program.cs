using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Socket> _clientSockets = new List<Socket>();
        private static byte[] _buffer = new byte[1024];
        static void Main(string[] args)
        {
            Console.Title = "BattleShip Server";
            SetupServer();
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            // backlog
            _serverSocket.Listen(10);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            // Accepted connection
            Socket socket = _serverSocket.EndAccept(AR);
            _clientSockets.Add(socket);
            Console.WriteLine("Client connected...");
            // begin recieve data for each client socket
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReveiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReveiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int received = socket.EndReceive(AR);
            byte[] dataBuf = new byte[received];
            Array.Copy(_buffer, dataBuf, received);

            string text = Encoding.ASCII.GetString(dataBuf);
            char[] delimiters = new char[] { ' ', '\r' };
            string output;
            byte[] data;
            int wordCount = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
            if (wordCount == 1)
            {
                switch (text.ToLower())
                {
                    case "surrender":
                        data = Encoding.ASCII.GetBytes(ConfigureReport(text));

                        break;
                    case "back":
                        data = Encoding.ASCII.GetBytes(ConfigureReport(text));

                        break;
                    default:
                        data = Encoding.ASCII.GetBytes(WrongQuery());

                        break;
                }
            }
            else if (wordCount == 2)
            {
                string[] words = text.Split(' ');
                if (words[0].ToLower() == "attack")
                {
                    if (int.TryParse(words[1], out int attack))
                    {
                        PrintActions(words[0], Int32.Parse(words[1]), "");
                        output = "PLayer has attacked opponent with damage equal to " + words[1];
                        data = Encoding.ASCII.GetBytes(output);

                    }
                    else
                    {
                        data = Encoding.ASCII.GetBytes("Wrong attack format");

                    }
                }
                else if (words[0].ToLower() == "go")
                {
                    data = Encoding.ASCII.GetBytes(PrintActions(words[0], 0, words[1]));
                }
                else
                {
                    data = Encoding.ASCII.GetBytes(WrongQuery());
                }
            }
            else
            {
                data = Encoding.ASCII.GetBytes(WrongQuery() + " and does not contain any supported input format");
            }
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReveiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
        private static string PrintActions(string text, int number, string move)
        {
            switch (text.ToLower())
            {
                case "attack":
                    return "Player inflicted " + number.ToString() + " damage to opponent";
                default:
                    return "Player moved to " + move + " position";
            }

        }
        private static string WrongQuery()
        {
            return "Bad request";
        }
        private static string ConfigureReport(string text)
        {
            return "Client asked to " + text;
        }
    }
}

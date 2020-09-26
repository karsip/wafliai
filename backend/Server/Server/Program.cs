using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Server
{
    class Program
    {
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Socket> _clientSockets = new List<Socket>();
        private static List<Player> _gamePlayerList = new List<Player>();
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
            _gamePlayerList.Add(new Player(socket));


            // begin recieve data for each client socket
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReveiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReveiveCallback(IAsyncResult AR)
        {
            try
            {
                Socket socket = (Socket)AR.AsyncState;
                int received = socket.EndReceive(AR);
                byte[] dataBuf = new byte[received];
                Array.Copy(_buffer, dataBuf, received);

                string text = Encoding.ASCII.GetString(dataBuf);
                if (text.Contains("username: "))
                {
                    try
                    {
                        Player player = _gamePlayerList.First(x => x.getSocket() == socket);
                        string username = text.Replace("username: ", "");
                        if (player == null)
                        {
                            Console.WriteLine("Couldn t find player with socket that matches username - " + username);
                        }
                        else
                        {
                            Console.WriteLine(username + " connected...");
                            player.SetUsername(username);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                string userNameToShow = "undefined";
                Player playerToShow = _gamePlayerList.First(x => x.getSocket() == socket);
                if(playerToShow != null)
                {
                    userNameToShow = playerToShow.getUsername();
                }
                char[] delimiters = new char[] { ' ', '\r' };
                string output;
                byte[] data;
                int wordCount = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
                if (wordCount == 1)
                {
                    switch (text.ToLower())
                    {
                        case "surrender":
                            data = Encoding.ASCII.GetBytes(ConfigureReport(text, userNameToShow));

                            break;
                        case "back":
                            data = Encoding.ASCII.GetBytes(ConfigureReport(text, userNameToShow));

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
                            PrintActions(words[0], Int32.Parse(words[1]), "", userNameToShow);
                            output = userNameToShow + " has attacked opponent with damage equal to " + words[1];
                            data = Encoding.ASCII.GetBytes(output);

                        }
                        else
                        {
                            data = Encoding.ASCII.GetBytes("Wrong attack format");

                        }
                    }
                    else if (words[0].ToLower() == "go")
                    {
                        data = Encoding.ASCII.GetBytes(PrintActions(words[0], 0, words[1], userNameToShow));
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
        private static string PrintActions(string text, int number, string move, string username)
        {
            switch (text.ToLower())
            {
                case "attack":
                    return username + " inflicted " + number.ToString() + " damage to opponent";
                default:
                    return username + " moved to " + move + " position";
            }

        }
        private static string WrongQuery()
        {
            return "Bad request";
        }
        private static string ConfigureReport(string text, string username)
        {
            return username + "asked to " + text;
        }
    }

    public class Player
    {
        private string username;
        private Socket socket;

        public Player(string username, Socket socket)
        {
            this.username = username;
            this.socket = socket;
        }

        public Player(string username)
        {
            this.username = username;
        }
        public Player(Socket socket)
        {
            this.socket = socket;
        }
        public string getUsername()
        {
            return this.username;
        }
        public Socket getSocket()
        {
            return this.socket;
        }
        public void SetUsername(string username)
        {
            this.username = username;
        }
    }
}

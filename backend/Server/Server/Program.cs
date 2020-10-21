using GameModels;
using GameModels.Singleton;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server
{
    class Program
    {
        private static ILogger _logger;
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Socket> _clientSockets = new List<Socket>();
        private static List<PlayerData> _gamePlayerList = new List<PlayerData>();
        private static byte[] _buffer = new byte[10000];
        // array to update after each object is placed in map.
        private static int[,] unitArray = new int[64, 64];
        private static bool unitMapSendFirst = false;
        private static MapCell[][] gameCells;


        static void Main(string[] args)
        {
            Map gameMap = new Map();
            gameCells = gameMap.GetMapObjects();

            Console.Title = "BattleShip Server";
            SetupServer();
            Console.ReadLine();
        }

        private static void updateUnitArray(int row, int column, int object_id, int rows, int columns)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    unitArray[i, j] = object_id;
                }
            }
            PrintArray();
        }
        private static void PrintArray()
        {
            for (var i = 0; i < unitArray.GetLength(0); i++)
            {
                for (var j = 0; j < unitArray.GetLength(1); j++)
                {
                    Console.Write(unitArray[i, j]);
                }
                Console.WriteLine();
            }
        }
        private static void SetupServer()
        {
            _logger = Logger.GetInstance;
            Console.WriteLine("Setting up server...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            // backlog
            _serverSocket.Listen(10);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            //Console.WriteLine("File path " + Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName));
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            // Accepted connection
            Socket socket = _serverSocket.EndAccept(AR);
            _clientSockets.Add(socket);
            PlayerData playerToAdd = new PlayerData(socket);
            _gamePlayerList.Add(playerToAdd);

            if (unitMapSendFirst == false)
            {
                unitArray[0, 0] = 1;
                unitArray[0, 1] = 2;
                unitArray[0, 20] = 3;
                string arrayToString = string.Join(',', unitArray.Cast<int>());
                var data = Encoding.ASCII.GetBytes(arrayToString);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                //socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReveiveCallback), socket);
                // _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                unitMapSendFirst = true;
                Array.Clear(_buffer, 0, _buffer.Length);
            } else
            {
                // begin recieve data for each client socket
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReveiveCallback), socket);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }            
        }

        private static void ReveiveCallback(IAsyncResult AR)
        {
            try
            {
                Socket socket = (Socket)AR.AsyncState;
                int received = socket.EndReceive(AR);
                Console.WriteLine("received data -------> " + received);
                byte[] dataBuf = new byte[received];
                Array.Copy(_buffer, dataBuf, received);
                Func<PlayerData, bool> predicate = x => x.socket == socket;
                PlayerData player = _gamePlayerList.First(predicate);
                string text = Encoding.ASCII.GetString(dataBuf);
                if (text.Contains("username: "))
                {
                    try
                    {
                        string username = text.Replace("username: ", "");
                        if (player == null)
                        {
                            Console.WriteLine("Couldn't find player with socket that matches username - " + username);
                        }
                        else
                        {
                            Console.WriteLine(username + " connected...");
                            player.username = username;
                        }

                    }
                    catch (Exception e)
                    {
                        _logger.LogException(e.Message);
                    }
                }
                string userNameToShow = "undefined";
                for (int i = 0; i < player.unitCount.Length; i++)
                {
                    Console.WriteLine("Unit count objects " + player.unitCount[i].ToString());
                }
                if (player != null)
                {
                    userNameToShow = player.username;
                }
                char[] delimiters = new char[] { ' ', '\r' };
                string output;
                int n;
                byte[] data;
                Console.WriteLine("Text -----------------> " + text);
                if (int.TryParse(text, out n))
                {
                    Console.WriteLine("Yes it issssss");
                }
                int wordCount = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
                if (wordCount == 1)
                {
                    switch (text.ToLower())
                    {
                        case "map":
                            Map gameMap = new Map();
                            string val = JsonConvert.SerializeObject(gameMap.GetMapObjects(),
                                        typeof(MapCell), new JsonSerializerSettings
                                        { TypeNameHandling = TypeNameHandling.Auto });
                            data = Encoding.ASCII.GetBytes(val);
                            break;
                        case "surrender":
                            data = Encoding.ASCII.GetBytes(ConfigureReport(text, userNameToShow));

                            break;
                        case "back":
                            data = Encoding.ASCII.GetBytes(ConfigureReport(text, userNameToShow));
                            break;
                        case "start":
                            var playerData = new PlayerData(player.unitCount, player.livePoints, player.isYourTurn, player.objectLocations, player.isLost, player.username);
                            string player_string = JsonConvert.SerializeObject(playerData,
                                        typeof(PlayerData), new JsonSerializerSettings
                                        { TypeNameHandling = TypeNameHandling.Auto });
                            data = Encoding.ASCII.GetBytes(player_string);
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
                String message = String.Format("Exception occurred while receiving callback -  {0}", e.Message);
                _logger.LogException(message);
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
}

using GameModels;
using GameModels.RequestAdapter;
using GameModels.Singleton;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using GameModels.Iterator;
namespace Server
{
    class Program
    {
        private static ILogger _logger;
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Socket> _clientSockets = new List<Socket>();
        private static List<PlayerData> _gamePlayerList = new List<PlayerData>();
        private static byte[] _buffer = new byte[10000];
        private static int[,] unitArray = new int[64, 64];
        static void Main(string[] args)
        {
            Console.Title = "BattleShip Server";
            SetupServer();
            Console.ReadLine();
        }
        /*
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
        */
        private static void SetupServer()
        {
            _logger = Logger.GetInstance;
            Console.WriteLine("Setting up server...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));

            // backlog
            _serverSocket.Listen(10);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = _serverSocket.EndAccept(AR);
            _clientSockets.Add(socket);
            PlayerData playerToAdd = new PlayerData(socket);

            _gamePlayerList.Add(playerToAdd);
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
                byte[] data;

                // Adapter
                Adaptee adaptee = new Adaptee(dataBuf);
                IAdapterInrterface target = new RequestAdapter(adaptee);
                int[,] queryArr = target.getRequest();
                // ----------------------------------------------------------
                if (queryArr[0, 0] != -1)
                {
                    UpdateUnitMap(unitArray, queryArr);
                    data = Encoding.ASCII.GetBytes("Map updated");
                    Print2DArray();
                } else
                {
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
                    if (player != null)
                    {
                        userNameToShow = player.username;
                    }
                    char[] delimiters = new char[] { ' ', '\r' };
                    string output;
                    int wordCount = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (wordCount == 1)
                    {

                        switch (text.ToLower())
                        {
                            case "map":
                                Map gameMap = new Map();
                                string val = JsonConvert.SerializeObject(gameMap.GetMapObjects(),
                                            typeof(Matrix), new JsonSerializerSettings
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
                            case "reload":
                                string arrayToString = string.Join(',', unitArray.Cast<int>());
                                data = Encoding.ASCII.GetBytes(arrayToString);
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
        private static void Print2DArray()
        {
            string line = new string('-', 70);
            Console.WriteLine(line);
            for (int i = 0; i < unitArray.GetLength(0); i++)
            {
                for (int j = 0; j < unitArray.GetLength(1); j++)
                {
                    Console.Write(unitArray[i, j]);
                }
                Console.WriteLine();
            }
        }
        private static int[,] StringTo2DArray(string query)
        {
            int[] array = query.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            int[,] arrayToReturn = new int[64, 64];
            int counter = 0;
            for (int i = 0; i < Math.Sqrt(array.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(array.Length); j++)
                {
                    arrayToReturn[i, j] = array[counter];
                    counter++;
                }
            }
            return arrayToReturn;
        }
        private static void UpdateUnitMap(int [,] unitMap, int [,] updateMap)
        {
            for(int i = 0; i < unitMap.GetLength(0); i++)
            {
                for (int j = 0; j < unitMap.GetLength(1); j++)
                {
                    if(updateMap[i, j] != unitMap[i,j] && updateMap[i, j] != -1)
                    {
                        unitMap[i, j] = updateMap[i, j];
                    } 
                    if(updateMap[i, j] == -1)
                    {
                        unitMap[i, j] = 0;
                    }
                }
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

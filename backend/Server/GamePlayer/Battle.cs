using GameModels;
using GameModels.GroundTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GamePlayer
{
    public partial class Battle : Form
    {
        private static Socket _clientSocket;
        Int32 rows = 64;
        Int32 columns = 64;
        int squareSize = 25;
        private int[,,] gameArr = new int[64,64,3];

        private MapCell[][] map;

        Image img = null;
        Graphics imgGraph = null;
        Graphics graph = null;

        private void FillArr()
        {
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    gameArr[i, j, 0] = i;
                    gameArr[i, j, 1] = j;
                    gameArr[i, j, 2] = 0;
                }
            }
        }
        private void SendData(string request)
        {
            if (request.ToLower() == "end game")
            {
                MessageBox.Show("You ended the game", "Game status",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            byte[] buffer = Encoding.ASCII.GetBytes(request);


             _clientSocket.Send(buffer);
            byte[] responseBuffer = new byte[1024];
            int rec = _clientSocket.Receive(responseBuffer);

            byte[] data = new byte[rec];
            Array.Copy(responseBuffer, data, rec);
            Console.WriteLine("Received: " + Encoding.ASCII.GetString(data));
            MessageBox.Show("Received: " + Encoding.ASCII.GetString(data), "Recieve info",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public Battle(string username, Socket socket)
        {
            InitializeComponent();
            this.AutoScroll = true;
            grid.BorderStyle = BorderStyle.FixedSingle;

            img = new Bitmap(squareSize * rows, squareSize * columns);
            imgGraph = Graphics.FromImage(img);
            graph = grid.CreateGraphics();

            FillArr();
            
            this.username.Text = username;
            _clientSocket = socket;
        }
        private void handleRequest(string request)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(request);
            _clientSocket.Send(buffer);

            byte[] responseBuffer = new byte[300000];
            int rec = _clientSocket.Receive(responseBuffer);

            byte[] data = new byte[rec];
            Array.Copy(responseBuffer, data, rec);
            
            Console.WriteLine("Full encoded data", Encoding.ASCII.GetString(data));
            var mapString = System.Text.Encoding.Default.GetString(data);
            Console.WriteLine(mapString);
            map = JsonConvert.DeserializeObject<MapCell[][]>(mapString, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            DrawMap();
        }
       private void DrawMap()
       {
            var gridBrush = new SolidBrush(Color.Blue);
            Pen gridPen = new Pen(gridBrush);

            // vertical Lines 
            for (int i = 0; i < rows; i++)
            {
                graph.DrawLine(gridPen, 0, i * squareSize, squareSize * rows, i * squareSize);
            }
            for (int j = 0; j < columns; j++)
            {
                graph.DrawLine(gridPen, j * squareSize, 0, j * squareSize, squareSize * columns);
            }

            if (map == null) return;

            //Draw
            var desertTexture = new Bitmap("../../../GameModels/Textures/sandTile.png");
            TextureBrush desertBrush = new TextureBrush(desertTexture);
            var grassTexture = new Bitmap("../../../GameModels/Textures/grassTile.png");
            TextureBrush grassBrush = new TextureBrush(grassTexture);
            var waterTexture = new Bitmap("../../../GameModels/Textures/waterTile.png");
            TextureBrush watertBrush = new TextureBrush(waterTexture);
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if(map[i][j].mapObject is Sand)
                    {
                        map[i][j].mapObject.Draw(desertBrush, i * squareSize, j * squareSize, squareSize - 1, squareSize - 1, imgGraph);
                    }
                    else if(map[i][j].mapObject is Grass)
                    {
                        map[i][j].mapObject.Draw(grassBrush, i * squareSize, j * squareSize, squareSize - 1, squareSize - 1, imgGraph);
                    }
                    else
                    {
                        map[i][j].mapObject.Draw(watertBrush, i * squareSize, j * squareSize, squareSize - 1, squareSize - 1, imgGraph);
                    }
                }
            }

            graph.DrawImage(img, 0, 0);

        }

        private void Battle_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DrawMap();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            handleRequest("map");
        }

        private void grid_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

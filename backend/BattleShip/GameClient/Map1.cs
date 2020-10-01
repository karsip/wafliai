using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleShipModels;
using BattleShipModels.GroundTypes;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace GameClient
{
    public partial class Map1 : Form
    {
        readonly Int32 gridColNum = 64;
        readonly Int32 gridRowNum = 64;
        readonly int gridSquareSize = 25;
        bool IsGameOver = true;
        private HubConnection hubConnection;

        Image img = null;
        Graphics imgGraph = null;
        Graphics graph = null;

        private MapCell[][] map;
        private MapCell[][] generatedmap;

        private void generateMap()
        {
            var groundFactory = new GroundFactory();

            Random rnd = new Random();
            generatedmap = new MapCell[64][];
            for (int i = 0; i < 64; i++)
            {
                generatedmap[i] = new MapCell[64];
                for (int j = 0; j < 64; j++)
                {
                    generatedmap[i][j] = new MapCell();
                }            
            }

            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    int groundType = rnd.Next(1, 4);
                    generatedmap[i][j].mapObject = groundFactory.CreateGround(groundType);
                }
            }
    }
        public Map1()
        {
            InitializeComponent();
            button1.Enabled = true;
            // InitializeConnection();
            this.screen.Visible = false;

            img = new Bitmap(25 * 64, 25 * 64);
            imgGraph = Graphics.FromImage(img);
            graph = screen.CreateGraphics();

            generateMap();
        }
        public void MapUpdate(string serializedWordFromServer)
        {
            var mapFromServer = JsonConvert.DeserializeObject<MapCell[][]>(serializedWordFromServer, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            map = mapFromServer;
            Console.WriteLine("Map object", map[5][5].mapObject);
            Draw();
        }

        private void Draw()
        {
            Console.WriteLine("Painting map " + generatedmap[1][63].mapObject + " length " + generatedmap.GetLength(0));
            imgGraph.FillRectangle(new SolidBrush(Color.White), 0, 0, 25 * 64, 25 * 64);

            var gridBrush = new SolidBrush(Color.Blue);
            var gridPen = new Pen(gridBrush);

            for (int i = 1; i < 64; ++i)
            {
                imgGraph.DrawLine(gridPen, 0, i * 25, 25 * 64, i * 25);
            }
                
            for (int i = 1; i < 64; ++i)
            {
                imgGraph.DrawLine(gridPen, i * 25, 0, i * 25, 25 * 64);
            }


            if (generatedmap == null) return;

            Random rnd = new Random();

            var waterColor = new SolidBrush(Color.Blue);
            var earthColor = new SolidBrush(Color.Brown);
            var obstacleColor = new SolidBrush(Color.Gray);
            var grassColor = new SolidBrush(Color.Green);

            for (int i = 0; i < generatedmap.GetLength(0); i++)
            {
                for (int j = 0; j < generatedmap[i].Length; j++)
                {
                    if (generatedmap[i][j].mapObject is Water)
                    {
                        generatedmap[i][j].mapObject.Draw(waterColor, i * gridSquareSize, j * gridSquareSize, gridSquareSize - 1, gridSquareSize - 1, imgGraph);
                    }
                    else if (generatedmap[i][j].mapObject is Grass)
                    {
                        generatedmap[i][j].mapObject.Draw(grassColor, i * gridSquareSize, j * gridSquareSize, gridSquareSize - 1, gridSquareSize - 1, imgGraph);
                    }
                    else if (generatedmap[i][j].mapObject is Earth)
                    {
                        generatedmap[i][j].mapObject.Draw(earthColor, i * gridSquareSize, j * gridSquareSize, gridSquareSize - 1, gridSquareSize - 1, imgGraph);
                    } else
                        imgGraph.FillRectangle(obstacleColor, i * gridSquareSize, j * gridSquareSize, gridSquareSize - 1, gridSquareSize - 1);
                }
            }
            graph.DrawImage(img, 0, 0);
        }
        private void ChangeGameState()
        {
            IsGameOver = !IsGameOver;

            this.screen.Visible = !IsGameOver;
            this.button1.Visible = IsGameOver;
            Draw();
        }
        private async Task InitializeConnection()
        {
            hubConnection = new HubConnectionBuilder()
                            .WithUrl("https://localhost:5000/hub/")
                            .Build();
            hubConnection.On<string>("MapUpdate", MapUpdate);

            await hubConnection.StartAsync();
            button1.Enabled = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (hubConnection != null)
            {
                if (keyData == Keys.A || keyData == Keys.S || keyData == Keys.D || keyData == Keys.W)
                {
                    hubConnection.SendAsync("HandleMapClick", keyData.ToString());
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Map1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeGameState();
        }
    }
}

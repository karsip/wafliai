using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
        Int32 gridColNum = 64;
        Int32 gridRowNum = 64;
        int gridSquareSize = 25;

        Image img = null;
        Graphics imgGraph = null;
        Graphics graph = null;

        private MapCell[][] map;

        private HubConnection hubConnection;
        public Map1()
        {
            InitializeComponent();

            _ = InitConnection();

            img = new Bitmap(25 * 64, 25 * 64);
            imgGraph = Graphics.FromImage(img);
            graph = screen.CreateGraphics();
        }
        public void StateUpdated(string serializedWordFromServer)
        {
            var worldFromServer = JsonConvert.DeserializeObject<MapCell[][]>(serializedWordFromServer, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            map = worldFromServer;
            Draw();
        }

        private void Draw()
        {
            imgGraph.FillRectangle(new SolidBrush(Color.White), 0, 0, gridSquareSize * gridColNum, gridSquareSize * gridRowNum);

            var gridBrush = new SolidBrush(Color.LightGray);
            var gridPen = new Pen(gridBrush);

            for (int i = 1; i < gridColNum; ++i)
                imgGraph.DrawLine(gridPen, 0, i * gridSquareSize, gridSquareSize * gridColNum, i * gridSquareSize);

            for (int i = 1; i < gridColNum; ++i)
                imgGraph.DrawLine(gridPen, i * gridSquareSize, 0, i * gridSquareSize, gridSquareSize * gridRowNum);

            if (map == null) return;


            Random rnd = new Random();

            var waterColor = new SolidBrush(Color.Blue);
            var earthColor = new SolidBrush(Color.Brown);
            var obstacleColor = new SolidBrush(Color.Gray);
            var grassColor = new SolidBrush(Color.Green);

            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    if (map[i][j].mapObject is Water)
                    {
                        imgGraph.FillRectangle(waterColor, i * gridSquareSize, j * gridSquareSize, gridSquareSize - 1, gridSquareSize - 1);
                    }
                    else if (map[i][j].mapObject is Grass)
                    {
                        imgGraph.FillRectangle(grassColor, i * gridSquareSize, j * gridSquareSize, gridSquareSize - 1, gridSquareSize - 1);
                    }
                    else if (map[i][j].mapObject is Earth)
                    {
                        imgGraph.FillRectangle(earthColor, i * gridSquareSize, j * gridSquareSize, gridSquareSize - 1, gridSquareSize - 1);
                    }
                    else
                    {
                        imgGraph.FillRectangle(obstacleColor, i * gridSquareSize, j * gridSquareSize, gridSquareSize - 1, gridSquareSize - 1);
                    }

                }
            }

            graph.DrawImage(img, 0, 0);
        }
        private async Task InitConnection()
        {
            //WithUrl not working
            hubConnection = new HubConnectionBuilder()
                .Build();
            hubConnection.On<string>("StateUpdate", StateUpdated);

            await hubConnection.StartAsync();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (hubConnection != null)
            {
                if (keyData == Keys.A || keyData == Keys.S || keyData == Keys.D || keyData == Keys.W)
                {
                    hubConnection.SendAsync("HandleClick", keyData.ToString());
                    return true;
                }
                else if (keyData == Keys.Space)
                {
                    //put a bomb
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
            this.screen.Visible = true;
            Draw();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}

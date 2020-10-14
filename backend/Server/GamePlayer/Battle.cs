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
        private int clickedObject = 2;

        int ship1 = 1;
        int ship2 = 2;
        int ship3 = 3;
        int plane = 2;
        int soldier = 3;
        int mine = 2;

        private MapCell[][] map;
        private PlayerData playerDataOnStart;

        private void renderLabels()
        {
            Random rnd = new Random();
            var lableArray = new Label[64, 64];
            for (int i = 0; i < lableArray.GetLength(0); i++)
            {
                for (int j = 0; j < lableArray.GetLength(1); j++)
                {

                    var label_to_add = new Label();
                    label_to_add.Size = new Size(25, 25);
                    label_to_add.Top = i * 25;
                    label_to_add.Left = j * 25;
                    label_to_add.Margin = new Padding(0);
                    label_to_add.BorderStyle = BorderStyle.FixedSingle;

                    label_to_add.AutoSize = false;

                    label_to_add.Click += new EventHandler(HandleClickLabel);

                    if (map[i][j].mapObject is Sand)
                    {
                        label_to_add.BackColor = Color.SandyBrown;
                        label_to_add.Image = Image.FromFile("../../../GameModels/Textures/sandTile.png");
                    }
                    else if (map[i][j].mapObject is Grass)
                    {
                        label_to_add.BackColor = Color.LawnGreen;
                        label_to_add.Image = Image.FromFile("../../../GameModels/Textures/grassTile.png");
                    }
                    else
                    {
                        label_to_add.BackColor = Color.DarkCyan;
                        label_to_add.Image = Image.FromFile("../../../GameModels/Textures/waterTile.png");
                    }
                    flowLayoutPanel2.Controls.Add(label_to_add);
                }
            }
        }
        protected void HandleClickLabel(object sender, EventArgs e)
        {
            Label button = sender as Label;

            int row = button.Top;
            int column = button.Left;
            UpdateMap(row, column);
        }
        private void UpdateMap(int row, int column)
        {
            switch (clickedObject)
            {
                case 1:
                    // render plane
                    renderObject(column, row, 3, 2, clickedObject);
                    break;
                case 2:
                    renderObject(column, row, 2, 4, 2);
                    break;
                case 3:
                    renderObject(column, row, 1, 4, 3);
                    break;
                case 4:
                    renderObject(column, row, 1, 5, 4);
                    break;
                case 5:
                    renderObject(column, row, 1, 2, 5);
                    break;
                default:
                    break;

            }
        }
        private void renderObject(int column, int row, int columnNumber, int rowNumber, int object_id)
        {
            int counter = 1;
            bool badPosition = false;
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    Point myPoint = new Point((column + (25 * j)), (row + 25 * i));
                    Label update_label = flowLayoutPanel2.GetChildAtPoint(myPoint) as Label;
                    switch (object_id)
                    {
                        // aircrafts 2, ships - 3, mine - 1, soldier: 1
                        case 1:
                            // plane
                            update_label.BorderStyle = BorderStyle.None;
                            update_label.Image = Image.FromFile("../../../GameModels/Textures/plane/plane" + counter.ToString() + ".png");
                            break;
                        case 2:
                            // shipcarrier
                            if (row / 25 >= 20 && row / 25 < 42)
                            {
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/shipcarrier/shipcarrier" + counter.ToString() + ".png");
                            } else
                            {
                                MessageBox.Show("You can place this ship where the water is!", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                            break;
                        case 3:
                            // shipdestroyer
                            if (row / 25 >= 20 && row / 25 < 42)
                            {
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/shipdestroyer/shipdestroyer" + counter.ToString() + ".png");
                            }else
                            {
                                MessageBox.Show("You can place this ship where the water is!", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                            
                            break;
                        case 4:
                            // submarine
                            if (row / 25 >= 20 && row / 25 < 42)
                            {
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/submarine/submarine" + counter.ToString() + ".png");
                            } else
                            {
                                MessageBox.Show("You can place this ship where the water is!", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                            break;
                        case 5:
                            // soldier
                            if (row / 25 < 20 || row / 25 >= 42)
                            {
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/soldier/soldier" + counter.ToString() + ".png");
                            } else {
                                MessageBox.Show("Soldiers cannot be placed in the water. They Will drawn!", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                            break;
                             
                    }
                    counter++;
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
            label1.Text = "Left: " + ship1.ToString();
            label2.Text = "Left: " + ship2.ToString();
            label3.Text = "Left: " + ship3.ToString();
            label4.Text = "Left: " + plane.ToString();
            label5.Text = "Left: " + soldier.ToString();
            label6.Text = "Left: " + mine.ToString();
            this.AutoScroll = true;
            flowLayoutPanel2.Size = new Size(25 * 64, 25 * 64);



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
            switch (request)
            {
                case "map":
                    var mapString = System.Text.Encoding.Default.GetString(data);
                    Console.WriteLine("MapString:    " + mapString);
                    map = JsonConvert.DeserializeObject<MapCell[][]>(mapString, new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    Array.Clear(responseBuffer, 0, responseBuffer.Length);
                    Array.Clear(data, 0, data.Length); ;
                    break;
                case "playerData":
                    var playerDataString = System.Text.Encoding.Default.GetString(data);
                    Console.WriteLine("Player String" + playerDataString);
                    playerDataOnStart = JsonConvert.DeserializeObject<PlayerData>(playerDataString, new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    break;

            }

        }
        private void Battle_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // renderLabels();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            handleRequest("map");
            renderLabels();
        }

        private void grid_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ship1--;
            label1.Text = "Left: " + ship1.ToString();
            handleRequest("playerData");
            if (ship1 <= 0)
            {
                button3.Enabled = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ship2--;
            label2.Text = "Left: " + ship2.ToString();
            handleRequest("playerData");
            if (ship2 <= 0)
            {
                button4.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ship3--;
            label3.Text = "Left: " + ship3.ToString();
            handleRequest("playerData");
            if (ship3 <= 0)
            {
                button5.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            plane--;
            label4.Text = "Left: " + plane.ToString();
            handleRequest("playerData");
            if (plane <= 0)
            {
                button6.Enabled = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            soldier--;
            label5.Text = "Left: " + soldier.ToString();
            handleRequest("playerData");
            if (soldier <= 0) 
            {
                button7.Enabled = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            mine--;
            label6.Text = "Left: " + mine.ToString();
            handleRequest("playerData");
            if (mine <= 0)
            {
                button8.Enabled = false;
            }
        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

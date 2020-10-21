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
using GameModels.AirCraftTypes;
using GameModels.Checker;
using System.CodeDom;

namespace GamePlayer
{
    public partial class Battle : Form
    {
        private static Socket _clientSocket;
        private int clickedObject = 0;
        int[,] unitMap = new int[64, 64];

        int shipCarrier = 2;
        int shipDestroyer = 2;
        int submarine = 3;
        int plane = 2;
        int jet = 2;
        int soldier = 3;
        int mine = 5;

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
            clickedObject = 0;
        }
        private void UpdateMap(int row, int column)
        {
            switch (clickedObject)
            {
                case 1:
                    // render shipCarrier
                    renderObject(column, row, 2, 4, clickedObject);
                    break;
                case 2:
                    renderObject(column, row, 1, 4, clickedObject);
                    break;
                case 3:
                    renderObject(column, row, 1, 5, clickedObject);
                    break;
                case 4:
                    renderObject(column, row, 3, 2, clickedObject);
                    break;
                case 5:
                    renderObject(column, row, 2, 2, clickedObject);
                    break;
                case 6:
                    renderObject(column, row, 1, 2, clickedObject);
                    break;
                case 7:
                    renderObject(column, row, 1, 1, clickedObject);
                    break;
                default:
                    break;
            }
        }
        private void renderObject(int column, int row, int columnNumber, int rowNumber, int object_id)
        {
            AirCraftBuilder builder;
            AirCraftDirector airCraftDirector = new AirCraftDirector();
            builder = new JetBuilder();
            airCraftDirector.Construct(builder);
            Image[] jetImg = builder.AirCraft.ForMap();
            builder = new PlaneBuilder();
            airCraftDirector.Construct(builder);
            Image[] planeImg = builder.AirCraft.ForMap();
            CanBePlaced canBePlaced = new CanBePlaced();
            bool eligible = canBePlaced.IsEligible(unitMap, (column / 25), (row / 25), rowNumber, columnNumber, object_id);
            int counter = 1;
            if (eligible)
            {
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
                                // shipcarrier
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/shipcarrier/shipcarrier" + counter.ToString() + ".png");
                                unitMap[myPoint.Y / 25, myPoint.X / 25] = 1;
                                if (i + j == 0)
                                {
                                    shipCarrier--;
                                    label1.Text = "Left: " + shipCarrier.ToString();
                                    
                                }
                                break;
                            case 2:
                                // shipdestroyer
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/shipdestroyer/shipdestroyer" + counter.ToString() + ".png");
                                unitMap[myPoint.Y / 25, myPoint.X / 25] = 2;
                                if (i + j == 0)
                                {
                                    shipDestroyer--;
                                    label2.Text = "Left: " + shipDestroyer.ToString();
                                    
                                }

                                break;
                            case 3:
                                // submarine
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/submarine/submarine" + counter.ToString() + ".png");
                                unitMap[myPoint.Y / 25, myPoint.X / 25] = 3;
                                if (i + j == 0)
                                {
                                    submarine--;
                                    label3.Text = "Left: " + submarine.ToString();
                                   
                                }
                                break;
                            case 4:
                                // plane
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = planeImg[counter - 1];
                                unitMap[myPoint.Y / 25, myPoint.X / 25] = 4;
                                if (i + j == 0)
                                {
                                    plane--;
                                    label4.Text = "Left: " + plane.ToString();
                                    
                                }
                                break;
                            case 5:
                                // jet
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = jetImg[counter - 1];
                                unitMap[myPoint.Y / 25, myPoint.X / 25] = 5;
                                if (i + j == 0)
                                {
                                    jet--;
                                    label5.Text = "Left: " + jet.ToString();
                                    
                                }
                                break;
                            case 6:
                                // soldier
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/soldier/soldier" + counter.ToString() + ".png");
                                unitMap[myPoint.Y / 25, myPoint.X / 25] = 6;
                                if (i + j == 0)
                                {
                                    soldier--;
                                    label6.Text = "Left: " + soldier.ToString();
                                    
                                }
                                break;
                            case 7:
                                //mine
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/mine.png");
                                unitMap[myPoint.Y / 25, myPoint.X / 25] = 7;
                                if (i + j == 0)
                                {
                                    mine--;
                                    label7.Text = "Left: " + mine.ToString();
                                }
                                break;
                            default:
                                break;

                        }
                        counter++;
                    }
                }
            }
            else
            {
                MessageBox.Show("You can't place object here", "Game error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            label1.Text = "Left: " + shipCarrier.ToString();
            label2.Text = "Left: " + shipDestroyer.ToString();
            label3.Text = "Left: " + submarine.ToString();
            label4.Text = "Left: " + plane.ToString();
            label5.Text = "Left: " + jet.ToString();
            label6.Text = "Left: " + soldier.ToString();
            label7.Text = "Left: " + mine.ToString();

            //flowLayoutPanel2.Size = new Size(1239, 64 * 25);
            this.AutoScroll = true;
            this.username.Text = "User" + username;
            this.lifepoints.Text = "LifePoints: " + 19.ToString();
            _clientSocket = socket;
            handleRequest("username: " + username);
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
            if(request.Length < 10)
            {
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
                        Array.Clear(data, 0, data.Length);
                        break;
                    case "start":
                        var playerDataString = System.Text.Encoding.Default.GetString(data);
                        Console.WriteLine("Player String " + playerDataString);
                        playerDataOnStart = JsonConvert.DeserializeObject<PlayerData>(playerDataString, new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        });
                        Console.WriteLine("player data  " + playerDataOnStart.ToString());
                        break;
                    default:
                        Console.WriteLine("Wrong request.");
                        break;
                }
            }
            else
            {
                Console.WriteLine(System.Text.Encoding.Default.GetString(data));
            }       
        }
        private void Battle_Load(object sender, EventArgs e)
        {
            AirCraftBuilder builder;
            AirCraftDirector airCraftDirector = new AirCraftDirector();
            builder = new JetBuilder();
            airCraftDirector.Construct(builder);
            Image jetImg = builder.AirCraft.Show();
            button7.BackgroundImage = jetImg;
            builder = new PlaneBuilder();
            airCraftDirector.Construct(builder);
            Image planeImg = builder.AirCraft.Show();
            button6.BackgroundImage = planeImg;

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
            clickedObject = 1;
            handleRequest("start");
            if (shipCarrier <= 1)
            {
                button3.Enabled = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            clickedObject = 2;
            handleRequest("start");
            if (shipDestroyer <= 1)
            {
                button4.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            clickedObject = 3;
            handleRequest("start");
            if (submarine <= 1)
            {
                button5.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var lableArray = new Label[64, 64];
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    Console.Write(map[i][j].ToString());
                }
                Console.WriteLine();
            }
            clickedObject = 4;
            handleRequest("start");
            if (plane <= 1)
            {
                button6.Enabled = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            clickedObject = 5;
            handleRequest("start");
            if (jet <= 1)
            {
                button7.Enabled = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            clickedObject = 6;
            handleRequest("start");
            if (soldier <= 1)
            {
                button8.Enabled = false;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            clickedObject = 7;
            handleRequest("start");
            if (mine <= 1)
            {
                button9.Enabled = false;
            }
        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void username_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

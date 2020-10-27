using GameModels;
using GameModels.AirCraftTypes;
using GameModels.Checker;
using GameModels.Command;
using GameModels.Mine_explosion;
using GameModels.GroundTypes;
using GameModels.Singleton;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace GamePlayer
{
    public partial class Battle : Form
    {
        private const char V = ',';
        private static ILogger _logger;
        private static Socket _clientSocket;
        private int clickedObject = 0;
        private bool isInCreationStage = false;
        private bool objectChecked = false;
        private int currentSelectedObject = 0;
        private int x_loc;
        private int y_loc;
        private int[,] pointsArr = new int[19, 2];

        private int prev_x_loc;
        private int prev_y_loc;

        private static int[,] unitMap = new int[64, 64];
        private int[,] myUnits = new int[64, 64];
        private int[,] currentSelected;

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

            bool isOK = true;
            if (objectChecked && currentSelectedObject != -1)
            {
                int column_number = ReturnbjectColumnNumber(currentSelectedObject);
                int row_number = ReturnObjectRowNumber(currentSelectedObject);
                CanBePlaced canBePlaced = new CanBePlaced();
                bool eligible = canBePlaced.IsEligible(unitMap, (column / 25), (row / 25), column_number, row_number, currentSelectedObject);
                if (eligible)
                {
                    x_loc = row / 25;
                    y_loc = column / 25;
                    renderObject(column, row, column_number, row_number, currentSelectedObject);
                    RenderGroundAfterChange();
                    // Command design pattern for player map and server map
                    MoveReceiver receiver = new MoveReceiver(myUnits, currentSelectedObject, row / 25, column / 25, currentSelected);
                    MoveReceiver server_receiver = new MoveReceiver(unitMap, currentSelectedObject, row / 25, column / 25, currentSelected);
                    MoveCommand command = new MoveToCommand(receiver);
                    MoveCommand server_command = new MoveToCommand(server_receiver);
                    MoveInvoker invoker = new MoveInvoker();
                    MoveInvoker server_invoker = new MoveInvoker();
                    invoker.SetCommand(command);
                    server_invoker.SetCommand(server_command);
                    Console.WriteLine("Before command --------------------------------------------");
                    Print2DArray(unitMap);
                    myUnits = invoker.ExecuteCommand();
                    unitMap = server_invoker.ExecuteCommand();

                    string arrayString = string.Join(",", unitMap.Cast<int>());
                    handleRequest(arrayString);

                    undo.Enabled = true;
                    Console.WriteLine("After command --------------------------------------------");
                    Print2DArray(unitMap);

                    // should set currentSelected to main value
                    objectChecked = false;
                }
                else
                {
                    isOK = false;
                    //String err_Message = String.Format("User tried to place object on another object or on a wrong tile");
                    //_logger.LogException(err_Message);
                    MessageBox.Show("You can't place object here", "Game error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (CheckIfClickOnAnObject(row, column))
                {
                    if (unitMap[row / 25, column / 25] == 7)
                    {

                        var rnd = new Random();
                        int ExplosionPower = rnd.Next(3);
                        MineStrategy mineStrategy = new MineStrategy();
                        switch (ExplosionPower)
                        {
                            case 0:
                                mineStrategy.SetMineStrategy(new SmallExplosion());
                                break;
                            case 1:
                                mineStrategy.SetMineStrategy(new MediumExplosion());
                                break;
                            case 2:
                                mineStrategy.SetMineStrategy(new HugeExplosion());
                                break;
                        }
                        mineStrategy.ExplodeMine(1);
                    }
                    else
                    {
                        objectChecked = true;
                        prev_x_loc = row / 25;
                        prev_y_loc = column / 25;
    
                        currentSelectedObject = myUnits[row / 25, column / 25];
                        string Cordinates = "";
                        BasicFill(myUnits, column / 25, row / 25, currentSelectedObject, ref Cordinates);
    
                        currentSelected = ConvertStringToArray(Cordinates, currentSelectedObject);
                        Print2DArray(currentSelected);
                        HighlightObject(currentSelected);
                    }
                }
            }
            if (isOK)
            {
                UpdateMap(row, column);
                clickedObject = 0;
            }
        }
        private void RenderGroundAfterChange()
        {
            for (int i = 0; i < currentSelected.GetLength(0); i++)
            {
                for (int j = 0; j < currentSelected.GetLength(0); j++)
                {
                    Point myPoint = new Point(25 * currentSelected[i, 1], 25 * currentSelected[i, 0]);
                    Label update_label = flowLayoutPanel2.GetChildAtPoint(myPoint) as Label;
                    update_label.BorderStyle = BorderStyle.None;
                    update_label.BorderStyle = BorderStyle.FixedSingle;
                    if (map[currentSelected[i, 0]][currentSelected[i, 1]].mapObject is Sand)
                    {
                        update_label.BackColor = Color.SandyBrown;
                        update_label.Image = Image.FromFile("../../../GameModels/Textures/sandTile.png");
                    }
                    else if (map[currentSelected[i, 0]][currentSelected[i, 1]].mapObject is Grass)
                    {
                        update_label.BackColor = Color.LawnGreen;
                        update_label.Image = Image.FromFile("../../../GameModels/Textures/grassTile.png");
                    }
                    else
                    {
                        update_label.BackColor = Color.DarkCyan;
                        update_label.Image = Image.FromFile("../../../GameModels/Textures/waterTile.png");
                    }
                }
            }
        }
        private bool CheckIfClickOnAnObject(int row, int column)
        {
            int x = row / 25;
            int y = column / 25;
            if (myUnits[x, y] != 0 && myUnits[x, y] != 8)
            {
                return true;
            }
            else return false;
        }
        private static void BasicFill(int[,] array, int x, int y, int object_id, ref string coordinates)
        {
            if (array[y, x] == object_id)
            {
                BasicFill(array, x, y, array.GetLength(1), array.GetLength(0), object_id, ref coordinates);
            }
        }
        private int[,] ConvertStringToArray(string Cordinates, int object_id)
        {
            Cordinates = Cordinates.Remove(Cordinates.Length - 1);
            Console.WriteLine("coordinates after remove " + Cordinates);
            var numbers = Cordinates.Split(',').Select(Int32.Parse).ToList();

            int[,] newArr = ReturnAreaSize(object_id);
            for (int i = 0; i < newArr.GetLength(0); i++)
            {
                newArr[i, 0] = numbers[i * 2 + 1];
                newArr[i, 1] = numbers[i * 2];
            }
            return newArr;
        }
        private void HighlightObject(int[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                Point myPoint = new Point(25 * arr[i, 1], 25 * arr[i, 0]);
                Label update_label = flowLayoutPanel2.GetChildAtPoint(myPoint) as Label;
                update_label.BorderStyle = BorderStyle.Fixed3D;
                update_label.ForeColor = Color.Red;
            }
        }
        static void BasicFill(int[,] array, int x, int y, int width, int height, int object_id, ref string coordinates)
        {
            array[y, x] = 2;
            coordinates += x.ToString() + "," + y.ToString() + ",";
            if (y > 0 && array[y - 1, x] == object_id)
            {
                BasicFill(array, x, y - 1, width, height, object_id, ref coordinates);
            }

            if (x > 0 && array[y, x - 1] == object_id)
            {
                BasicFill(array, x - 1, y, width, height, object_id, ref coordinates);
            }

            if (x < width - 1 && array[y, x + 1] == object_id)
            {
                BasicFill(array, x + 1, y, width, height, object_id, ref coordinates);
            }

            if (y < height - 1 && array[y + 1, x] == object_id)
            {
                BasicFill(array, x, y + 1, width, height, object_id, ref coordinates);
            }
        }
        private int[,] ReturnAreaSize(int object_id)
        {
            switch (object_id)
            {
                case 1:
                    return new int[8, 2];
                case 2:
                case 5:
                    return new int[4, 2];
                case 3:
                    return new int[5, 2];
                case 4:
                    return new int[6, 2];
                case 6:
                    return new int[2, 2];
                default:
                    return new int[1, 2];
            }
        }
        private int ReturnObjectRowNumber(int object_id)
        {
            switch (object_id)
            {
                case 5:
                case 6:
                case 4:
                    return 2;
                case 2:
                case 1:
                    return 4;
                case 3:
                    return 5;
                default:
                    return 1;
            }
        }
        private int ReturnbjectColumnNumber(int object_id)
        {
            switch (object_id)
            {
                case 1:
                case 5:
                    return 2;
                case 4:
                    return 3;
                default:
                    return 1;
            }
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
                        // Point myPoint = new Point((column + (25 * j)), (row + 25 * i));
                        // Label update_label = flowLayoutPanel2.GetChildAtPoint(myPoint) as Label;

                        Point myPoint = new Point((column + (25 * j)), (row + 25 * i));
                        int x = (column + (25 * j)) / 25;
                        int y = (row + (25 * i)) / 25;
                        unitMap[y, x] = object_id;
                        Console.WriteLine("Object id placed on map -- " + object_id);
                        myUnits[y, x] = object_id;
                        Label update_label = flowLayoutPanel2.GetChildAtPoint(myPoint) as Label;
                        switch (object_id)
                        {
                            // aircrafts 2, ships - 3, mine - 1, soldier: 1
                            case 1:
                                // shipcarrier
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/shipcarrier/shipcarrier" + counter.ToString() + ".png");
                                if (isInCreationStage)
                                {
                                    shipCarrier--;
                                    label1.Text = "Left: " + shipCarrier.ToString();
                                    isInCreationStage = false;
                                    if (shipCarrier <= 0)
                                    {
                                        button3.Enabled = false;
                                    }

                                }
                                break;
                            case 2:
                                // shipdestroyer
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/shipdestroyer/shipdestroyer" + counter.ToString() + ".png");
                                if (isInCreationStage)
                                {
                                    shipDestroyer--;
                                    label2.Text = "Left: " + shipDestroyer.ToString();
                                    isInCreationStage = false;
                                    if (shipDestroyer <= 0)
                                    {
                                        button4.Enabled = false;
                                    }
                                }

                                break;
                            case 3:
                                // submarine
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/submarine/submarine" + counter.ToString() + ".png");
                                if (isInCreationStage)
                                {
                                    submarine--;
                                    label3.Text = "Left: " + submarine.ToString();
                                    isInCreationStage = false;
                                    if (submarine <= 0)
                                    {
                                        button5.Enabled = false;
                                    }
                                }
                                break;
                            case 4:
                                // plane
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = planeImg[counter - 1];
                                if (isInCreationStage)
                                {
                                    plane--;
                                    label4.Text = "Left: " + plane.ToString();
                                    isInCreationStage = false;
                                    if (plane <= 0)
                                    {
                                        button6.Enabled = false;
                                    }
                                }
                                break;
                            case 5:
                                // jet
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = jetImg[counter - 1];
                                if (isInCreationStage)
                                {
                                    jet--;
                                    label8.Text = "Left: " + jet.ToString();
                                    isInCreationStage = false;
                                    if (jet <= 0)
                                    {
                                        button10.Enabled = false;
                                    }
                                }
                                break;
                            case 6:
                                // soldier
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/soldier/soldier" + counter.ToString() + ".png");
                                if (isInCreationStage)
                                {
                                    soldier--;
                                    label5.Text = "Left: " + soldier.ToString();
                                    isInCreationStage = false;
                                    if (soldier <= 0)
                                    {
                                        button7.Enabled = false;
                                    }
                                }
                                break;
                            case 7:
                                //mine
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures/mine.png");
                                if (isInCreationStage)
                                {
                                    mine--;
                                    label6.Text = "Left: " + mine.ToString();
                                    isInCreationStage = false;
                                    if (mine <= 0)
                                    {
                                        button8.Enabled = false;
                                    }
                                }
                                break;
                            default:
                                break;

                        }
                        counter++;
                    }
                }
                string arrayString = string.Join(",", unitMap.Cast<int>());
                Console.WriteLine("before request send: ");
                Console.WriteLine(arrayString);
                handleRequest(arrayString);
            }
            else
            {
                String err_Message = String.Format("User tried to place object which id is {0} in location x: {1} and y: {2}, which is not available", object_id, row, column);
                _logger.LogException(err_Message);
                MessageBox.Show("You can't place object here", "Game error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SendData(string request)
        {
            if (request.ToLower() == "end game")
            {
                this.Hide();
                EndGame endForm = new EndGame();
                endForm.ShowDialog();
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
            _logger = Logger.GetInstance;
            // refresh.Visible = false;
            InitializeComponent();

            label1.Text = "Left: " + shipCarrier.ToString();
            label2.Text = "Left: " + shipDestroyer.ToString();
            label3.Text = "Left: " + submarine.ToString();
            label4.Text = "Left: " + plane.ToString();
            label8.Text = "Left: " + jet.ToString();
            label5.Text = "Left: " + soldier.ToString();
            label6.Text = "Left: " + mine.ToString();

            undo.Enabled = false;

            // flowLayoutPanel2.Size = new Size(1239, 64 * 25);
            this.AutoScroll = true;
            this.username.Text = "User: " + username;
            this.lifepoints.Text = "LifePoints: " + 19.ToString();
            _clientSocket = socket;

            handleRequest("username: " + username);
        }
        private void handleRequest(string request)
        {
            Console.WriteLine("request ----> ", request);
            byte[] buffer = Encoding.ASCII.GetBytes(request);
            Console.WriteLine("Buffer --- " + System.Text.Encoding.UTF8.GetString(buffer));
            _clientSocket.Send(buffer);

            byte[] responseBuffer = new byte[300000];
            int rec = _clientSocket.Receive(responseBuffer);

            byte[] data = new byte[rec];
            Array.Copy(responseBuffer, data, rec);


            Console.WriteLine("Full encoded data", Encoding.ASCII.GetString(data));
            if (request.Length < 10)
            {
                switch (request)
                {
                    case "map":
                        var mapString = System.Text.Encoding.Default.GetString(data);
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
                    case "reload":
                        var unitArrayString = System.Text.Encoding.Default.GetString(data);
                        unitMap = StringTo2DArray(unitArrayString);
                        // Print2DArray(unitMap);
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
        private void Print2DArray(int[,] arr)
        {
            string line = new string('-', 70);
            Console.WriteLine(line);
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    Console.Write(arr[i, j]);
                }
                Console.WriteLine();
            }
        }
        private int[,] StringTo2DArray(string query)
        {
            int[] array = query.Split(V).Select(n => Convert.ToInt32(n)).ToArray();
            int[,] arrayToReturn = new int[64, 64];
            int counter = 0;
            Console.WriteLine("int length " + array.Length);
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
        private void Battle_Load(object sender, EventArgs e)
        {
            AirCraftBuilder builder;
            AirCraftDirector airCraftDirector = new AirCraftDirector();
            builder = new JetBuilder();
            airCraftDirector.Construct(builder);
            Image jetImg = builder.AirCraft.Show();
            button10.BackgroundImage = jetImg;
            builder = new PlaneBuilder();
            airCraftDirector.Construct(builder);
            Image planeImg = builder.AirCraft.Show();
            button6.BackgroundImage = planeImg;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            undo.Enabled = false;
            handleRequest("map");
            renderLabels();
            refresh.Visible = true;
        }

        private void grid_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            undo.Enabled = false;
            clickedObject = 1;
            isInCreationStage = true;
            handleRequest("start");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            undo.Enabled = false;
            clickedObject = 2;
            isInCreationStage = true;
            handleRequest("start");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            undo.Enabled = false;
            clickedObject = 3;
            isInCreationStage = true;
            handleRequest("start");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            undo.Enabled = false;
            // plane builder 
            clickedObject = 4;
            isInCreationStage = true;
            handleRequest("start");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            undo.Enabled = false;
            clickedObject = 6;
            isInCreationStage = true;
            handleRequest("start");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            undo.Enabled = false;
            clickedObject = 7;
            isInCreationStage = true;
            handleRequest("start");
        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

            // clickedObject = 7;
            // handleRequest("start");
            // if (mine <= 1)
            // {
            //     button9.Enabled = false;
            // }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SendData("end game");
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            undo.Enabled = false;
            handleRequest("reload");
        }
        private void username_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            undo.Enabled = false;
            // jet builder
            clickedObject = 5;
            isInCreationStage = true;
            handleRequest("start");
        }
        private void UndoObjectPosition(int x, int y, int object_id)
        {
            int[,] areaSize = ReturnAreaSize(object_id);
            for (int i = 0; i < areaSize.GetLength(0); i++)
            {
                for (int j = 0; j < areaSize.GetLength(1); j++)
                {
                    Point myPoint = new Point((y + i) * 25, (x + j) * 25);
                    Label update_label = flowLayoutPanel2.GetChildAtPoint(myPoint) as Label;
                    update_label.BorderStyle = BorderStyle.FixedSingle;
                    if (map[x + j][y + i].mapObject is Grass)
                    {
                        update_label.BackColor = Color.LawnGreen;
                        update_label.Image = Image.FromFile("../../../GameModels/Textures/grassTile.png");
                    }
                    else if (map[x + j][y + i].mapObject is Sand)
                    {
                        update_label.BackColor = Color.SandyBrown;
                        update_label.Image = Image.FromFile("../../../GameModels/Textures/sandTile.png");
                    }
                    else
                    {
                        update_label.BackColor = Color.DarkCyan;
                        update_label.Image = Image.FromFile("../../../GameModels/Textures/waterTile.png");
                    }
                }
            }
            int column_number = ReturnbjectColumnNumber(currentSelectedObject);
            int row_number = ReturnObjectRowNumber(currentSelectedObject);
            Console.WriteLine("PREV X POS " + prev_x_loc);
            Console.WriteLine("PREV Y POS " + prev_y_loc);
            renderObject(prev_y_loc * 25, prev_x_loc * 25, column_number, row_number, currentSelectedObject);
        }
        private void undo_Click(object sender, EventArgs e)
        {

            Print2DArray(currentSelected);
            // Command design pattern for player map and server map
            MoveReceiver receiver = new MoveReceiver(myUnits, currentSelectedObject, x_loc, y_loc, currentSelected);
            MoveReceiver server_receiver = new MoveReceiver(unitMap, currentSelectedObject, x_loc, y_loc, currentSelected);
            // undo moves 
            MoveCommand command = new UndoCommand(receiver);
            MoveCommand server_command = new UndoCommand(server_receiver);
            MoveInvoker invoker = new MoveInvoker();
            MoveInvoker server_invoker = new MoveInvoker();
            String line = new String('-', 50);
            Console.WriteLine(line);
            Print2DArray(unitMap);

            invoker.SetCommand(command);
            server_invoker.SetCommand(server_command);
            // update map front
            UndoObjectPosition(x_loc, y_loc, currentSelectedObject);
            myUnits = invoker.ExecuteCommand();

            Console.WriteLine(line);
            Print2DArray(unitMap);
            unitMap = server_invoker.ExecuteCommand();

            string arrayString = string.Join(",", unitMap.Cast<int>());
            handleRequest(arrayString);

            undo.Enabled = false;
            currentSelectedObject = -1;
        }
    }
}

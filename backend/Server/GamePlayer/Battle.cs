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
using System.Threading.Tasks;
using System.Threading;
using GameModels.Template;
using GameModels.Iterator;
using GameModels.FlyWeight;
using GameModels.Memento;
using GameModels.ChainOfResp;
using GameModels.State;

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
        private bool shootIsClicked = false;
        private int currentSelectedObject = 0;

        private int x_loc;
        private int y_loc;
        private int[,] pointsArr = new int[19, 2];
        
        System.Windows.Forms.Timer myTimer;
        private int timer_counter = 0;
        private bool afterShotWasClicked = false;


        // MEMENTO
        private MapOriginator mapOriginator = new MapOriginator();
        private MapMemory mapMemory = new MapMemory();
        private bool isMapLocked = false;


        // Chains
        private Badge lowBadge = new LowBadge();
        private Badge mediumBadge = new MidBadge();
        private Badge highBadge = new HighBadge();

        // State
        private PlayerContext playerContext = new PlayerContext(new NewbieState("Newbie"), 0);


        // highlight area
        private int min_x = 0;
        private int max_x = 63;
        private int min_y = 3;
        private int max_y = 63;

       
 
        private FlyWeightGroundFactory factory = new FlyWeightGroundFactory();
        private int prev_x_loc;
        private int prev_y_loc;

        private static int[,] unitMap = new int[64, 64];
        private int[,] myUnits = new int[64, 64];
        private int[,] currentSelected;
        int lifepointsLeft = 14;
        int shipCarrier = 2;
        int shipDestroyer = 2;
        int submarine = 3;
        int plane = 2;
        int jet = 2;
        int soldier = 3;
        int mine = 5;

        private Matrix map;
        private PlayerData playerDataOnStart;


        private void renderLabels()
        {
            Console.WriteLine(map.RowCount);
            var lableArray = new Label[64, 64];
            for (int i = 0; i < map.RowCount; i++)
            {
                for (int j = 0; j < map.ColumnCount; j++)
                {
                    var label_to_add = new Label();
                    label_to_add.Size = new Size(25, 25);
                    label_to_add.Top = i * 25;
                    label_to_add.Left = j * 25;
                    label_to_add.Margin = new Padding(0);
                    label_to_add.BorderStyle = BorderStyle.FixedSingle;

                    label_to_add.AutoSize = false;

                    label_to_add.Click += new EventHandler(HandleClickLabel);

                    if (map[i,j] is Sand)
                    {
                        label_to_add.BackColor = Color.SandyBrown;
                        GroundImage image = factory.GetGround("Sand");
                        label_to_add.Image = image.GiveImage();
                    }
                    else if (map[i,j] is Grass)
                    {
                        label_to_add.BackColor = Color.LawnGreen;
                        GroundImage image = factory.GetGround("Grass");
                        label_to_add.Image = image.GiveImage();
                    }
                    else
                    {
                        label_to_add.BackColor = Color.DarkCyan;
                        GroundImage image = factory.GetGround("Water");
                        label_to_add.Image = image.GiveImage();
                    }
                    flowLayoutPanel2.Controls.Add(label_to_add);
                }
            }
        }
        private bool LandedOnMine(int[,] map, int x, int y, int row, int column)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    int newx = (x + j);
                    int newy = (y + i);
                    if (map[newy, newx] == 7)
                        return true;
                }
            }
            return false;
        }
        private int[] MineCoordinates(int[,] map, int x, int y, int row, int column)
        {
            int[] coordinates = new int[2];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    int newx = (x + j);
                    int newy = (y + i);
                    if (map[newy, newx] == 7)
                    {
                        coordinates[0] = newx;
                        coordinates[1] = newy;
                    }
                }
            }
            return coordinates;
        }
        protected void HandleClickLabel(object sender, EventArgs e)
        {
            Label button = sender as Label;
            int row = button.Top;
            int column = button.Left;

            bool isOK = true;
            // shoot is clicked
            if (objectChecked && currentSelectedObject != -1 && shootIsClicked == false)
            {
                int column_number = ReturnbjectColumnNumber(currentSelectedObject);
                int row_number = ReturnObjectRowNumber(currentSelectedObject);
                CanBePlaced canBePlaced = new CanBePlaced();
                bool eligible = canBePlaced.IsEligible(unitMap, (column / 25), (row / 25), column_number, row_number, currentSelectedObject);
                if (eligible)
                {
                    if (LandedOnMine(unitMap, (column / 25), (row / 25), row_number, column_number))
                    {
                        int[] coordinates = MineCoordinates(unitMap, (column / 25), (row / 25), row_number, column_number);
                        var rnd = new Random();
                        int ExplosionPower = rnd.Next(3);
                        MineStrategy mineStrategy = new MineStrategy();
                        switch (ExplosionPower)
                        {
                            case 0:
                                mineStrategy.SetMineStrategy(new SmallExplosion());
                                renderObject(coordinates[0] * 25, coordinates[1] * 25, 1, 1, 8, unitMap);
                                break;
                            case 1:
                                mineStrategy.SetMineStrategy(new MediumExplosion());
                                renderObject(coordinates[0] * 25, coordinates[1] * 25, 2, 2, 9, unitMap);
                                break;
                            case 2:
                                mineStrategy.SetMineStrategy(new HugeExplosion());
                                renderObject(coordinates[0] * 25, coordinates[1] * 25, 3, 3, 10, unitMap);
                                break;
                        }
                        unitMap = mineStrategy.ExplodeMine(unitMap, coordinates[1], coordinates[0]);
                        myUnits = mineStrategy.ExplodeMine(myUnits, coordinates[1], coordinates[0]);
                        playerContext.Request(-1);

                        this.state_label.Text = playerContext.StateReturner();
                        // CONFLICTS
                        objectChecked = false;
                        this.shoot.Visible = false;
                        //RenderGroundAfterChange();
                        lifepointsLeft--;
                        this.lifepoints.Text = "LifePoints: " + lifepointsLeft;
                        badgeSetter();
                        // Print2DArray(unitMap);
                        // RenderGroundAfterChange();
                        // lifepointsLeft--;
                        // this.lifepoints.Text = "LifePoints: " + lifepointsLeft;
                        MoveReceiver explode_receiver = new MoveReceiver(myUnits, currentSelectedObject, row / 25, column / 25, currentSelected);
                        MoveReceiver server_explode_receiver = new MoveReceiver(unitMap, currentSelectedObject, row / 25, column / 25, currentSelected);
                        MoveCommand command = new ExplosionCommand(explode_receiver);
                        MoveCommand server_command = new ExplosionCommand(server_explode_receiver);
                        MoveInvoker invoker = new MoveInvoker();
                        MoveInvoker server_invoker = new MoveInvoker();
                        invoker.SetCommand(command);
                        server_invoker.SetCommand(server_command);
                        myUnits = invoker.ExecuteCommand();
                        unitMap = server_invoker.ExecuteCommand();

                        string arrayString = string.Join(",", unitMap.Cast<int>());
                        handleRequest(arrayString);

                        // Console.WriteLine("After Explosion");
                        Print2DArray(unitMap, "2D ARR");
                    }
                    else
                    {
                        x_loc = row / 25;
                        y_loc = column / 25;
                        renderObject(column, row, column_number, row_number, currentSelectedObject, unitMap);
                        RenderGroundAfterChange();
                        // Command design pattern for player map and server map
                        MoveReceiver receiver = new MoveReceiver(myUnits, currentSelectedObject, x_loc, y_loc, currentSelected);
                        MoveReceiver server_receiver = new MoveReceiver(unitMap, currentSelectedObject, x_loc, y_loc, currentSelected);
                        MoveCommand command = new MoveToCommand(receiver);
                        MoveCommand server_command = new MoveToCommand(server_receiver);
                        MoveInvoker invoker = new MoveInvoker();
                        MoveInvoker server_invoker = new MoveInvoker();
                        invoker.SetCommand(command);
                        server_invoker.SetCommand(server_command);
                        // Console.WriteLine("Before command --------------------------------------------");
                        // (unitMap);
                        myUnits = invoker.ExecuteCommand();
                        unitMap = server_invoker.ExecuteCommand();

                        string arrayString = string.Join(",", unitMap.Cast<int>());
                        handleRequest(arrayString);

                        undo.Enabled = true;
                        // Console.WriteLine("After command --------------------------------------------");
                        // Print2DArray(unitMap);

                        // should set currentSelected to main value
                        objectChecked = false;
                        this.shoot.Visible = false;
                    }
                }
                else
                {
                    isOK = false;

                    MessageBox.Show("You can't place object here", "Game error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (CheckIfClickOnAnObject(row, column) && objectChecked == false)
                {
                    this.shoot.Visible = true;
                    string Cordinates = "";

                    prev_x_loc = row / 25;
                    prev_y_loc = column / 25;
                    currentSelectedObject = unitMap[row / 25, column / 25];

                    BasicFill(unitMap, prev_y_loc, prev_x_loc, currentSelectedObject, ref Cordinates, ReturnAreaSize(currentSelectedObject).GetLength(0));

                    currentSelected = ConvertStringToArray(Cordinates, currentSelectedObject);
                    Print2DArray(currentSelected, "current selected");
                    HighlightObject(currentSelected);
                    afterShotWasClicked = false;
                    objectChecked = true;
                } 
                else if (shootIsClicked && objectChecked)
                {
                    Console.WriteLine("Shoot is clicked. ");
                    RemoveHighlight(currentSelected);
                    int obj_x = prev_x_loc;
                    int obj_y = prev_y_loc;
                    MineStrategy mineStrategy = new MineStrategy();
                    mineStrategy.SetMineStrategy(new SmallExplosion());
                    if (CheckIfClickOnAnObject(row, column))
                    {
                        string coordinates = "";
                        int[] areaPoints = new int[3];
                        BasicFill(unitMap, column / 25, row / 25, unitMap[row / 25, column / 25], ref coordinates, ReturnAreaSize(unitMap[row/ 25, column / 25 ]).GetLength(0));
                        int[,] toBlowMap = new int[coordinates.Split(',').Length/2, 2];

                        playerContext.Request(1);
                        this.state_label.Text = playerContext.StateReturner();
                        Thread newThread = new Thread(() =>
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                myTimer.Start();                              
                                toBlowMap = ConvertStringToArray(coordinates, unitMap[row / 25, column / 25]);
                                areaPoints = CalculatePoints(currentSelected);
                                HighlightShootingObject(currentSelected);                        
                            });
                            Thread.Sleep(1500);
                            this.BeginInvoke((Action)delegate ()
                            {                                
                                myTimer.Start();
                                int[] pointsBounds = new int[] { min_x, max_x, min_y, max_y };
                                if (!CheckIfNotOutOfBounds(pointsBounds, row / 25, row / 25) && unitMap[obj_x, obj_y] == 4)
                                {
                                    BlowObject(toBlowMap, unitMap[row / 25 , column / 25]);
                                    lifepointsLeft--;
                                    // update badge
                                    badgeSetter();
                                    this.lifepoints.Text = "LifePoints: " + lifepointsLeft;
                                    RemovePlaneHighlightArea(areaPoints);
                                    RemoveHighlight(currentSelected);
                                } else
                                {
                                    BlowObject(toBlowMap, unitMap[row / 25, column / 25]);
                                    // update badge
                                    lifepointsLeft--;
                                    badgeSetter();
                                    this.lifepoints.Text = "LifePoints: " + lifepointsLeft;
                                    RemoveHighlight(currentSelected);
                                }
                            });
                        });
                        newThread.Start();
                        string arrayString = string.Join(",", unitMap.Cast<int>());
                        handleRequest(arrayString);

                    } else
                    {
                        ShootingObject shootingObject = new PlaneTemplate();
                        ShootingObject shootingShip = new ShipTemplate();
                        int[] areaPoints = new int[3];
                        Thread newThread = new Thread(() =>
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                myTimer.Start();
                                switch (unitMap[obj_x, obj_y])
                                {
                                    case 4:
                                        shootingObject.RunFirstPart(this.flowLayoutPanel2, currentSelected, obj_x, obj_y, unitMap[obj_x, obj_y]);
                                        areaPoints = CalculatePoints(currentSelected);
                                        break;
                                    case 2:
                                        shootingShip.RunFirstPart(this.flowLayoutPanel2, currentSelected, obj_x, obj_y, unitMap[obj_x, obj_y]);
                                        break;
                                    default:
                                        HighlightShootingObject(currentSelected);
                                        break;
                                }
                            });
                            Thread.Sleep(1500);
                            this.BeginInvoke((Action)delegate ()
                            {                            
                                myTimer.Start();
                                int[] pointsBounds = new int[] { min_x, max_x, min_y, max_y };
                                if(!CheckIfNotOutOfBounds(pointsBounds, obj_x, obj_y) && unitMap[obj_x, obj_y] == 4)
                                {
                                    renderObject(column, row, 1, 1, 8, unitMap);
                                    shootingObject.RunSecondPart(this.flowLayoutPanel2, areaPoints, map, currentSelected);
                                }
                                else if (CheckIfNotOutOfBounds(pointsBounds, obj_x, obj_y) && unitMap[obj_x, obj_y] == 4)
                                {
                                    RemoveHighlight(currentSelected);
                                }
                                else
                                {
                                    renderObject(column, row, 1, 1, 8, unitMap);
                                    switch (unitMap[obj_x, obj_y])
                                    {
                                        case 2:
                                            shootingShip.RunSecondPart(this.flowLayoutPanel2, new int[] { 0, 63, 0, 63 }, map, currentSelected);
                                            break;
                                        default:
                                            RemoveHighlight(currentSelected);
                                            break;
                                    }
                                }
                            });
                        });
                        newThread.Start();
                    }
                    Console.WriteLine("Removed current focus. ");
                    objectChecked = false;
                    this.shoot.Visible = false;
                    shootIsClicked = false;
                }
            }
            if (isOK)
            {
                UpdateMap(row, column);
                clickedObject = 0;
            }
        }
            
        private void badgeSetter()
        {
            string path = "";
            highBadge.SetBadge(ref path, lifepointsLeft);

            this.badge_btn.BackgroundImage = Image.FromFile(path);
            // if()

        }
        private int[] CalculatePoints(int[,] planePosition)
        {
            int[,] endPoint = ReturnEndCoordinate(planePosition);
            min_x = 0;
            max_x = 63;
            min_y = endPoint[0, 1] + 1;
            max_y = 63;

            if (endPoint[0, 0] > 8) min_x = endPoint[0, 0] - 8;
            if (endPoint[0, 0] < 55) max_x = endPoint[0, 0] + 8;
            if (endPoint[0, 1] < 48) max_y = endPoint[0, 1] + 15;
            return new int[] { min_x, max_x, min_y, max_y };
        }
        private bool CheckIfNotOutOfBounds(int[] arr, int x, int y)
        {
            int counter = 0;
            for(int i = arr[0]; i < arr[1]; i++)
            {
                for(int j = arr[2]; j < arr[3]; j++)
                {
                    if (i == x && j == y) counter++;
                }
            }
            if (counter > 0) return true;
            else return false;
        }
        private void RemovePlaneHighlightArea(int [] areaPoints)
        {
            this.flowLayoutPanel2.BackColor = Color.Transparent;
            for (int i = areaPoints[0]; i < areaPoints[1]; i++)
            {
                for (int j = areaPoints[2]; j < areaPoints[3]; j++)
                {
                    Point myPoint = new Point(25 * j, 25 * i);
                    Label update_label = flowLayoutPanel2.GetChildAtPoint(myPoint) as Label;
                    update_label.BorderStyle = BorderStyle.FixedSingle;
                }
            }
        }
        private int[,] ReturnEndCoordinate(int [,] arr)
        {
            int[,] endCoordinate = new int[1, 2];
            int max = arr[0, 0] + arr[0, 1];
            for(int i = 1; i < arr.GetLength(0); i++)
            {
                if(arr[i, 0] + arr[i, 1] > max)
                {
                    max = arr[i, 0] + arr[i, 1];
                    endCoordinate[0, 0] = arr[i, 0];
                    endCoordinate[0, 1] = arr[i, 1];
                }
            }
            return endCoordinate;
        }
        private void BlowObject(int [,] blowArr, int id)
        {
            MineStrategy mineStrategy = new MineStrategy();
            if(id == 5)
            {
                mineStrategy.SetMineStrategy(new MediumExplosion());
                DestroyObject(blowArr[0, 1] * 25, blowArr[0, 0] * 25, 2, 2, 9);
            }
            for (int i = 0; i < blowArr.GetLength(0); i++)
            {
                if(id != 5)
                {
                    unitMap[blowArr[i, 0], blowArr[i, 1]] = 8;
                    myUnits[blowArr[i, 0], blowArr[i, 1]] = 8;
                    mineStrategy.SetMineStrategy(new SmallExplosion());
                    DestroyObject(blowArr[i, 1] * 25, blowArr[i, 0] * 25, 1, 1, 8);
                }
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
                    if (map[currentSelected[i, 0], currentSelected[i, 1]] is Sand)
                    {
                        update_label.BackColor = Color.SandyBrown;
                        GroundImage image = factory.GetGround("Sand");
                        update_label.Image = image.GiveImage();
                    }
                    else if (map[currentSelected[i, 0], currentSelected[i, 1]] is Grass)
                    {
                        update_label.BackColor = Color.LawnGreen;
                        GroundImage image = factory.GetGround("Grass");
                        update_label.Image = image.GiveImage();
                    }
                    else
                    {
                        update_label.BackColor = Color.DarkCyan;
                        GroundImage image = factory.GetGround("Water");
                        update_label.Image = image.GiveImage();
                    }
                }
            }
        }
        private bool CheckIfClickOnAnObject(int row, int column)
        {
            int x = row / 25;
            int y = column / 25;
            if (myUnits[x, y] != 0 && myUnits[x, y] != 8 && myUnits[x, y] != 9 && myUnits[x, y] != 10)
            {
                return true;
            }
            else return false;
        }
        private static void BasicFill(int[,] array, int x, int y, int object_id, ref string coordinates, int length)
        {
            // switched x and y 
            if (array[y, x] == object_id)
            {
                BasicFill(array, x, y, array.GetLength(1), array.GetLength(0), object_id, ref coordinates, length);
            }
        }
        private int[,] ConvertStringToArray(string Cordinates, int object_id)
        {
            Cordinates = Cordinates.Remove(Cordinates.Length - 1);
            var numbers = Cordinates.Split(',').Select(Int32.Parse).ToList();
            int[,] newArr = ReturnAreaSize(object_id);
            for (int i = 0; i < newArr.GetLength(0); i++)
            {
                newArr[i, 0] = numbers[i * 2];
                newArr[i, 1] = numbers[i * 2 + 1];
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
        private void HighlightShootingObject(int[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                Point myPoint = new Point(25 * arr[i, 1], 25 * arr[i, 0]);
                Label update_label = flowLayoutPanel2.GetChildAtPoint(myPoint) as Label;
                update_label.BorderStyle = BorderStyle.Fixed3D;
                update_label.BackColor = Color.Red;
            }
        }
        private void RemoveHighlight(int[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                Point myPoint = new Point(25 * arr[i, 1], 25 * arr[i, 0]);
                Label update_label = flowLayoutPanel2.GetChildAtPoint(myPoint) as Label;
                update_label.BorderStyle = BorderStyle.None;
                if (map[arr[i, 1], arr[i, 0]] is Sand)
                {
                    update_label.BackColor = Color.SandyBrown;
                }
                else if (map[arr[i, 1], arr[i, 0]] is Grass)
                {
                    update_label.BackColor = Color.LawnGreen;
                }
                else
                {
                    update_label.BackColor = Color.DarkCyan;
                }
            }
        }
        static void BasicFill(int[,] array, int x, int y, int width, int height, int object_id, ref string coordinates, int length)
        {
            // array[y, x] = 2;
            coordinates += y.ToString() + "," + x.ToString() + ",";
            int newLength = SimpleStringToIntArr(coordinates).GetLength(0);
            if (newLength != length)
            {
                if (y > 0 && array[y - 1, x] == object_id && DifferentValues(coordinates, y - 1, x))
                {
                    BasicFill(array, x, y - 1, width, height, object_id, ref coordinates, length);
                }

                if (x > 0 && array[y, x - 1] == object_id && DifferentValues(coordinates, y, x - 1))
                {
                    BasicFill(array, x - 1, y, width, height, object_id, ref coordinates, length);
                }

                if (x < width - 1 && array[y, x + 1] == object_id && DifferentValues(coordinates, y, x + 1))
                {
                    BasicFill(array, x + 1, y, width, height, object_id, ref coordinates, length);
                }

                if (y < height - 1 && array[y + 1, x] == object_id && DifferentValues(coordinates, y + 1, x))
                {
                    BasicFill(array, x, y + 1, width, height, object_id, ref coordinates, length);
                }
            }
        }
        private static bool DifferentValues(string coordinates, int x, int y)
        {
            // 2, 2   2, 3   2, 4   3, 2    3, 3   3, 4 
            int[,] arr = SimpleStringToIntArr(coordinates);
            int counter = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i, 0] == x && arr[i, 1] == y) counter++;
            }
            if(counter > 0)
            {
                return false;
            } else return true;
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
                    renderObject(column, row, 2, 4, clickedObject, unitMap);
                    break;
                case 2:
                    renderObject(column, row, 1, 4, clickedObject, unitMap);
                    break;
                case 3:
                    renderObject(column, row, 1, 5, clickedObject, unitMap);
                    break;
                case 4:
                    renderObject(column, row, 3, 2, clickedObject, unitMap);
                    break;
                case 5:
                    renderObject(column, row, 2, 2, clickedObject, unitMap);
                    break;
                case 6:
                    renderObject(column, row, 1, 2, clickedObject, unitMap);
                    break;
                case 7:
                    renderObject(column, row, 1, 1, clickedObject, unitMap);
                    break;
                default:
                    break;
            }
        }
        private void DestroyObject(int column, int row, int columnNumber, int rowNumber, int object_id)
        {
            int counter = 1;
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    Point myPoint = new Point((column + (25 * j)), (row + 25 * i));
                    int x = (column + (25 * j)) / 25;
                    int y = (row + (25 * i)) / 25;
                    unitMap[y, x] = object_id;
                    myUnits[y, x] = object_id;
                    Label update_label = flowLayoutPanel2.GetChildAtPoint(myPoint) as Label;
                    switch (object_id)
                    {
                        case 8:
                            update_label.BorderStyle = BorderStyle.None;
                            update_label.Image = Image.FromFile("../../../GameModels/Textures//Explosion/explosion1.png");
                            update_label.BackColor = Color.Black;
                            break;
                        case 9:
                            update_label.BorderStyle = BorderStyle.None;
                            update_label.Image = Image.FromFile("../../../GameModels/Textures//Explosion/explosion2/explosion2-" + counter.ToString() + ".png");
                            update_label.BackColor = Color.Black;
                            break;
                    }
                    playerContext.Request(-1);
                    counter++;
                }
            }
            string arrayString = string.Join(",", unitMap.Cast<int>());
            handleRequest(arrayString);
        }
        private void renderObject(int column, int row, int columnNumber, int rowNumber, int object_id, int [,] checkingMap)
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
            bool eligible = canBePlaced.IsEligible(checkingMap, (column / 25), (row / 25), rowNumber, columnNumber, object_id);
            int counter = 1;
            if (eligible)
            {
                for (int i = 0; i < rowNumber; i++)
                {
                    for (int j = 0; j < columnNumber; j++)
                    {

                        Point myPoint = new Point((column + (25 * j)), (row + 25 * i));
                        int x = (column + (25 * j)) / 25;
                        int y = (row + (25 * i)) / 25;
                        if (object_id == 10)
                        {
                            if (column + (25 * j) - 25 < 0 && row + (25 * j) - 25 < 0)
                            {
                                myPoint = new Point(0, 0);
                            }
                            else if (column + (25 * j) - 25 < 0)
                            {
                                myPoint = new Point(0, (row + 25 * i) - 25);
                            }
                            else if (row + (25 * j) - 25 < 0)
                            {
                                myPoint = new Point((column + (25 * j) - 25), 0);
                            }
                            else
                            {
                                myPoint = new Point((column + (25 * j) - 25), (row + 25 * i) - 25);
                            }
                        }
                        unitMap[y, x] = object_id;
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
                            case 8:
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures//Explosion/explosion1.png");
                                update_label.BackColor = Color.Black;
                                break;
                            case 9:
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures//Explosion/explosion2/explosion2-" + counter.ToString() + ".png");
                                update_label.BackColor = Color.Black;
                                break;
                            case 10:
                                update_label.BorderStyle = BorderStyle.None;
                                update_label.Image = Image.FromFile("../../../GameModels/Textures//Explosion/explosion3/explosion3-" + counter.ToString() + ".png");
                                update_label.BackColor = Color.Black;
                                break;
                            default:
                                break;

                        }
                        counter++;
                    }
                }
                string arrayString = string.Join(",", unitMap.Cast<int>());
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
            MessageBox.Show("Received: " + Encoding.ASCII.GetString(data), "Recieve info",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public Battle(string username, Socket socket)
        {
            _logger = Logger.GetInstance;
            InitializeComponent();

            // chain of resp
            // badge set
            highBadge.SetSuccessor(mediumBadge);
            mediumBadge.SetSuccessor(lowBadge);

            string result = "";
            highBadge.SetBadge(ref result, lifepointsLeft);
            Console.WriteLine("ref badge image path " + result);

            this.badge_btn.BackgroundImage = Image.FromFile(result);

            this.state_label.Text = "Newbie";


            myTimer = new System.Windows.Forms.Timer();
            myTimer.Interval = 250;

            this.shoot.Visible = false;

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
            this.lifepoints.Text = "LifePoints: " + lifepointsLeft;
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

            if (request.Length < 10)
            {
                switch (request)
                {
                    case "map":
                        var mapString = System.Text.Encoding.Default.GetString(data);
                        map = JsonConvert.DeserializeObject<Matrix>(mapString, new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        });
                        Array.Clear(responseBuffer, 0, responseBuffer.Length);
                        Array.Clear(data, 0, data.Length);
                        break;
                    case "start":
                        var playerDataString = System.Text.Encoding.Default.GetString(data);
                        playerDataOnStart = JsonConvert.DeserializeObject<PlayerData>(playerDataString, new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        });
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
        private void Print2DArray(int[,] arr, string title)
        {
            string line = new string('-', 70);
            string gaps = new string(' ', 25);
            Console.WriteLine(line);
            Console.WriteLine(gaps + " " + title + " " + gaps);
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    Console.Write(arr[i, j]);
                }
                Console.WriteLine();
            }
        }
        private static int [,] SimpleStringToIntArr(string query)
        {
            query = query.TrimEnd(',');
            string[] numbers = query.Split(',');
            if(numbers == null)
            {
                return new int[0, 0];
            } else
            {
                int[,] arr = new int[numbers.Length / 2, 2];
                int counter = 0;
                for(int i = 0; i < numbers.Length; i+=2)
                {
                    arr[counter, 0] = int.Parse(numbers[i]);
                    arr[counter, 1] = int.Parse(numbers[i+1]);
                    counter++;
                }
                return arr;
            }
        }
        private static int[,] StringTo2DArray(string query)
        {
            int[] array = query.Split(V).Select(n => Convert.ToInt32(n)).ToArray();
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
            this.button2.Visible = false;
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
                    if (map[x + j, y + i] is Grass)
                    {
                        update_label.BackColor = Color.LawnGreen;
                        GroundImage image = factory.GetGround("Grass");
                        update_label.Image = image.GiveImage();
                    }
                    else if (map[x + j, y + i] is Sand)
                    {
                        update_label.BackColor = Color.SandyBrown;
                        GroundImage image = factory.GetGround("Sand");
                        update_label.Image = image.GiveImage();
                    }
                    else
                    {
                        update_label.BackColor = Color.DarkCyan;
                        GroundImage image = factory.GetGround("Water");
                        update_label.Image = image.GiveImage();
                    }
                }
            }
            int column_number = ReturnbjectColumnNumber(currentSelectedObject);
            int row_number = ReturnObjectRowNumber(currentSelectedObject);

            renderObject(prev_y_loc * 25, prev_x_loc * 25, column_number, row_number, currentSelectedObject, unitMap);
        }
        private void undo_Click(object sender, EventArgs e)
        {

            Print2DArray(currentSelected, "before undo");
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
            Print2DArray(unitMap, "unit map before undo");

            invoker.SetCommand(command);
            server_invoker.SetCommand(server_command);
            // update map front
            UndoObjectPosition(x_loc, y_loc, currentSelectedObject);
            myUnits = invoker.ExecuteCommand();

            Console.WriteLine(line);
            Print2DArray(unitMap, "unit map after undo");
            unitMap = server_invoker.ExecuteCommand();

            string arrayString = string.Join(",", unitMap.Cast<int>());
            handleRequest(arrayString);

            undo.Enabled = false;
            currentSelectedObject = -1;
        }

        private void shoot_Click(object sender, EventArgs e)
        {
            shootIsClicked = true;
        }

        private void lock_btn_Click(object sender, EventArgs e)
        {
            isMapLocked = !isMapLocked;
            if (isMapLocked)
            {
                this.lock_btn.BackgroundImage = Image.FromFile("../../../GameModels/Textures/icons8-lock-30.png");
                mapOriginator.Status = isMapLocked;
                int[,] currentUnitMapState = MakeACopy(unitMap);
                mapOriginator.GameMap = currentUnitMapState;
                mapOriginator.LifePoints = lifepointsLeft;
                mapMemory.Memento = mapOriginator.SaveMemento();
                Console.WriteLine(" mapMemory.Memento  " + mapMemory.Memento);

            }
            else
            {
                this.lock_btn.BackgroundImage = Image.FromFile("../../../GameModels/Textures/icons8-unlock-30.png");
                mapOriginator.RestoreMemento(mapMemory.Memento);
                unitMap = mapOriginator.GameMap;

                string arrayString = string.Join(",", unitMap.Cast<int>());
                handleRequest(arrayString);
                ClearSector();
                RerenderMap();
                lifepointsLeft = mapOriginator.LifePoints;
                this.lifepoints.Text = "LifePoints: " + lifepointsLeft;
            }
        }
        private void ClearSector()
        {
            for(int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Point myPoint = new Point(25 * j, 25 * i);
                    Label update_label = flowLayoutPanel2.GetChildAtPoint(myPoint) as Label;
                    if (map[i, j] is Grass)
                    {
                        update_label.BackColor = Color.LawnGreen;
                        GroundImage image = factory.GetGround("Grass");
                        update_label.Image = image.GiveImage();
                    }
                    else if (map[i, j] is Sand)
                    {
                        update_label.BackColor = Color.SandyBrown;
                        GroundImage image = factory.GetGround("Sand");
                        update_label.Image = image.GiveImage();
                    }
                    else
                    {
                        update_label.BackColor = Color.DarkCyan;
                        GroundImage image = factory.GetGround("Water");
                        update_label.Image = image.GiveImage();
                    }
                }
            }
        }
        private int [,] MakeACopy(int [,] arr)
        {
            int[,] arrToReturn = new int[arr.GetLength(0), arr.GetLength(1)];
            for(int i = 0; i < arr.GetLength(0); i++)
            {
                for(int j  = 0; j < unitMap.GetLength(1); j++)
                {
                    arrToReturn[i, j] = arr[i, j];
                }
            }

            return arrToReturn;
        }
        private void RerenderMap()
        {
            bool stop = false;
            for(int i = 0;  i < unitMap.GetLength(0);  i++)
            {
                for (int j = 0; j < unitMap.GetLength(1); j++)
                {
                    if(unitMap[i,j] != 0 && stop == false)
                    {
                        int column_number = ReturnbjectColumnNumber(unitMap[i, j]);
                        int row_number = ReturnObjectRowNumber(unitMap[i, j]);

                        renderObject(j * 25, i * 25, column_number, row_number, unitMap[i, j], GereateEmptyMap(unitMap.GetLength(0), unitMap.GetLength(1)));
                        stop = true;
                    }
                }
            }
        }
        private int [,] GereateEmptyMap (int xSize, int ySize)
        {
            int[,] arr = new int[xSize, ySize];
            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize; j++)
                {
                    arr[i, j] = 0;
                }
            }
            return arr;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

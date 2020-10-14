using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GamePlayer
{
    public partial class optional : Form
    {
        private int clickedObject = 5;

        private void renderLabels()
        {
            Random rnd = new Random();
            var lableArray = new Label[32, 50];
            for (int i = 0; i < lableArray.GetLength(0); i++)
            {
                for (int j = 0; j < lableArray.GetLength(1); j++)
                {
                    int random_value = rnd.Next(1, 4);
                    var label_to_add = new Label();
                    label_to_add.Size = new Size(25, 25);
                    label_to_add.Top = i * 25;
                    label_to_add.Left = j * 25;
                    label_to_add.Margin = new Padding(0);

                    label_to_add.AutoSize = false;

                    label_to_add.Click += new EventHandler(HandleClickLabel);

                    switch (random_value)
                    {
                        case 1:
                            label_to_add.BackColor = System.Drawing.Color.Red;
                            break;
                        case 2:
                            label_to_add.BackColor = System.Drawing.Color.Green;
                            break;
                        case 3:
                            label_to_add.BackColor = System.Drawing.Color.Blue;
                            break;
                        default:
                            label_to_add.BackColor = System.Drawing.Color.Black;
                            break;
                    }
                    flowLayoutPanel1.Controls.Add(label_to_add);
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
            for (int i =0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    Point myPoint = new Point((column + (25 * j)), (row + 25 * i));
                    Label update_label = flowLayoutPanel1.GetChildAtPoint(myPoint) as Label;
                    switch (object_id)
                    {
                        // aircrafts 2, ships - 3, mine - 1, soldier: 1
                        case 1:
                            // plane
                            update_label.Image = Image.FromFile("../../../GameModels/Textures/plane/plane" + counter.ToString() + ".png");
                            break;
                        case 2:
                            // shipcarrier
                            update_label.Image = Image.FromFile("../../../GameModels/Textures/shipcarrier/shipcarrier" + counter.ToString() + ".png");
                            break;
                        case 3:
                            // shipdestroyer
                            update_label.Image = Image.FromFile("../../../GameModels/Textures/shipdestroyer/shipdestroyer" + counter.ToString() + ".png");
                            break;
                        case 4:
                            // submarine
                            update_label.Image = Image.FromFile("../../../GameModels/Textures/submarine/submarine" + counter.ToString() + ".png");
                            break;
                        case 5:
                            // soldier
                            update_label.Image = Image.FromFile("../../../GameModels/Textures/soldier/soldier" + counter.ToString() + ".png");
                            break;
                    }                      
                    counter++;
                }
            }
        }
        public optional()
        {
            InitializeComponent();
            flowLayoutPanel1.Size = new Size(25 * 50, 25 * 32);
            this.AutoSize = true;
            flowLayoutPanel1.SetAutoScrollMargin(0, 0);
            renderLabels();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

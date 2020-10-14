using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using GameModels;
using GameModels.AirCraftTypes;
using System.Windows.Forms;

namespace GamePlayer
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            AirCraftBuilder builder;
            AirCraftDirector airCraftDirector = new AirCraftDirector();
            builder = new JetBuilder();
            airCraftDirector.Construct(builder);
            Image b = builder.AirCraft.Show();
            Image a = Image.FromFile("../../../GameModels/Textures/plane.png");
            button1.BackgroundImage = b;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}

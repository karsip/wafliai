using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameModels.FlyWeight
{
    public class WaterImage : GroundImage
    {
        public WaterImage () 
        {
            this.image = Image.FromFile("../../../GameModels/Textures/waterTile.png");
        }
        public override Image GiveImage()
        {
            return this.image;
        }
    }
}

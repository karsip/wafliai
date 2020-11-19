using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameModels.FlyWeight
{
    public class GrassImage : GroundImage
    {
        public GrassImage() 
        {
            this.image = Image.FromFile("../../../GameModels/Textures/grassTile.png");
        }
        public override Image GiveImage()
        {
            return this.image;
        }
    }
}

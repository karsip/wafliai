using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace GameModels.FlyWeight
{
    public class SandImage : GroundImage
    {
        public SandImage() 
        {
            this.image = Image.FromFile("../../../GameModels/Textures/sandTile.png");
        }
        public override Image GiveImage() 
        {
            return this.image;
        }
    }
}

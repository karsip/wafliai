using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameModels.FlyWeight
{
    public abstract class GroundImage
    {
        protected Image image;
        public abstract Image GiveImage();
    }
}

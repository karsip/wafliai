using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameModels.GroundTypes
{
    [Serializable]
    public abstract class Ground : MapObjectInterface
    {
        public void Draw(TextureBrush color, int x, int y, int width, int height, Graphics graphics)
        {
            graphics.FillRectangle(color, x, y, width, height);
        }
    }
}

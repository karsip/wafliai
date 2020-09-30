using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BattleShipModels.GroundTypes
{
    [Serializable]
    public abstract class Ground : MapObjectInterface
    {
        public void Draw(SolidBrush color, int x, int y, int width, int height, Graphics graphics)
        {
            graphics.FillRectangle(color, x, y, width, height);
        }
    }
}

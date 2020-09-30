using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BattleShipModels
{
    public interface MapObjectInterface
    {
        void Draw(SolidBrush color, int x, int y, int width, int height, Graphics graphics);
    }
}

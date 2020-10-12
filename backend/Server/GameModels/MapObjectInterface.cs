using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameModels
{
    public interface MapObjectInterface
    {
        void Draw(TextureBrush color, int x, int y, int width, int height, Graphics graphics);
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GameModels.Template
{
    // shut arround  15 squares arround
    public class ShipTemplate : ShootingObject
    {
        public override void RenderBeforeShot(FlowLayoutPanel e, int[,] planePosition, int x, int y, int id)
        {
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 49; j++)
                {
                    Point myPoint = new Point(25 * j, 25 * i);
                    Label update_label = e.GetChildAtPoint(myPoint) as Label;
                    update_label.BorderStyle = BorderStyle.Fixed3D;
                }
            }
        }
    }
}

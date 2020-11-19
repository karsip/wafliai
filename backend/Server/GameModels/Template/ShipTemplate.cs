using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GameModels.Template
{
    // shut arround  15 squares arround
    public class ShipTemplate : ShootingObject
    {
        public override void RenderAfterShot()
        {
            Console.WriteLine("Ship After shot");
        }

        public override void RenderBeforeShot(FlowLayoutPanel e, int [,] positionArr)
        {
            throw new NotImplementedException();
        }

        public override void Shoot()
        {
            Console.WriteLine("Ship shot");
        }
    }
}

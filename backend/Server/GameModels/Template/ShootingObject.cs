using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GameModels.Template
{
    public abstract class ShootingObject
    {
        // after player clicks on map when object is selected
        public virtual void HighlightObject(FlowLayoutPanel e, int[,] positionArr)
        {
            for (int i = 0; i < positionArr.GetLength(0); i++)
            {
                Point myPoint = new Point(25 * positionArr[i, 1], 25 * positionArr[i, 0]);
                Label update_label = e.GetChildAtPoint(myPoint) as Label;
                update_label.BorderStyle = BorderStyle.Fixed3D;
                update_label.BackColor = Color.Red;
            }
        }
        public abstract void RenderBeforeShot(FlowLayoutPanel e, int[,] planePosition, int x, int y, int id);
        // do changes to both maps
        public abstract void Shoot();
        // render changes
        public abstract void RenderAfterShot();

        public void RunFirstPart(FlowLayoutPanel e, int[,] positionArr, int x, int y, int id)
        {
            Console.WriteLine("FlowLayoutPanel run");
            HighlightObject(e, positionArr);
            RenderBeforeShot(e, positionArr, x, y, id);
            
        }
        public void RunSecondPart(FlowLayoutPanel e, int[,] positionArr)
        {
            Shoot();
            RenderAfterShot();
        }
    }
}

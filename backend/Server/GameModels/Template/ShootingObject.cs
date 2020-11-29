using GameModels.GroundTypes;
using GameModels.Iterator;
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
        // render changes
        public virtual void RenderAfterShot(FlowLayoutPanel flowLayout, int[] areaPoints)
        {
            flowLayout.BackColor = Color.Transparent;
            for (int i = areaPoints[0]; i < areaPoints[1]; i++)
            {
                for (int j = areaPoints[2]; j < areaPoints[3]; j++)
                {
                    Point myPoint = new Point(25 * j, 25 * i);
                    Label update_label = flowLayout.GetChildAtPoint(myPoint) as Label;
                    update_label.BorderStyle = BorderStyle.FixedSingle;
                }
            }
        }

        // highlights object and shooting area
        public void RunFirstPart(FlowLayoutPanel e, int[,] positionArr, int x, int y, int id)
        {
            HighlightObject(e, positionArr);
            RenderBeforeShot(e, positionArr, x, y, id);     
        }
        public void RemoveHighlight(FlowLayoutPanel e ,int[,] currentSelected, Matrix map)
        {
            for (int i = 0; i < currentSelected.GetLength(0); i++)
            {
                Point myPoint = new Point(25 * currentSelected[i, 1], 25 * currentSelected[i, 0]);
                Label update_label = e.GetChildAtPoint(myPoint) as Label;
                update_label.BorderStyle = BorderStyle.None;
                if (map[currentSelected[i, 1], currentSelected[i, 0]] is Sand)
                {
                    update_label.BackColor = Color.SandyBrown;
                }
                else if (map[currentSelected[i, 1], currentSelected[i, 0]] is Grass)
                {
                    update_label.BackColor = Color.LawnGreen;
                }
                else
                {
                    update_label.BackColor = Color.DarkCyan;
                }
            }
        }
        public void RunSecondPart(FlowLayoutPanel e, int[] areaPoints, Matrix map, int [,] currentSelected)
        {
            RenderAfterShot(e, areaPoints);
            RemoveHighlight(e, currentSelected, map);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GameModels.Template
{
    public class PlaneTemplate : ShootingObject
    {
        private int min_x = 0;
        private int max_x = 0;
        private int min_y = 0;
        private int max_y = 0;
        public override void RenderAfterShot()
        {
            Console.WriteLine("Plane After shot");
        }

        public override void RenderBeforeShot(FlowLayoutPanel e, int [,] planePosition, int x, int y, int id)
        {
            int[,] endPoint = ReturnEndCoordinate(planePosition);
            min_x = 0;
            max_x = 63;
            min_y = endPoint[0, 1] + 1;
            max_y = 63;

            if (endPoint[0, 0] > 8) min_x = endPoint[0, 0] - 8;
            if (endPoint[0, 0] < 55) max_x = endPoint[0, 0] + 8;
            if (endPoint[0, 1] < 48) max_y = endPoint[0, 1] + 15;
            int[] areaPoints = new int[] { min_x, max_x, min_y, max_y };
            if (!CheckIfNotOutOfBounds(areaPoints, x, y) && id == 5)
            {
                MessageBox.Show("Object can't reach this far", "Shooting error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                e.BackColor = Color.Red;
                for (int i = min_x; i < max_x; i++)
                {
                    for (int j = min_y; j < max_y; j++)
                    {
                        Point myPoint = new Point(25 * j, 25 * i);
                        Label update_label = e.GetChildAtPoint(myPoint) as Label;
                        update_label.BorderStyle = BorderStyle.Fixed3D;
                    }
                }
            }
        }
        private bool CheckIfNotOutOfBounds(int[] arr, int x, int y)
        {
            int counter = 0;
            for (int i = arr[0]; i < arr[1]; i++)
            {
                for (int j = arr[2]; j < arr[3]; j++)
                {
                    if (i == x && j == y) counter++;
                }
            }
            if (counter > 0) return true;
            else return false;
        }
        private int[,] ReturnEndCoordinate(int[,] arr)
        {
            int[,] endCoordinate = new int[1, 2];
            int max = arr[0, 0] + arr[0, 1];
            for (int i = 1; i < arr.GetLength(0); i++)
            {
                if (arr[i, 0] + arr[i, 1] > max)
                {
                    max = arr[i, 0] + arr[i, 1];
                    endCoordinate[0, 0] = arr[i, 0];
                    endCoordinate[0, 1] = arr[i, 1];
                }
            }
            return endCoordinate;
        }

        public override void Shoot()
        {
            Console.WriteLine("Plane Shot");
        }
    }
}

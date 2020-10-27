using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Checker
{
    class IsGroundUsed
    {
        public bool isFree(int[,] map, int x, int y, int row, int column)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    int newx = (x + j);
                    int newy = (y + i);
                    if (map[newy, newx] != 0 && map[newy, newx] != 7 && map[newy, newx] != 8 && map[newy, newx] != 9 && map[newy, newx] != 10)
                        return false;
                }
            }
            return true;
        }
    }
}

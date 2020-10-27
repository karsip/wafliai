using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Checker
{
    class IsGoodType
    {
        public bool isOnGoodTile(int x, int y, int row, int column, int type)
        {
            if (type == 4 || type == 5 || type == 7)
            {
                return true;
            }
            else if (type == 6)
            {
                if (y + row - 1 < 20 || y >= 42)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if ((y >= 20 && y + row - 1 < 42))
                {
                    return true;
                }
                return false;
            }
        }
    }
}

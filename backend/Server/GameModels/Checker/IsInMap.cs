using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Checker
{
    class IsInMap
    {
        public bool inside(int x, int y, int row, int col)
        {
            if ((x + col - 1) < 64 && (y + row - 1) < 64)
                return true;
            return false;
        }
    }
}

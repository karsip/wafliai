using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Mine_explosion
{
    public class SmallExplosion : IMineStrategy
    {
        public int[,] CalculateArea(int[,] map, int Y, int X)
        {
            map[Y, X] = 8;
            return map;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Mine_explosion
{
    public class HugeExplosion : IMineStrategy
    {
        public int[,] CalculateArea(int[,] map, int Y, int X)
        {
            Y--;
            X--;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if(Y + i >= 0 && X + j >= 0 && Y + i < 64 && X + j <64)
                    map[Y+i, X+j] = 10;
                }
            }
            return map;
        }
    }
}

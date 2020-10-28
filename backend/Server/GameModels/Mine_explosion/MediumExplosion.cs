using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Mine_explosion
{
    public class MediumExplosion : IMineStrategy
    {
        public int [,] CalculateArea(int[,] map, int Y, int X)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if(Y+i<64 && X+j<64)
                    map[Y+i, X+j] = 9;
                }
            }
            return map;
        }
    }
}

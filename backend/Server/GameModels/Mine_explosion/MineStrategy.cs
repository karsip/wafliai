using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Mine_explosion
{
    public class MineStrategy
    {
        private IMineStrategy _mineStrategy;
        public MineStrategy() { }
        public MineStrategy(IMineStrategy mineStrategy) 
        {
            this._mineStrategy = mineStrategy;
        }
        public void SetMineStrategy(IMineStrategy mineStrategy)
        {
            this._mineStrategy = mineStrategy;
        }
        public int [,] ExplodeMine( int[,] map, int Y, int X) 
        {
            return this._mineStrategy.CalculateArea(map, Y, X);
        }
    }
}

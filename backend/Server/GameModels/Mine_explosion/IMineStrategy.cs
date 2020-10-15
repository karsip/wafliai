using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Mine_explosion
{
    public interface IMineStrategy
    {
        public int[] CalculateArea(int size, int damage);
    }
}

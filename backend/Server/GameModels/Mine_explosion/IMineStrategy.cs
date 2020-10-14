using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Mine_explosion
{
    public interface IMineStrategy
    {
        public int[] calculateArea(int size, int damage);
    }
}

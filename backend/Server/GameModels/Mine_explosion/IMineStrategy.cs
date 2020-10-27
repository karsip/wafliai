using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Mine_explosion
{
    public interface IMineStrategy
    {
        public void CalculateArea(int size);
    }
}

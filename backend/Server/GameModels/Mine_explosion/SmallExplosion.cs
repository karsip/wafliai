using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Mine_explosion
{
    public class SmallExplosion : IMineStrategy
    {
        public void CalculateArea(int size)
        {
            int[] explosion_damage = new int[] { 1, 1 };
            Console.WriteLine("Small");
        }
    }
}

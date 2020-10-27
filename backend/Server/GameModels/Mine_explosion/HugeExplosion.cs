using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Mine_explosion
{
    public class HugeExplosion : IMineStrategy
    {
        public void CalculateArea(int size)
        {
            int[] explosion_damage = new int[] { 3, 3 };
            Console.WriteLine("Huge");
        }
    }
}

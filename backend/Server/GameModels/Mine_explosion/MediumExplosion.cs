using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Mine_explosion
{
    public class MediumExplosion : IMineStrategy
    {
        public void CalculateArea(int size)
        {
            int[] explosion_damage = new int[] { 2, 2 };
            Console.WriteLine("Medium");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Mine_explosion
{
    public class SmallExplosion : IMineStrategy
    {
        public int[] calculateArea(int size, int damage)
        {
            int[] explosion_damage = new int[] { 1, 1};
            return explosion_damage;
        }
    }
    public class MediumExplosion : IMineStrategy
    {
        public int[] calculateArea(int size, int damage)
        {
            int[] explosion_damage = new int[] { 2, 2 };
            return explosion_damage;
        }
    }
    public class HugeExplosion : IMineStrategy
    {
        public int[] calculateArea(int size, int damage)
        {
            int[] explosion_damage = new int[] { 3, 3 };
            return explosion_damage;
        }
    }
}

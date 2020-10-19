using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Checker
{
    public class CanBePlaced
    {
        private IsInMap _isInMap = new IsInMap();
        private IsGoodType _isGoodType = new IsGoodType();
        private IsGroundUsed _isGroundUsed = new IsGroundUsed();
        public bool IsEligible(int[,] map, int x, int y, int row, int column, int type) 
        {
            Console.WriteLine("X: " + x);
            Console.WriteLine("Y: " + y);
            Console.WriteLine("row: " + row);
            Console.WriteLine("column: " + column);
            Console.WriteLine("type: " + type);
            bool eligible = true;
            if (!_isInMap.inside(x, y, row, column))
            {
                eligible = false;
            }
            else if (!_isGoodType.isOnGoodTile(x, y, row, column, type))
            {
                eligible = false;
            }
            else if (!_isGroundUsed.isFree(map, x, y, row, column))
            {
                eligible = false;
            }
            return eligible;
        }
    }
}

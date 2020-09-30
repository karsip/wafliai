using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShipModels.GroundTypes
{
    public class GroundFactory : Factory
    {
        public override Ground CreateGround(int type)
        {
            switch (type)
            {
                case 1:
                    return new Water();
                case 2:
                    return new Grass();
                case 3:
                    return new Earth();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

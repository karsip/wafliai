using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShipModels.GroundTypes
{
    public abstract class Factory
    {
        public abstract Ground CreateGround(int type);
    }
}

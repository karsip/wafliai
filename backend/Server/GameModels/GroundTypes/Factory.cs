using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.GroundTypes
{
    public abstract class Factory
    {
        public abstract Ground CreateGround(int type);
    }
}

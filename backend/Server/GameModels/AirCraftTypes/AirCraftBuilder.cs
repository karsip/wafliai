using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.AirCraftTypes
{
    public abstract class AirCraftBuilder
    {
        protected AirCraft airCraft;
        public AirCraft AirCraft 
        {
            get { return airCraft; }
        }
        public abstract void BuildNose();
        public abstract void BuildWings();
        public abstract void BuildTail();
    }
}

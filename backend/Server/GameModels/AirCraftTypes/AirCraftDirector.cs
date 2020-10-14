using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.AirCraftTypes
{
    public class AirCraftDirector
    {
        public void Construct(AirCraftBuilder airCraftBuilder) 
        {
            airCraftBuilder.BuildNose();
            airCraftBuilder.BuildWings();
            airCraftBuilder.BuildTail();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameModels.AirCraftTypes
{
     public class JetBuilder : AirCraftBuilder
    {
        public JetBuilder()
        {
            airCraft = new AirCraft("Jet");
        }
        public override void BuildNose()
        {
            airCraft["noseL"] = Image.FromFile("../../../GameModels/Textures/plane/plane3.png");
            airCraft["noseR"] = Image.FromFile("../../../GameModels/Textures/plane/plane6.png");
        }
        public override void BuildWings()
        {
            airCraft["wingL"] = Image.FromFile("../../../GameModels/Textures/plane/plane2.png");
            airCraft["wingR"] = Image.FromFile("../../../GameModels/Textures/plane/plane5.png");
        }
        public override void BuildTail()
        {
            airCraft["TailL"] = null;
            airCraft["TailR"] = null;
        }
    }
}

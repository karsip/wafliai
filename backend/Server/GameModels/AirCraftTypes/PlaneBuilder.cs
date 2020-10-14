using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace GameModels.AirCraftTypes
{
    public class PlaneBuilder : AirCraftBuilder
    {
        public PlaneBuilder() 
        {
            airCraft = new AirCraft("Plane");
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
            airCraft["tailL"] = Image.FromFile("../../../GameModels/Textures/plane/plane1.png");
            airCraft["tailR"] = Image.FromFile("../../../GameModels/Textures/plane/plane4.png");
        }
    }
}

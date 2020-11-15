using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameModels.FlyWeight
{
    public class FlyWeightGroundFactory
    {
        private Dictionary<string, GroundImage> _flyWeight = new Dictionary<string, GroundImage>();
        public GroundImage GetGround(string name) 
        {
            GroundImage img = null;
            if (_flyWeight.ContainsKey(name))
            {
                img = _flyWeight[name];
            }
            else 
            {
                switch (name) 
                {
                    case "Grass": img = new GrassImage();break;
                    case "Sand": img = new SandImage(); break;
                    case "Water": img = new WaterImage(); break;
                }
                _flyWeight.Add(name, img);
            }
            return img;
        }
    }
}

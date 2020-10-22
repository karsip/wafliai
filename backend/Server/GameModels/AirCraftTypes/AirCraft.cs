using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameModels.AirCraftTypes
{
    public  class AirCraft
    {
        private string _airCraftType;
        private Dictionary<string, Image> _parts =
        new Dictionary<string,Image>();
        public AirCraft(string airCraftType) 
        {
            this._airCraftType = airCraftType;
        }
        public Image this[string key] 
        {
            get { return _parts[key]; }
            set { _parts[key] = value; }
        }
        public Bitmap Show() 
        {
            if (_airCraftType == "Plane")
            {
                Bitmap bmp = new Bitmap(75, 50);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(_parts["noseL"], 50, 0, 25, 25);
                        g.DrawImage(_parts["noseR"], 50, 25, 25, 25);
                        g.DrawImage(_parts["wingL"], 25, 0, 25, 25);
                        g.DrawImage(_parts["wingR"], 25, 25, 25, 25);
                        g.DrawImage(_parts["tailL"], 0, 0, 25, 25);
                        g.DrawImage(_parts["tailR"], 0, 25, 25, 25);
                    }
                return bmp;
            }
            else 
            {
                Bitmap bmp = new Bitmap(50, 50);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(_parts["noseL"], 25, 0, 25, 25);
                        g.DrawImage(_parts["noseR"], 25, 25, 25, 25);
                        g.DrawImage(_parts["wingL"], 0, 0, 25, 25);
                        g.DrawImage(_parts["wingR"], 0, 25, 25, 25);
                    }
                return bmp;
            }
        }
    }
}

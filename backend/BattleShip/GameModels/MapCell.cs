using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels
{
    [Serializable]
    public class MapCell
    {
        public Player player { get; set; }

        public MapObjectInterface mapObject { get; set; }
    }
}

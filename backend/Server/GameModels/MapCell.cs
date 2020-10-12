using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels
{
    [Serializable]
    public class MapCell
    {
        public MapObjectInterface mapObject { get; set; }
    }
}

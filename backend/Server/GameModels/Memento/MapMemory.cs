using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Memento
{
    public class MapMemory
    {
        private MapMemento mapMemento;
        public MapMemento Memento
        {
            set { mapMemento = value; }
            get { return mapMemento; }
        }
    }
}

using GameModels.GroundTypes;
using System;

namespace GameModels
{
    public class Map
    {
        private Int32 colNumber = 64;
        private Int32 rowNumber = 64;
        private MapCell[][] MapObjects { get; set; }
        public Map()
        {
            MapObjects = new MapCell[rowNumber][];
            for (int i = 0; i < rowNumber; i++)
            {
                MapObjects[i] = new MapCell[colNumber];
                for (int j = 0; j < colNumber; j++)
                {
                    MapObjects[i][j] = new MapCell();
                }
            }
            FillMap();
        }
        public MapCell[][] GetMapObjects()
        {
            return MapObjects;
        }
        private void FillMap()
        {
            var groundFactory = new GroundFactory();

            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    int groundType;
                    if (i >= 0 && i < 10 || i >= 52 && i < 64)
                    {
                        // grass
                        groundType = 2;
                    }
                    else if (i >= 10 && i < 20 || i >= 42 && i < 52)
                    {
                        // sand
                        groundType = 3;
                    }
                    else
                    {
                        // water
                        groundType = 1;
                    }
                    MapObjects[i][j].mapObject = groundFactory.CreateGround(groundType);
                }
            }
        }
    }
}

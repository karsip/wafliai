using GameModels.GroundTypes;
using GameModels.ObjectObserver;
using System;
using GameModels.Iterator;

namespace GameModels
{
    public class Map : Subject
    {
        private static int colNumber = 64;
        private static int rowNumber = 64;
        private Matrix MapObjects = new Matrix(rowNumber, colNumber);
        public Map()
        {
            FillMap();
        }
        public Matrix GetMapObjects()
        {
            return MapObjects;
        }
        private void FillMap()
        {
            Iterate iterator = MapObjects.CreateIterator();
            var groundFactory = new GroundFactory();

            for (iterator.First(); !iterator.IsDone; iterator.Next())
            {
                    int groundType;
                    if (iterator.Row >= 0 && iterator.Row < 10 || iterator.Row >= 52 && iterator.Row < 64)
                    {
                        // grass
                        groundType = 2;
                    }
                    else if (iterator.Row >= 10 && iterator.Row < 20 || iterator.Row >= 42 && iterator.Row < 52)
                    {
                        // sand
                        groundType = 3;
                    }
                    else
                    {
                        // water
                        groundType = 1;
                    }
                    iterator.CurrentItem = groundFactory.CreateGround(groundType);
            }
        }
    }
}

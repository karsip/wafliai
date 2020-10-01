using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShipModels.GroundTypes;

namespace BattleShipModels
{
    [Serializable]
    public sealed class Map
    {
        private static readonly Map mapInstance = new Map();

        private Int32 colNumber = 64;
        private Int32 rowNumber = 64;
        private List<Player> Players { get; set; }
        private MapCell[][] MapObjects { get; set; }
        public static Map MapInstance { get { return mapInstance; } }

        public MapCell[][] GetMapObjects()
        {
            return MapObjects;
        }

        private Map()
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
            Players = new List<Player>();
            FillMap();
        }


        public void AddPlayer(string id)
        {
            Player player = new Player(id);

            Players.Add(player);
        }
        private void FillMap()
        {
            var groundFactory = new GroundFactory();

            Random rnd = new Random();

            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    int groundType = rnd.Next(1, 4);
                    MapObjects[i][j].mapObject = groundFactory.CreateGround(groundType);
                }
            }
        }
    }
}

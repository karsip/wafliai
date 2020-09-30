using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShipModels
{
    [Serializable]
    public sealed class Map
    {
        private static readonly Map mapInstance = new Map();
        private List<Player> Players { get; set; }
        private MapCell[][] MapObjects { get; set; }

        static Map() { }
        public static Map MapInstance { get { return mapInstance; } }

        private Player GetPlayer(string id)
        {
            return Players.FirstOrDefault(player => player.Id == id);
        }
        public MapCell[][] GetMapObjects()
        {
            return MapObjects;
        }

        public void AddPlayer(string id)
        {
            Player player = new Player(id);

            Players.Add(player);
        }

    }
}

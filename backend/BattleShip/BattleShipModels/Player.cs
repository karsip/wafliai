using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShipModels
{
    public class Player
    {
        public string Id { get; set; }
        private int lifePoints { get; set; }

        public Player(string id)
        {
            Id = id;
        }
    }
}

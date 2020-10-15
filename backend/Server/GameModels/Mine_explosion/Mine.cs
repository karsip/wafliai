using GameModels.Mine_explosion;
using GameModels.ObjectObserver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Bomb_explosion
{
    class Mine : IObserver
    {
        public int x_coordinate {get; set;}
        public int y_coordinate {get; set;}

        private int currentTime = 0;
        private int explosion_effect_time = 1;
        public IMineStrategy strategy { get; set; }

        private Map gameMap;
        public Mine() { }

        public Mine(int x_coordinate, int y_coordinate, IMineStrategy strategy, Map game_map)
        {
            this.x_coordinate = x_coordinate;
            this.y_coordinate = y_coordinate;
            this.strategy = strategy;

            gameMap = game_map;
            gameMap.Attach(this);
           // add it to game map
        }

        public int[] expliotionDamageArea(int size, int damage)
        {
            return strategy.CalculateArea(size, damage);
        }

        public async Task Update()
        {
            currentTime++;
            if(currentTime == explosion_effect_time)
            {
                // need to detach
                gameMap.Detach(this);
                // do something with mine effects
                // await doAction();
            }
        }
    }
}

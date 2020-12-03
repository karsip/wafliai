using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Memento
{
    public class MapOriginator
    {
        private int[,] gameMap;
        private bool mLock;
        private int lifePoints;

        public bool Status
        {
            get { return mLock; }
            set

            {
                mLock = value;
            }
        }
        // nemanau, kad reikia 
        public int[,] GameMap
        {
            get { return gameMap; }
            set
            {
                gameMap = value;
            }
        }
        public int LifePoints
        {
            get { return lifePoints; }
            set
            {
                lifePoints = value;
            }
        }
        public MapMemento SaveMemento()
        {
            return new MapMemento(gameMap, mLock, lifePoints);
        }

        public void RestoreMemento(MapMemento memento)
        {
            this.GameMap = memento.GameMap;
            this.Status = memento.Status;
            this.LifePoints = memento.LifePoints;
        }

    }
}

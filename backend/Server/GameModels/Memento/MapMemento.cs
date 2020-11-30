using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Memento
{
    public class MapMemento
    {
        private int[,] gameMap;
        private bool mLock;
        private int lifePoints;

        public MapMemento(int[,] matrix, bool mLock, int lifePoints)
        {
            gameMap = matrix;
            this.mLock = mLock;
            this.lifePoints = lifePoints;
        }

        public int[,] GameMap
        {
            get { return gameMap; }
            set { gameMap = value; }
        }
        public bool Status
        {
            get { return mLock; }
            set { mLock = value; }
        }
        public int LifePoints
        {
            get { return lifePoints; }
            set { lifePoints = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GameModels
{
     public class PlayerData
    {
        private int[] unitCount { get; set; }
        private int livePoints { get; set; }
        private bool isYourTurn { get; set; }
        private int[][] shipsLocation { get; set; }
        private bool isLost { get; set; }

        public PlayerData()
        {
            for (int i = 0; i < 6; i++) 
            {
                unitCount[i] = 0;
            }
            livePoints = 10;
            isYourTurn = true;
            isLost = false;
        }
        public PlayerData(int[] UnitCount, int LivePoints, bool IsYourTurn, int[][] ShipLocation, bool IsLost)
        {
            unitCount = UnitCount;
            livePoints = LivePoints;
            isYourTurn = IsYourTurn;
            shipsLocation = ShipLocation;
            isLost = IsLost;
        }
        public int[] GetUnitCount() { return (unitCount); }
        public int GetLivePoints() { return (livePoints); }
        public bool GetIsYourTurn() { return (isYourTurn); }
        public int[][] GetShipsLocation() { return (shipsLocation); }
        public bool GetIsLost() { return (isLost); }
    }
}

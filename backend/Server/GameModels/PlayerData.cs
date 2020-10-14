using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GameModels
{
     public class PlayerData
    {
        private int[] unitCount;
        private int livePoints { get; set; }
        private bool isYourTurn { get; set; }
        private int[][] shipsLocation { get; set; }
        private bool isLost { get; set; }

        public PlayerData()
        {
            unitCount = new int[6];
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
        public override string ToString()
        {
            return "Life points: " + livePoints.ToString() + "  and is he lost: " + isLost.ToString();
        }
    }
}

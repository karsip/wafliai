using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace GameModels
{
    [Serializable]
     public class PlayerData
    {
        private int[] unitCount;
        private int livePoints { get; set; }
        private bool isYourTurn { get; set; }
        private int[,,] objectLocations { get; set; }
        private bool isLost { get; set; }

        private string username { get; set; }
        private Socket socket { get; set; }

        public PlayerData(string username)
        {
            this.username = username;
        }
        public PlayerData(Socket socket)
        {
            this.socket = socket;
            unitCount = new int[6];
            for (int i = 0; i < 6; i++)
            {
                unitCount[i] = 0;
            }
            livePoints = 10;
            isYourTurn = true;
            isLost = false;
            objectLocations = new int[0,0,0];
        }
        public string getUsername()
        {
            return this.username;
        }
        public Socket getSocket()
        {
            return this.socket;
        }
        public void SetUsername(string username)
        {
            this.username = username;
        }
        public PlayerData(string username, Socket socket)
        {
            this.socket = socket;
            this.username = username;
            unitCount = new int[6];
            for (int i = 0; i < 6; i++) 
            {
                unitCount[i] = 0;
            }
            livePoints = 10;
            isYourTurn = true;
            isLost = false;
        }
        public PlayerData(int[] UnitCount, int LivePoints, bool IsYourTurn, int[,,] objectLocation, bool IsLost)
        {
            unitCount = UnitCount;
            livePoints = LivePoints;
            isYourTurn = IsYourTurn;
            this.objectLocations = objectLocation;
            isLost = IsLost;
        }
        public PlayerData() { }
        public void SetNewUnitPoints(int [] unitPoints)
        {
            this.unitCount = unitPoints;
        }
        public void SetNewLifePoints(int lifePoints)
        {
            this.livePoints = lifePoints;
        }
        public void SetIsYourTurn(bool turn)
        {
            isYourTurn = turn;
        }

        public int[] GetUnitCount() { return (unitCount); }
        public int GetLivePoints() { return (livePoints); }
        public bool GetIsYourTurn() { return (isYourTurn); }
        public int[,,] GetObjectLocations() { return (objectLocations); }

        public void setObjectLocations(int object_id, int x, int y)
        {
            int current_Length = objectLocations.GetLength(0);
            objectLocations[current_Length, 1, 0] = x;
            objectLocations[current_Length, 1, 1] = y;
            objectLocations[current_Length, 1, 2] = object_id;
        }
        public int getLocationsArrLength()
        {
            return objectLocations.GetLength(0);
        }
        public bool GetIsLost() { return (isLost); }
        public override string ToString()
        {
            return "Life points: " + livePoints.ToString() + "  and is he lost: " + isLost.ToString() + "  Username: " + username;
        }
    }
}

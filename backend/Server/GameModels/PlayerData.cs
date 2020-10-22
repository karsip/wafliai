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
        public int[] unitCount;
        public int livePoints;

        public bool isYourTurn;
        public int[,,] objectLocations;
        public bool isLost;

        public string username;
        public Socket socket;

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
                switch (i)
                {
                    case 0:
                    case 1:
                    case 3:
                    case 4:
                        //planes, ShipCarrier, shipDestroyer
                        unitCount[i] = 2;
                        break;
                    case 2:
                    case 5:
                        // soldiers submarines
                        unitCount[i] = 3;
                        break;
                    case 6:
                        // mines
                        unitCount[i] = 5;
                        break;
                    default: break;  
                }
            }
            livePoints = 10;
            isYourTurn = true;
            isLost = false;
            objectLocations = new int[0,0,0];
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
        public PlayerData(int[] UnitCount, int LivePoints, bool IsYourTurn, int[,,] objectLocation, bool IsLost, string username)
        {
            this.username = username;
            unitCount = UnitCount;
            livePoints = LivePoints;
            isYourTurn = IsYourTurn;
            this.objectLocations = objectLocation;
            isLost = IsLost;
        }
        public PlayerData() { }
        
        /* public void setObjectLocations(int object_id, int x, int y)
        {
            int current_Length = objectLocations.GetLength(0);
            objectLocations[current_Length, 1, 0] = x;
            objectLocations[current_Length, 1, 1] = y;
            objectLocations[current_Length, 1, 2] = object_id;
        } */
        public override string ToString()
        {
            return "Life points: " + livePoints.ToString() + "  and is he lost: " + isLost.ToString() + "  Username: " + username;
        }
    }
}

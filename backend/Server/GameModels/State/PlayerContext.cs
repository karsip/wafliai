using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.State
{
    public class PlayerContext
    {
        private PlayerState playerState;
        private int currentState;

        public PlayerContext(PlayerState playerState, int currentState)
        {
            this.playerState = playerState;
            this.currentState = currentState;
        }

        public PlayerState State
        {
            get { return playerState; }
            set

            {
                playerState = value;
                Console.WriteLine("State: " +
                  playerState.GetType().Name);
            }
        }
        public String StateReturner()
        {
            return playerState.getName();
        }
        public void Request(int val)
        {
            playerState.HandleCurrentState(this, val);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.State
{
    public class OnFireState : PlayerState
    {
        private string name;

        public OnFireState(string name)
        {
            this.name = name;
        }
        public override string getName()
        {
            return name;
        }

        public override void HandleCurrentState(PlayerContext playerContext, int currentState)
        {
            switch (currentState)
            {
                case 1:
                    playerContext.State = new RampageState("Rampage");
                    break;
                default:
                    playerContext.State = new NewbieState("Newbie");
                    break;
            }
        }
    }
}

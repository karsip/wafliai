using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.State
{
    public class RampageState : PlayerState
    {
        private string name;

        public RampageState(string name)
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
                case -1:
                    playerContext.State = new OnFireState("On fire");
                    break;
                default:
                    break;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.State
{
    public class AlarmedState : PlayerState
    {
        private string name;
        public override string getName()
        {
            return name;
        }
        public AlarmedState(string name)
        {
            this.name = name;
        }

        public override void HandleCurrentState(PlayerContext playerContext, int currentState)
        {
            switch (currentState)
            {
                case 1:
                    playerContext.State = new NewbieState("Newbie");
                    break;
                default:
                    playerContext.State = new AgonyState("Agony");
                    break;
            }
        }
    }
}

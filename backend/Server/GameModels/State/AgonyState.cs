using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.State
{
    public class AgonyState : PlayerState
    {
        private string name;
        public AgonyState(string name)
        {
            this.name = name;
        }
        public override void HandleCurrentState(PlayerContext playerContext, int currentState)
        {
            switch (currentState)
            {
                case 1:
                    playerContext.State = new AlarmedState("Alarmed");
                    break;
                default:
                    break;
            }
        }
        public override string getName()
        {
            return name;
        }
    }
}

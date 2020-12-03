using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.State
{
    public abstract class PlayerState
    {
        public abstract void HandleCurrentState(PlayerContext playerContext, int currentState);
        public abstract String getName();
    }
}

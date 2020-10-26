using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Command
{
    public abstract class MoveCommand
    {
        protected MoveReceiver receiver;

        public MoveCommand(MoveReceiver receiver)
        {
            this.receiver = receiver;
        }
        public abstract int[,] Ecexute();
    }
}

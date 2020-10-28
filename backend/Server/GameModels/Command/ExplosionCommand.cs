using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Command
{
    public class ExplosionCommand : MoveCommand
    {
        public ExplosionCommand(MoveReceiver receiver) : base(receiver)
        {

        }
        public override int[,] Ecexute()
        {
            return receiver.Explode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Command
{
    public class MoveToCommand : MoveCommand
    {
        public MoveToCommand(MoveReceiver receiver) : base(receiver)
        {

        }
        public override int[,] Ecexute()
        {
            return receiver.Move();
        }
    }
}

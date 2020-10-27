using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Command
{
    public class UndoCommand : MoveCommand
    {
        public UndoCommand(MoveReceiver receiver) : base(receiver)
        {

        }
        public override int[,] Ecexute()
        {
            return receiver.Undo();
        }
    }
}

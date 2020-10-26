using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Command
{
    public class MoveInvoker
    {
        private MoveCommand moveCommand;
        public void SetCommand(MoveCommand command)
        {
            moveCommand = command;
        }
        public int[,] ExecuteCommand()
        {
            return moveCommand.Ecexute();
        }
    }
}

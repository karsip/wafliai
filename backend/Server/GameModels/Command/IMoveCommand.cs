using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Command
{
    public interface IMoveCommand
    {
        void Execute();
        void Undo();
    }
}

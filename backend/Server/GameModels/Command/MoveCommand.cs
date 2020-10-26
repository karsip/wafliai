using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Command
{
    public class MoveCommand : IMoveCommand
    {
        private MovementReceiver _receiver;
        private int object_id;
        private int x;
        private int y;
        private int[,] mapArr;
        public MoveCommand(int [,] playerGameMap, MovementReceiver receiver, int object_id, int x, int y)
        {
            this.x = x;
            this.y = y;
            this.object_id = object_id;
            _receiver = receiver;
            mapArr = playerGameMap;
        }
        public void Execute()
        {
            _receiver.MoveTo(mapArr, object_id, x, y);
            Console.WriteLine("Command -> array size is: {0}x{1}", mapArr.GetLength(0), mapArr.GetLength(1));

        }
    }
}

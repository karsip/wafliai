using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Command
{
    public class MovementInvoker
    {
        private IMoveCommand _onStart;
        private IMoveCommand _onFinish;

        public void SetOnStart(IMoveCommand command)
        {
            this._onStart = command;
        }

        public void SetOnFinish(IMoveCommand command)
        {
            this._onFinish = command;
        }
        public void DoSomethingImportant()
        {
            Console.WriteLine("Invoker: Does anybody want something done before I begin?");
            if (this._onStart is IMoveCommand)
            {
                this._onStart.Execute();
            }

            Console.WriteLine("Invoker: ...doing something really important...");

            Console.WriteLine("Invoker: Does anybody want something done after I finish?");
            /* if (this._onFinish is IMoveCommand)
            {
                this._onFinish.Execute();
            } */
        }

    }
}

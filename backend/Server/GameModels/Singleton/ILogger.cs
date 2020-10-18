using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Singleton
{
    public interface ILogger
    {
        void LogException(string messagge);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.ObjectObserver
{
    public interface IObserver
    {
        Task Update();
    }
}

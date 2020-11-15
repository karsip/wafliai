using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Iterator
{
    interface IAbstractMatrix
    {
        Iterate CreateIterator();
    }
}

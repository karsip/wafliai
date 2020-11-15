using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Iterator
{
    interface IAbstractIterate
    {
        object First();
        object Next();
        bool IsDone{get;}
        int Row { get; }
        object CurrentItem { get; set; }
    }
}

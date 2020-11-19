using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.RequestAdapter
{
    public class RequestAdapter : IAdapterInrterface
    {
        private readonly Adaptee adaptee;
        public RequestAdapter(Adaptee adaptee)
        {
            this.adaptee = adaptee;
        }
        public int[,] getRequest()
        {
            return adaptee.BufferToArray();
        }
    }
}

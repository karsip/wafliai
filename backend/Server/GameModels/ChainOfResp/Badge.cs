using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GameModels.ChainOfResp
{
    public abstract class Badge
    {
        protected Badge successor;
        public void SetSuccessor(Badge successor)
        {
            this.successor = successor;
        }
        public abstract void SetBadge(ref String badgeImg, int LifePoints);
    }
}

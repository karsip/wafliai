using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GameModels.ChainOfResp
{
    public class LowBadge : Badge
    {
        //public override void SetBadge(Button label, int lifePoints)
        // {
        /*
        if (lifePoints <= 10)
        {
            label.BackgroundImage = Image.FromFile("../../../GameModels/Textures/Badges/LowBadge.png");
        }
        else if (successor != null)
        {
            successor.SetBadge(label, lifePoints);
        }
        else
        {
            label.BackgroundImage = null;
        }
        */
        //}
        public override void SetBadge(ref string badgeImg, int lifePoints)
        {
            if (lifePoints <= 10)
            {
                badgeImg = "../../../GameModels/Textures/Badges/LowBadge.png";
            }
            else if (successor != null)
            {
                successor.SetBadge(ref badgeImg, lifePoints);
            }
        }
    }
}

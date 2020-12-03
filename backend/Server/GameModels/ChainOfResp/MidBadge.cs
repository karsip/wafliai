using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GameModels.ChainOfResp
{
    public class MidBadge : Badge
    {
        //public override void SetBadge(Button BadgePlace, int lifePoints)
        // {
        /*
        if (lifePoints > 10 && lifePoints < 14)
        {
            BadgePlace.BackgroundImage = Image.FromFile("../../../GameModels/Textures/Badges/MidBadge.png");
        }
        else if (successor != null)
        {
            successor.SetBadge(BadgePlace, lifePoints);
        }
        */
        // }
        public override void SetBadge(ref string badgeImg, int lifePoints)
        {
            if (lifePoints > 10 && lifePoints < 14)
            {
                badgeImg = "../../../GameModels/Textures/Badges/MidBadge.png";
            }
            else if (successor != null)
            {
                successor.SetBadge(ref badgeImg, lifePoints);
            }
        }
    }
}

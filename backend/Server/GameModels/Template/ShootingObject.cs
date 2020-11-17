using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Template
{
    abstract class ShootingObject
    {
        // after player clicks on map when object is selected
        public abstract void RenderBeforeShot();
        // do changes to both maps
        public abstract void Shoot();
        // render changes
        public abstract void RenderAfterShot();
    }
}

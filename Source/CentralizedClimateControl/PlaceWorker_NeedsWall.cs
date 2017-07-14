using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CentralizedClimateControl
{
    public class PlaceWorker_NeedsWall : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot,
            Thing thingToIgnore = null)
        {
            IntVec3 c = center;
            Building wall = c.GetEdifice(this.Map);

            if (wall == null)
            {
                return false;
            }

            return true;
        }
    }
}

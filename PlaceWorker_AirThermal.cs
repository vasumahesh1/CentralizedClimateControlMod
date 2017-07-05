using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedTemperature
{
    public class PlaceWorker_AirThermal: PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Thing thingToIgnore = null)
        {
            List<Thing> thingList = center.GetThingList(base.Map);

            foreach (var thing in thingList)
            {
                if (thing is Building_AirPipe)
                {
                    return AcceptanceReport.WasRejected;
                }
            }

            return true;
        }
    }
}

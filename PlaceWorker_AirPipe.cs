using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace EnhancedTemperature
{
    public class PlaceWorker_AirPipe: PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null)
        {
            List<Thing> thingList = loc.GetThingList(base.Map);

            foreach (var thing in thingList)
            {
                if (thing is Building_AirFlowControl)
                {
                    return AcceptanceReport.WasRejected;
                }
            }

            return AcceptanceReport.WasAccepted;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedTemperature
{
    public class PlaceWorker_AirPipe: PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null)
        {
            CompProperties_AirFlow compProperties = (def as ThingDef).GetCompProperties<CompProperties_AirFlow>();

            var mapComponent = EnhancedTemperatureUtility.GetNetManager(base.Map);

            if (mapComponent.ZoneAt(loc, compProperties.flowType) || mapComponent.ZoneAt(loc, AirFlowType.Any))
            {
                return false;
            }

            return true;
        }
    }
}

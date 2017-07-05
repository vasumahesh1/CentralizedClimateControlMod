using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedTemperature
{
    public class PlaceWorker_AirVent : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            IntVec3 intVec = center + IntVec3.South.RotatedBy(rot);

            GenDraw.DrawFieldEdges(new List<IntVec3>
            {
                intVec
            }, Color.yellow);

            RoomGroup roomGroup = intVec.GetRoomGroup(base.Map);
            if (roomGroup != null)
            {
                if (!roomGroup.UsesOutdoorTemperature)
                {
                    GenDraw.DrawFieldEdges(roomGroup.Cells.ToList<IntVec3>(), Color.yellow);
                }
            }
        }

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

            IntVec3 vec = center + IntVec3.South.RotatedBy(rot);

            if (vec.Impassable(base.Map))
            {
                return "EnhancedTemperature.Consumer.AirVentPlaceError".Translate();
            }

            return true;
        }
    }
}

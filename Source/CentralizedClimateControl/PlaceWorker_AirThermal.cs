using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace CentralizedClimateControl
{
    public class PlaceWorker_AirThermal: PlaceWorker
    {

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            if (def == null)
            {
                return;
            }

            var size = def.size;

            List<IntVec3> list = new List<IntVec3>();

            IntVec3 iterator = new IntVec3(center.x, center.y, center.z);

            for (int dx = 0; dx < size.x; dx++)
            {
                IntVec3 intVec = iterator + IntVec3.South.RotatedBy(rot);
                list.Add(intVec);

                iterator += IntVec3.East.RotatedBy(rot);
            }

            GenDraw.DrawFieldEdges(list, Color.red);
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

            if (def == null)
            {
                return AcceptanceReport.WasRejected;
            }

            var size = def.Size;

            IntVec3 iterator = new IntVec3(center.x, center.y, center.z);

            for (int dx = 0; dx < size.x; dx++)
            {
                IntVec3 intVec = iterator + IntVec3.South.RotatedBy(rot);

                if (intVec.Impassable(base.Map))
                {
                    return "CentralizedClimateControl.Consumer.AirThermalPlaceError".Translate();
                }

                iterator += IntVec3.East.RotatedBy(rot);
            }

            return true;
        }
    }
}

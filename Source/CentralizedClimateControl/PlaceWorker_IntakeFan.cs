using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace CentralizedClimateControl
{
    class PlaceWorker_IntakeFan : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            if (def == null)
            {
                return;
            }

            var size = def.size;

            var list = GenAdj.CellsAdjacent8Way(center, rot, size);
            GenDraw.DrawFieldEdges(list.ToList(), Color.white);
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
            var list = GenAdj.CellsAdjacent8Way(center, rot, size);

            foreach (var intVec in list)
            {
                if (intVec.Impassable(base.Map))
                {
                    return "CentralizedClimateControl.Producer.IntakeFanPlaceError".Translate();
                }
            }

            return true;
        }
       
    }
}

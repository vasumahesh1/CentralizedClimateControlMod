using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace CentralizedClimateControl
{
    public class PlaceWorker_IntakeFan : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            var list = new List<IntVec3>();

            for (int i = 0; i < 8; i++)
            {
                IntVec3 intVec = center + GenAdj.AdjacentCellsAround[i];
                list.Add(intVec);
            }

            GenDraw.DrawFieldEdges(list, Color.white);
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

            for (int i = 0; i < 8; i++)
            {
                IntVec3 intVec = center + GenAdj.AdjacentCellsAround[i];

                if (intVec.Impassable(base.Map))
                {
                    return "CentralizedClimateControl.Producer.IntakeFanPlaceError".Translate();
                }
            }

            return true;
        }
    }
}

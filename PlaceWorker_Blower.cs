using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedTemperature
{
    public class PlaceWorker_Blower : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            // GenDraw.DrawFieldEdges(WindTurbineUtility.CalculateWindCells(center, rot, def.size).ToList<IntVec3>());
        }
    }
}

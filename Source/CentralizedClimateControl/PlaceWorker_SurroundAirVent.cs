﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace CentralizedClimateControl
{
    public class PlaceWorker_SurroundAirVent : PlaceWorker
    {
        /// <summary>
        /// Draw Overlay when Selected or Placing.
        /// 
        /// Here we just draw a red/blue/cyan cell (based on Network flow type) towards the North. To indicate Exhaust.
        /// </summary>
        /// <param name="def">The Thing's Def</param>
        /// <param name="center">Location</param>
        /// <param name="rot">Rotation</param>
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
        {
            if (def == null)
            {
                return;
            }

            var type = AirFlowType.Hot;

            Map map = Find.CurrentMap;

            var list = center.GetThingList(map);
            foreach (var thing in list)
            {
                if (!(thing is Building_AirVent))
                {
                    continue;
                }

                var airVent = thing as Building_AirVent;

                if (airVent.CompAirFlowConsumer.AirFlowNet != null)
                {
                    type = airVent.CompAirFlowConsumer.AirFlowNet.FlowType;
                }

                break;
            }


            var size = def.size;
            var surroundCells = GenAdj.CellsAdjacent8Way(center, rot, size).ToList();

            var typeColor = type == AirFlowType.Hot ? Color.red : type == AirFlowType.Cold ? Color.blue : Color.cyan;

            GenDraw.DrawFieldEdges(surroundCells, typeColor);

            var roomGroup = surroundCells[0].GetRoomGroup(map);
            if (roomGroup == null)
            {
                return;
            }

            if (!roomGroup.UsesOutdoorTemperature)
            {
                GenDraw.DrawFieldEdges(roomGroup.Cells.ToList(), typeColor);
            }
        }

        /// <summary>
        /// Place Worker for Air Vents.
        /// 
        /// Checks:
        /// - North Cell from Center musn't be Impassable
        /// </summary>
        /// <param name="def">The Def Being Built</param>
        /// <param name="center">Target Location</param>
        /// <param name="rot">Rotation of the Object to be Placed</param>
        /// <param name="thingToIgnore">Unused field</param>
        /// <returns>Boolean/Acceptance Report if we can place the object of not.</returns>
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            var size = def.Size;

            var list = GenAdj.CellsAdjacent8Way(center, rot, size);

            if (list.Any(intVec => intVec.Impassable(map)))
            {
                return "CentralizedClimateControl.Consumer.SurroundAirVentPlaceError".Translate();
            }

            return true;
        }
    }
}

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
            AirFlowType type = AirFlowType.Hot;

            var list = center.GetThingList(Map);
            foreach (var thing in list)
            {
                if (thing is Building_AirVent)
                {
                    var airVent = thing as Building_AirVent;

                    if (airVent.CompAirFlowConsumer.AirFlowNet != null)
                    {
                        type = airVent.CompAirFlowConsumer.AirFlowNet.FlowType;
                    }

                    break;
                }
            }

            IntVec3 intVec = center + IntVec3.North.RotatedBy(rot);

            Color typeColor = type == AirFlowType.Hot ? Color.red : Color.blue;

            GenDraw.DrawFieldEdges(new List<IntVec3>
            {
                intVec
            }, typeColor);

            RoomGroup roomGroup = intVec.GetRoomGroup(base.Map);
            if (roomGroup != null)
            {
                if (!roomGroup.UsesOutdoorTemperature)
                {
                    GenDraw.DrawFieldEdges(roomGroup.Cells.ToList<IntVec3>(), typeColor);
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

            IntVec3 vec = center + IntVec3.North.RotatedBy(rot);

            if (vec.Impassable(base.Map))
            {
                return "EnhancedTemperature.Consumer.AirVentPlaceError".Translate();
            }

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace CentralizedClimateControl
{
    public class GraphicPipe : Graphic_Linked
    {
        public AirFlowType FlowType;

        public GraphicPipe()
        {
        }

        public GraphicPipe(Graphic graphic, AirFlowType flowType)
        {
            subGraphic = graphic;
            this.FlowType = flowType;
        }

        public GraphicPipe(Graphic graphic)
        {
            subGraphic = graphic;
            FlowType = AirFlowType.Hot;
        }

        public override bool ShouldLinkWith(IntVec3 vec, Thing parent)
        {
            return vec.InBounds(parent.Map) && CentralizedClimateControlUtility.GetNetManager(parent.Map).ZoneAt(vec, FlowType);
        }

        private static bool CheckPipe(Thing obj)
        {
            return obj.GetType() == typeof(Building_AirPipe);
        }

        public override void Print(SectionLayer layer, Thing parent)
        {
            Material material = this.LinkedDrawMatFrom(parent, parent.Position);
            Printer_Plane.PrintPlane(layer, parent.TrueCenter(), Vector2.one, material, 0f, false, null, null, 0.01f);
            for (int i = 0; i < 4; i++)
            {
                IntVec3 intVec = parent.Position + GenAdj.CardinalDirections[i];
            
                if (intVec.InBounds(parent.Map) &&
                    CentralizedClimateControlUtility.GetNetManager(parent.Map).ZoneAt(intVec, this.FlowType) &&
                    !intVec.GetTerrain(parent.Map).layerable)
                {
                    List<Thing> thingList = intVec.GetThingList(parent.Map);
            
                    Predicate<Thing> predicate = CheckPipe;
                    if (!thingList.Any<Thing>(predicate))
                    {
                        Material material2 = this.LinkedDrawMatFrom(parent, intVec);
                        Printer_Plane.PrintPlane(layer, intVec.ToVector3ShiftedWithAltitude(parent.def.Altitude), Vector2.one, material2, 0f, false, null, null, 0.01f);
                    }
                }
            }
        }
    }
}

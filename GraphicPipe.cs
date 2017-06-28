using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedTemperature
{
    public class GraphicPipe : Graphic_Linked
    {
        public AirFlowType FlowType;

        protected Graphic SubGraphic;

        public override Material MatSingle
        {
            get
            {
                return this.SubGraphic.MatSingle;
            }
        }

        public GraphicPipe()
        {
        }

        public GraphicPipe(Graphic subGraphic, AirFlowType flowType)
        {
            this.SubGraphic = subGraphic;
            this.FlowType = flowType;
        }

        public GraphicPipe(Graphic subGraphic)
        {
            this.SubGraphic = subGraphic;
            FlowType = AirFlowType.Hot;
        }

        public override bool ShouldLinkWith(IntVec3 vec, Thing parent)
        {
            return vec.InBounds(parent.Map) && EnhancedTemperatureUtility.GetNetManager(parent.Map).ZoneAt(vec, FlowType);
        }

        protected Material LinkedPipeDrawMatFrom(Thing parent, IntVec3 cell)
        {
            int num = 0;
            int num2 = 1;
            for (int i = 0; i < 4; i++)
            {
                IntVec3 c = cell + GenAdj.CardinalDirections[i];
                if (this.ShouldLinkWith(c, parent))
                {
                    num += num2;
                }
                num2 *= 2;
            }

            var linkDirections = (LinkDirections)num;
            return MaterialAtlasPool.SubMaterialFromAtlas(this.SubGraphic.MatSingleFor(parent), linkDirections);
        }

        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            return new GraphicPipe(this.SubGraphic.GetColoredVersion(newShader, newColor, newColorTwo), this.FlowType)
            {
                data = this.data
            };
        }

        private static bool CheckPipe(Thing obj)
        {
            return obj.GetType() == typeof(Building_AirPipe);
        }

        public override void Print(SectionLayer layer, Thing parent)
        {
            Material material = this.LinkedPipeDrawMatFrom(parent, parent.Position);
            Printer_Plane.PrintPlane(layer, parent.TrueCenter(), Vector2.one, material, 0f, false, null, null, 0.01f);
            for (int i = 0; i < 4; i++)
            {
                IntVec3 intVec = parent.Position + GenAdj.CardinalDirections[i];

                if (intVec.InBounds(parent.Map) &&
                    EnhancedTemperatureUtility.GetNetManager(parent.Map).ZoneAt(intVec, this.FlowType) &&
                    !intVec.GetTerrain(parent.Map).layerable)
                {
                    List<Thing> thingList = intVec.GetThingList(parent.Map);

                    Predicate<Thing> predicate = CheckPipe;
                    if (!thingList.Any<Thing>(predicate))
                    {
                        Material material2 = this.LinkedPipeDrawMatFrom(parent, intVec);
                        Printer_Plane.PrintPlane(layer, intVec.ToVector3ShiftedWithAltitude(parent.def.Altitude), Vector2.one, material2, 0f, false, null, null, 0.01f);}
                }
            }
        }
    }
}

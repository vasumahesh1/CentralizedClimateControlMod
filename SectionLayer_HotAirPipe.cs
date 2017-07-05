using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RimWorld;
using Verse;

namespace EnhancedTemperature
{
    class SectionLayer_HotAirPipe : SectionLayer_Things
    {
        public AirFlowType FlowType;

        public SectionLayer_HotAirPipe(Section section) : base(section)
        {
            FlowType = AirFlowType.Hot;
            requireAddToMapMesh = false;
            relevantChangeTypes = (MapMeshFlag) 4;
        }

        public override void DrawLayer()
        {
            Designator_Build designatorBuild = Find.DesignatorManager.SelectedDesignator as Designator_Build;
            if (designatorBuild == null)
            {
                return;
            }

            ThingDef thingDef = designatorBuild.PlacingDef as ThingDef;
            if (thingDef == null)
            {
                return;
            }

            if (thingDef.comps.OfType<CompProperties_AirFlow>().FirstOrDefault((x) => x.flowType == this.FlowType) != null)
            {
                base.DrawLayer();
            }
        }

        protected override void TakePrintFrom(Thing thing)
        {
            Building building = thing as Building;
            if (building != null)
            {
                CompAirFlow compAirFlow = building.GetComps<CompAirFlow>().FirstOrDefault((x) => x.FlowType == this.FlowType || x.FlowType == AirFlowType.Any);
                if (compAirFlow == null)
                {
                    return;
                }

                compAirFlow.PrintForGrid(this);
            }
        }
    }
}

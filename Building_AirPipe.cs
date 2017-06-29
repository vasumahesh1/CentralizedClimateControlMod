using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedTemperature
{
    public class Building_AirPipe : Building
    {
        public AirFlowType FlowType;

        public CompAirFlowPipe CompAirFlowPipe;

//        public override Graphic Graphic
//        {
//            get
//            {
//                if (base.Position.GetTerrain(base.Map).layerable)
//                {
//                    return GraphicsLoader.GraphicHotPipeClear;
//                }
//
//                return GraphicsLoader.GraphicHotPipe;
//            }
//        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            CompAirFlowPipe = base.GetComp<CompAirFlowPipe>();
        }

    }
}

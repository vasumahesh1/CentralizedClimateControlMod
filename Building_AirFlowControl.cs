using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace CentralizedClimateControl
{
    public class Building_AirFlowControl : Building
    {
        public CompPowerTrader CompPowerTrader;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.CompPowerTrader = base.GetComp<CompPowerTrader>();
        }
    }
}

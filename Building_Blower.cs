using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace EnhancedTemperature
{
    public class Building_Blower : Building_AirFlowControl
    {
        private int windCellsBlocked = 0;
        private const float EfficiencyLossPerWindCubeBlocked = 0.0076923077f;

        public CompAirFlowProducer CompAirProducer;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.CompAirProducer = base.GetComp<CompAirFlowProducer>();
            this.CompAirProducer.Props.flowType = AirFlowType.Any;
        }

        public override void TickRare()
        {
            if (!this.CompPowerTrader.PowerOn)
            {
                this.CompAirProducer.CurrentAirFlow = 0;
                return;
            }

            IntVec3 intVec = base.Position + IntVec3.South.RotatedBy(base.Rotation);
            CompAirProducer.IntakeTemperature = intVec.GetTemperature(base.Map);

            float flow = this.CompAirProducer.Props.baseAirFlow - windCellsBlocked * EfficiencyLossPerWindCubeBlocked;
            CompAirProducer.CurrentAirFlow = flow;
        }
    }
}

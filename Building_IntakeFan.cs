using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace CentralizedClimateControl
{
    public class Building_IntakeFan : Building_AirFlowControl
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


            float sumTemp = 0f;
            for (int i = 0; i < 8; i++)
            {
                IntVec3 vec = Position + GenAdj.AdjacentCellsAround[i];

                if (vec.Impassable(base.Map))
                {
                    this.CompAirProducer.CurrentAirFlow = 0;
                    return;
                }

                sumTemp += vec.GetTemperature(Map);
            }

            float intake = (float) sumTemp / 8;
            CompAirProducer.IntakeTemperature = intake;

            float flow = this.CompAirProducer.Props.baseAirFlow - windCellsBlocked * EfficiencyLossPerWindCubeBlocked;
            CompAirProducer.CurrentAirFlow = flow;
        }
    }
}

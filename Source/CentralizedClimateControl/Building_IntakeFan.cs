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

            var size = def.Size;
            var list = GenAdj.CellsAdjacent8Way(Position, Rotation, size).ToList();

            foreach (var intVec in list)
            {
                if (intVec.Impassable(base.Map))
                {
                    this.CompAirProducer.CurrentAirFlow = 0;
                    CompAirProducer.IsBlocked = true;
                    return;
                }

                sumTemp += intVec.GetTemperature(Map);
            }

            CompAirProducer.IsBlocked = false;

            float intake = (float) sumTemp / list.Count;
            CompAirProducer.IntakeTemperature = intake;

            float flow = this.CompAirProducer.Props.baseAirFlow - windCellsBlocked * EfficiencyLossPerWindCubeBlocked;
            CompAirProducer.CurrentAirFlow = flow;
        }
    }
}

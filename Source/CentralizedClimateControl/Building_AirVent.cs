using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace CentralizedClimateControl
{
    public class Building_AirVent : Building
    {
        public CompAirFlowConsumer CompAirFlowConsumer;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            CompAirFlowConsumer = base.GetComp<CompAirFlowConsumer>();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }

            if (CompAirFlowConsumer != null)
            {
                yield return CentralizedClimateControlUtility.GetPipeSwitchToggle(CompAirFlowConsumer);
            }
        }

        public override void TickRare()
        {
            CompAirFlowConsumer.TickRare();

            if (!CompAirFlowConsumer.IsOperating())
            {
                return;
            }

            if (!CompAirFlowConsumer.IsActive())
            {
                return;
            }

            var outsideTemp = CompAirFlowConsumer.ConvertedTemperature;

            IntVec3 intVec = base.Position + IntVec3.North.RotatedBy(base.Rotation);

            if (intVec.Impassable(base.Map))
            {
                return;
            }

            var insideTemp = intVec.GetTemperature(base.Map);
            var tempDiff = outsideTemp - insideTemp;
            var magnitudeChange = Mathf.Abs(tempDiff);

            // Cap change at 10.0f
            if (magnitudeChange > 10.0f)
            {
                magnitudeChange = 10.0f;
            }

            float signChanger = 1;

            if (tempDiff < 0)
            {
                signChanger = -1;
            }

            // Flow Efficiency is capped at 1.0f. Squaring will only keep it less than or equal to 1.0f. Smaller the number more drastic the square.
            var efficiencyImpact = CompAirFlowConsumer.FlowEfficiency * CompAirFlowConsumer.FlowEfficiency;

            var smoothMagnitude =  magnitudeChange * 0.25f * (CompAirFlowConsumer.Props.baseAirExhaust / 100.0f);
            var energyLimit = smoothMagnitude * efficiencyImpact * 4.16666651f * 12f * signChanger;
            var tempChange = GenTemperature.ControlTemperatureTempChange(intVec, base.Map, energyLimit, outsideTemp);
            
            bool flag = !Mathf.Approximately(tempChange, 0f);
            if (flag)
            {
                intVec.GetRoomGroup(base.Map).Temperature += tempChange;
            }
        }
    }
}

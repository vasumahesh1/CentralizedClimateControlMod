using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override void TickRare()
        {
            CompAirFlowConsumer.TickRare();

            //            var outsideTemp = CompAirFlowConsumer.ConvertedTemperature;
            //
            //            IntVec3 intVec = base.Position + IntVec3.North.RotatedBy(base.Rotation);
            //
            //            if (intVec.Impassable(base.Map))
            //            {
            //                return;
            //            }
            //
            //            float insideTemp = intVec.GetTemperature(base.Map);
            //
            //            float tempDiff = Mathf.Abs(outsideTemp - insideTemp);
            ////            if (outsideTemp - 40f > tempDiff)
            ////            {
            ////                tempDiff = outsideTemp - 40f;
            ////            }
            //
            //            float num2 = 1f - tempDiff * 0.0076923077f;
            ////            if (num2 < 0f)
            ////            {
            ////                num2 = 0f;
            ////            }
            //
            //            float energyLimit = 12f * num2 * 4.16666651f * CompAirFlowConsumer.FlowEfficiency;
            //            float tempChange = GenTemperature.ControlTemperatureTempChange(intVec, base.Map, energyLimit, outsideTemp);
            //
            //            bool flag = !Mathf.Approximately(tempChange, 0f);
            //            if (flag)
            //            {
            //                intVec.GetRoomGroup(base.Map).Temperature += tempChange;
            //            }


            // IMPL 2
            var outsideTemp = CompAirFlowConsumer.ConvertedTemperature;

            IntVec3 intVec = base.Position + IntVec3.North.RotatedBy(base.Rotation);

            if (intVec.Impassable(base.Map))
            {
                return;
            }

            float insideTemp = intVec.GetTemperature(base.Map);
            float tempDiff = outsideTemp - insideTemp;
            float magnitudeChange = Mathf.Abs(tempDiff);

            float signChanger = 1;

            if (tempDiff < 0)
            {
                signChanger = -1;
            }

            float smoothMagnitude =  magnitudeChange * 0.5f;
            float energyLimit = smoothMagnitude * CompAirFlowConsumer.FlowEfficiency * 4.16666651f * 12f * signChanger;
            float tempChange = GenTemperature.ControlTemperatureTempChange(intVec, base.Map, energyLimit, outsideTemp);
            
            bool flag = !Mathf.Approximately(tempChange, 0f);
            if (flag)
            {
                intVec.GetRoomGroup(base.Map).Temperature += tempChange;
            }
        }
    }
}

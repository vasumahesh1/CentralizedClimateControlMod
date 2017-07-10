using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedTemperature
{
    public static class EnhancedTemperatureUtility
    {

        public static AirFlowNetManager GetNetManager(Map map)
        {
            return map.GetComponent<AirFlowNetManager>();
        }

//        public static float GetTempChange(IntVec3 cell, Map map, float energyLimit, float targetTemperature)
//        {
//            RoomGroup roomGroup = cell.GetRoomGroup(map);
//            if (roomGroup == null || roomGroup.UsesOutdoorTemperature)
//            {
//                return 0f;
//            }
//
//            float energyDivided = energyLimit / (float)roomGroup.CellCount;
//
//            float roomTemp = roomGroup.Temperature;
//            float tempChange = Mathf.Abs(roomTemp - targetTemperature);
//
//            bool flag = Mathf.Approximately(tempChange, 0f);
//            if (flag)
//            {
//                return 0;
//            }
//
//            if (roomTemp > targetTemperature)
//            {
//                // Need to Cool Down
//                return -1 * energyDivided;
//            }
//
//            return energyDivided;
//        }
    }
}

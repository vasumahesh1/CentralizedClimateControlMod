using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedTemperature
{
    public static class EnhancedTemperatureUtility
    {

        public static AirFlowNetManager GetNetManager(Map map)
        {
            return map.GetComponent<AirFlowNetManager>();
        }
    }
}

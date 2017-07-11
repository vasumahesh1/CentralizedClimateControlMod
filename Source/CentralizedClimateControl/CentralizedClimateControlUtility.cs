using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace CentralizedClimateControl
{
    public static class CentralizedClimateControlUtility
    {
        public static AirFlowNetManager GetNetManager(Map map)
        {
            return map.GetComponent<AirFlowNetManager>();
        }
    }
}

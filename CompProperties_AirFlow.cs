using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedTemperature
{
    public enum AirFlowType
    {
        Hot = 0,
        Cold = 1,
        Any = 2
    }

    public class CompProperties_AirFlow : CompProperties
    {
        public bool transmitsAir;

        public float baseAirFlow;

        public AirFlowType flowType;

        public float baseAirExhaust;

        public float thermalCapacity;
    }
}

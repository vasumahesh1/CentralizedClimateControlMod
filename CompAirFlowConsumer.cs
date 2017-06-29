using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace EnhancedTemperature
{
    public class CompAirFlowConsumer : CompAirFlow
    {
        public const string AirFlowOutputKey = "EnhancedTemperature.AirFlowOutput";
        public const string IntakeTempKey = "EnhancedTemperature.Consumer.ConvertedTemperature";
        public const string FlowEfficiencyKey = "EnhancedTemperature.Consumer.FlowEfficiencyKey";
        public const string ThermalEfficiencyKey = "EnhancedTemperature.Consumer.ThermalEfficiencyKey";

        public float ConvertedTemperature = 0.0f;
        protected CompFlickable FlickableComp;

        public float ExhaustAirFlow
        {
            get
            {
                return this.Props.baseAirExhaust;
            }
        }

        public float FlowEfficiency
        {
            get
            {
                return AirFlowNet.FlowEfficiency;
            }
        }

        public string DebugString
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(this.parent.LabelCap + " CompAirFlowConsumer:");
                stringBuilder.AppendLine("   ConvertedTemperature: " + ConvertedTemperature);
                return stringBuilder.ToString();
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            EnhancedTemperatureUtility.GetNetManager(this.parent.Map).RegisterConsumer(this);
            this.FlickableComp = this.parent.GetComp<CompFlickable>();

            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDeSpawn(Map map)
        {
            EnhancedTemperatureUtility.GetNetManager(map).DeregisterConsumer(this);
            ResetFlowVariables();
            base.PostDeSpawn(map);
        }

        public override string CompInspectStringExtra()
        {
            var convertedTemp = ConvertedTemperature.ToStringTemperature("F0");
            string str = IntakeTempKey.Translate(new object[] { convertedTemp });

            str += "\n";
            str += FlowEfficiencyKey.Translate(new object[] { AirFlowNet.FlowEfficiency });

            str += "\n";
            str += ThermalEfficiencyKey.Translate(new object[] { AirFlowNet.ThermalEfficiency });

            return str + "\n" + base.CompInspectStringExtra();
        }

        public void TickRare()
        {
            ConvertedTemperature = AirFlowNet.AverageConvertedTemperature;
        }

        public override void ResetFlowVariables()
        {
            ConvertedTemperature = 0.0f;
            base.ResetFlowVariables();
        }
    }
}

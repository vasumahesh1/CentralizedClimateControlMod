using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace EnhancedTemperature
{
    public class CompAirFlowTempControl: CompAirFlow
    {
        public const string TemperatureArrowKey = "EnhancedTemperature.Producer.TemperatureArrow";
        public const string TargetTemperatureKey = "EnhancedTemperature.Producer.TargetTemperature";

        [Unsaved]
        public bool IsOperatingAtHighPower;
        public bool IsHeating;
        public bool IsStable;

        public float IntakeTemperature = 0.0f;
        public float TargetTemperature = 21.0f;
        public float ConvertedTemperature = 0.0f;
        public float DeltaTemperature = 0.0f;

        private const float DeltaSmooth = 4.0f;

        protected CompFlickable FlickableComp;

        public float ThermalCapacity
        {
            get
            {
                return this.Props.thermalCapacity;
            }
        }

        public string DebugString
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(this.parent.LabelCap + " CompAirFlow:");
                stringBuilder.AppendLine("   AirFlow IsOperating: " + IsOperating());
                return stringBuilder.ToString();
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            EnhancedTemperatureUtility.GetNetManager(this.parent.Map).RegisterTempControl(this);
            this.FlickableComp = this.parent.GetComp<CompFlickable>();

            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDeSpawn(Map map)
        {
            EnhancedTemperatureUtility.GetNetManager(map).DeregisterTempControl(this);
            ResetFlowVariables();
            base.PostDeSpawn(map);
        }

        public override string CompInspectStringExtra()
        {
            string str = "";

            if (IsOperating())
            {
                var intake = IntakeTemperature.ToStringTemperature("F0");
                var converted = ConvertedTemperature.ToStringTemperature("F0");
                str += TemperatureArrowKey.Translate(new object[] { intake, converted }) + "\n";
                str += DeltaTemperature.ToStringTemperature("F0");
            }
            else
            {
                str = "PowerNeeded".Translate();
            }

            return str;
        }

        public override void ResetFlowVariables()
        {
            DeltaTemperature = 0.0f;
            TargetTemperature = 21.0f;
            ConvertedTemperature = 0.0f;
            IntakeTemperature = 0.0f;
            IsOperatingAtHighPower = false;
            base.ResetFlowVariables();
        }

        public void TickRare(CompTempControl compTempControl)
        {
            IntakeTemperature = AirFlowNet.AverageIntakeTemperature;
            TargetTemperature = compTempControl.targetTemperature;
            ConvertedTemperature = IntakeTemperature + DeltaTemperature;

            GenerateDelta(compTempControl);
        }

        private void GenerateDelta(CompTempControl compTempControl)
        {
            var targetDelta = TargetTemperature - IntakeTemperature;
            var currentDelta = ConvertedTemperature - IntakeTemperature;

            if (Mathf.Abs(targetDelta - currentDelta) < 1.0f)
            {
                DeltaTemperature += (targetDelta - currentDelta);
                IsStable = true;
                return;
            }

            IsStable = false;
            var deltaDelta = targetDelta - currentDelta;

            IsHeating = targetDelta > currentDelta;
            DeltaTemperature += (float)(compTempControl.Props.energyPerSecond * AirFlowNet.ThermalEfficiency) / (DeltaSmooth * deltaDelta);
        }
    }
}

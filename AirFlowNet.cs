using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace EnhancedTemperature
{
    public class AirFlowNet
    {
        private int _intGridId = -2;
        private float _currentAirflow = 0.0f;

        public List<CompAirFlow> Connectors = new List<CompAirFlow>();
        public List<CompAirFlowProducer> Producers = new List<CompAirFlowProducer>();
        public List<CompAirFlowTempControl> TempControls = new List<CompAirFlowTempControl>();

        public float AverageIntakeTemperature;

        public int GridID
        {
            get { return this._intGridId; }
            set { this._intGridId = value; }
        }

        public float CurrentAirFlow
        {
            get { return _currentAirflow; }
        }

        private void TickProducers()
        {
            float airFlow = 0.0f;

            float tempSum = 0.0f;

            foreach (var producer in Producers)
            {
                if (!producer.IsOperating())
                {
                    continue;
                }

                airFlow += producer.CurrentAirFlow;
                tempSum += producer.IntakeTemperature;
            }

            AverageIntakeTemperature = (float) tempSum / Producers.Count;

            _currentAirflow = airFlow;
        }

        public void RegisterProducer(CompAirFlowProducer producer)
        {
            if (this.Producers.Contains(producer))
            {
                Log.Error("AirFlowNet registered producer it already had: " + producer);
                return;
            }
            this.Producers.Add(producer);
        }

        public void DeregisterProducer(CompAirFlowProducer producer)
        {
            this.Producers.Remove(producer);
        }

        public void AirFlowNetTick()
        {
            TickProducers();
        }

        public string DebugString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("------------");
            stringBuilder.AppendLine("AIRFLOW NET:");
            stringBuilder.AppendLine("  Prodcued AirFlow: " + this.CurrentAirFlow);

            stringBuilder.AppendLine("  Producers: ");
            foreach (var current in this.Producers)
            {
                stringBuilder.AppendLine("      " + current.parent);
            }

            stringBuilder.AppendLine("------------");
            return stringBuilder.ToString();
        }
    }
}

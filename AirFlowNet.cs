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
        private float _currentIntakeAir = 0.0f;
        private float _currentExhaustAir = 0.0f;

        public List<CompAirFlow> Connectors = new List<CompAirFlow>();
        public List<CompAirFlowProducer> Producers = new List<CompAirFlowProducer>();
        public List<CompAirFlowTempControl> TempControls = new List<CompAirFlowTempControl>();
        public List<CompAirFlowConsumer> Consumers = new List<CompAirFlowConsumer>();

        public AirFlowType FlowType;

        public float ThermalCapacity = 0.0f;
        public float ThermalEfficiency = 1.0f;
        public float FlowEfficiency = 1.0f;

        public float AverageIntakeTemperature;
        public float AverageConvertedTemperature;

        public int GridID
        {
            get { return this._intGridId; }
            set { this._intGridId = value; }
        }

        public float CurrentIntakeAir
        {
            get { return _currentIntakeAir; }
        }

        public float CurrentExhaustAir
        {
            get { return _currentExhaustAir; }
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
            _currentIntakeAir = airFlow;
        }

        private void TickConsumers()
        {
            float airFlow = 0.0f;
            int rooms = 0;

            foreach (var consumer in Consumers)
            {
                airFlow += consumer.ExhaustAirFlow;
            }

            _currentExhaustAir = airFlow;
        }

        private void TickTempControllers()
        {
            float tempSum = 0.0f;
            float thermalCapacity = 0.0f;

            foreach (var compAirFlowTempControl in TempControls)
            {
                tempSum += compAirFlowTempControl.ConvertedTemperature;
                thermalCapacity += compAirFlowTempControl.ThermalCapacity;
            }

            ThermalCapacity = thermalCapacity;
            AverageConvertedTemperature = (float) tempSum / TempControls.Count;
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
            TickTempControllers();
            TickConsumers();

            if (CurrentIntakeAir > 0)
            {
                ThermalEfficiency = ThermalCapacity / CurrentIntakeAir;

                if (ThermalEfficiency > 1.0f)
                {
                    ThermalEfficiency = 1.0f;
                }
            }
            else
            {
                ThermalEfficiency = 0.0f;
            }

            if (CurrentExhaustAir > 0)
            {
                FlowEfficiency = (float)CurrentIntakeAir / CurrentExhaustAir;

                if (FlowEfficiency > 1.0f)
                {
                    FlowEfficiency = 1.0f;
                }
            }
            else
            {
                FlowEfficiency = 0.0f;
            }
        }

        public string DebugString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("------------");
            stringBuilder.AppendLine("AIRFLOW NET:");
            stringBuilder.AppendLine("  Prodcued AirFlow: " + this.CurrentIntakeAir);
            stringBuilder.AppendLine("  AverageIntakeTemperature: " + this.AverageIntakeTemperature);
            stringBuilder.AppendLine("  AverageConvertedTemperature: " + this.AverageConvertedTemperature);

            stringBuilder.AppendLine("  Producers: ");
            foreach (var current in this.Producers)
            {
                stringBuilder.AppendLine("      " + current.parent);
            }

            stringBuilder.AppendLine("  TempControls: ");
            foreach (var current in this.TempControls)
            {
                stringBuilder.AppendLine("      " + current.parent);
            }

            stringBuilder.AppendLine("  Consumers: ");
            foreach (var current in this.Consumers)
            {
                stringBuilder.AppendLine("      " + current.parent);
            }

            stringBuilder.AppendLine("------------");
            return stringBuilder.ToString();
        }
    }
}

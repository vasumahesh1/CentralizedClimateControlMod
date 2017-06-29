using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedTemperature
{
    public class CompAirFlow : ThingComp
    {
        private int _intGridId = -2;
        private AirFlowNet _airFlowNet;

        public const string NotConnectedKey = "EnhancedTemperature.AirFlowNetDisconnected";
        public const string ConnectedKey = "EnhancedTemperature.AirFlowNetConnected";
        public const string AirTypeKey = "EnhancedTemperature.AirType";
        public const string HotAirKey = "EnhancedTemperature.HotAir";
        public const string ColdAirKey = "EnhancedTemperature.ColdAir";
        public const string TotalNetworkAirKey = "EnhancedTemperature.TotalNetworkAir";

        public AirFlowType FlowType
        {
            get
            {
                return this.Props.flowType;
            }
        }

        public int GridID
        {
            get { return this._intGridId; }
            set { this._intGridId = value; }
        }

        public AirFlowNet AirFlowNet
        {
            get { return _airFlowNet; }
            set { _airFlowNet = value; }
        }

        public CompProperties_AirFlow Props
        {
            get
            {
                return (CompProperties_AirFlow)this.props;
            }
        }

        public virtual void ResetFlowVariables()
        {
            this.AirFlowNet = null;
            this.GridID = -1;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            EnhancedTemperatureUtility.GetNetManager(this.parent.Map).RegisterPipe(this);
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDeSpawn(Map map)
        {
            EnhancedTemperatureUtility.GetNetManager(map).DeregisterPipe(this);
            ResetFlowVariables();

            base.PostDeSpawn(map);
        }

        public bool IsOperating()
        {
            bool isConnected = this.AirFlowNet != null;
            return isConnected;
        }

        public override string CompInspectStringExtra()
        {
            if (!IsOperating())
            {
                return NotConnectedKey.Translate();
            }

            string res = ConnectedKey.Translate();

            if (FlowType != AirFlowType.Any)
            {
                res += "\n";
                res += GetAirTypeString(FlowType);
            }

            res += "\n";
            res += TotalNetworkAirKey.Translate(new object[] { AirFlowNet.CurrentIntakeAir });

            return res;
        }

        protected string GetAirTypeString(AirFlowType type)
        {
            string res = "";
            switch (type)
            {
                case AirFlowType.Cold:
                    res += AirTypeKey.Translate(new object[] { ColdAirKey.Translate() });
                    break;

                case AirFlowType.Hot:
                    res += AirTypeKey.Translate(new object[] { HotAirKey.Translate() });
                    break;

                default:
                    res += AirTypeKey.Translate(new object[] { "Unknown" });
                    break;
            }

            return res;
        }
    }
}

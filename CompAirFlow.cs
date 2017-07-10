using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CentralizedClimateControl
{
    public class CompAirFlow : ThingComp
    {
        private int _intGridId = -2;
        private AirFlowNet _airFlowNet;

        public const string NotConnectedKey = "CentralizedClimateControl.AirFlowNetDisconnected";
        public const string ConnectedKey = "CentralizedClimateControl.AirFlowNetConnected";
        public const string AirTypeKey = "CentralizedClimateControl.AirType";
        public const string HotAirKey = "CentralizedClimateControl.HotAir";
        public const string ColdAirKey = "CentralizedClimateControl.ColdAir";
        public const string FrozenAirKey = "CentralizedClimateControl.FrozenAir";
        public const string TotalNetworkAirKey = "CentralizedClimateControl.TotalNetworkAir";

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
            CentralizedClimateControlUtility.GetNetManager(this.parent.Map).RegisterPipe(this);
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDeSpawn(Map map)
        {
            CentralizedClimateControlUtility.GetNetManager(map).DeregisterPipe(this);
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

        public void PrintForGrid(SectionLayer layer, AirFlowType type)
        {
            switch (type)
            {
                case AirFlowType.Hot:
                    GraphicsLoader.GraphicHotPipeOverlay.Print(layer, this.parent);
                    break;

                case AirFlowType.Cold:
                    GraphicsLoader.GraphicColdPipeOverlay.Print(layer, this.parent);
                    break;

                case AirFlowType.Frozen:
                    GraphicsLoader.GraphicFrozenPipeOverlay.Print(layer, this.parent);
                    break;

                case AirFlowType.Any:
                    break;
            }
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

                case AirFlowType.Frozen:
                    res += AirTypeKey.Translate(new object[] { FrozenAirKey.Translate() });
                    break;

                default:
                    res += AirTypeKey.Translate(new object[] { "Unknown" });
                    break;
            }

            return res;
        }
    }
}

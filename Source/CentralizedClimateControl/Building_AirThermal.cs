using RimWorld;
using UnityEngine;
using Verse;

namespace CentralizedClimateControl
{
    public class Building_AirThermal : Building_AirFlowControl
    {
        public CompTempControl CompTempControl;
        public CompAirFlowTempControl CompAirFlowTempControl;

        /// <summary>
        /// Building spawned on the map
        /// </summary>
        /// <param name="map">RimWorld Map</param>
        /// <param name="respawningAfterLoad">Unused flag</param>
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            CompTempControl = GetComp<CompTempControl>();
            CompAirFlowTempControl = GetComp<CompAirFlowTempControl>();
            base.SpawnSetup(map, respawningAfterLoad);
        }

        /// <summary>
        /// Tick Function for Climate Buildings - Here we calculate the Temperature growth from Intake to Target Temperature
        /// Plus, we exhaust a certain amount of Heat to the South of the Building.
        /// </summary>
        public override void TickRare()
        {
            if (!CompPowerTrader.PowerOn)
            {
                return;
            }

            if (!CompAirFlowTempControl.IsOperating())
            {
                return;
            }

            var size = def.Size;

            IntVec3 iterator = new IntVec3(Position.x, Position.y, Position.z);

            for (int dx = 0; dx < size.x; dx++)
            {
                IntVec3 currentPos = iterator + IntVec3.South.RotatedBy(Rotation);

                if (currentPos.Impassable(Map))
                {
                    CompAirFlowTempControl.IsBlocked = true;
                    return;
                }

                iterator += IntVec3.East.RotatedBy(Rotation);
            }

            CompAirFlowTempControl.IsBlocked = false;

            if (!CompAirFlowTempControl.IsActive())
            {
                return;
            }

            CompAirFlowTempControl.TickRare(CompTempControl);

            var tempDiff = CompAirFlowTempControl.TargetTemperature - CompAirFlowTempControl.ConvertedTemperature;
            IntVec3 intVec = Position + IntVec3.South.RotatedBy(Rotation);

            if (CompAirFlowTempControl.IsHeating)
            {
                return;
            }

            // Push Heat when Cooling Only
            var magnitudeChange = Mathf.Abs(tempDiff);
            var baseHeat = 25.0f;

            // Cap change at 20.0f
            if (magnitudeChange > 20.0f)
            {
                magnitudeChange = 20.0f;
            }

            baseHeat += magnitudeChange;

            GenTemperature.PushHeat(intVec, Map, baseHeat * 1.25f);
        }

    }
}

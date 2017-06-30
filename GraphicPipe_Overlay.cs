using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedTemperature
{
    public class GraphicPipe_Overlay: Graphic_Linked
    {
        public AirFlowType FlowType;

        public int ID;

        public GraphicPipe_Overlay()
        {
        }

        public GraphicPipe_Overlay(Graphic subGraphic) : base(subGraphic)
        {
            FlowType = AirFlowType.Hot;
        }

        public GraphicPipe_Overlay(Graphic subGraphic, AirFlowType type) : base(subGraphic)
        {
            this.FlowType = type;
        }

        public override bool ShouldLinkWith(IntVec3 intVec, Thing parent)
        {
            var building = parent as Building;
            if (building == null)
            {
                return false;
            }

//            CompAirFlow compAirFlow = building.GetComps<CompAirFlow>().FirstOrDefault((k) => (k.FlowType == FlowType) || k.FlowType == AirFlowType.Any);
//            if (compAirFlow == null)
//            {
//                return false;
//            }
//
//            if (compAirFlow.GridID == -2)
//            {
//                return false;
//            }

            return intVec.InBounds(parent.Map) && EnhancedTemperatureUtility.GetNetManager(parent.Map).ZoneAt(intVec, FlowType);
        }

        public override void Print(SectionLayer layer, Thing parent)
        {
            CellRect.CellRectIterator iterator = parent.OccupiedRect().GetIterator();
            while (!iterator.Done())
            {
                IntVec3 current = iterator.Current;
                Vector3 vector = current.ToVector3ShiftedWithAltitude(29);
                Printer_Plane.PrintPlane(layer, vector, Vector2.one, base.LinkedDrawMatFrom(parent, current), 0f, false, null, null, 0.01f);
                iterator.MoveNext();
            }
        }
    }
}

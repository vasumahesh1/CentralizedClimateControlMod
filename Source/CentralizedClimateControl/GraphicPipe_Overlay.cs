using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace CentralizedClimateControl
{
    public class GraphicPipe_Overlay: Graphic_Linked
    {
        public AirFlowType FlowType;

        private Graphic _anyGraphic;
        private Graphic _flowGraphic;

        public int ID;

        public GraphicPipe_Overlay()
        {
        }

        public GraphicPipe_Overlay(Graphic subGraphic, Graphic anyGraphic) : base(subGraphic)
        {
            FlowType = AirFlowType.Hot;
            _anyGraphic = anyGraphic;
            _flowGraphic = subGraphic;
        }

        public GraphicPipe_Overlay(Graphic subGraphic, Graphic anyGraphic, AirFlowType type) : base(subGraphic)
        {
            this.FlowType = type;
            _anyGraphic = anyGraphic;
            _flowGraphic = subGraphic;
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

            return intVec.InBounds(parent.Map) && CentralizedClimateControlUtility.GetNetManager(parent.Map).ZoneAt(intVec, FlowType);
        }

        public override void Print(SectionLayer layer, Thing parent)
        {
            CellRect.CellRectIterator iterator = parent.OccupiedRect().GetIterator();
            while (!iterator.Done())
            {
                IntVec3 current = iterator.Current;
                Vector3 vector = current.ToVector3ShiftedWithAltitude(29);

                var building = parent as Building;
                if (building == null)
                {
                    return;
                }

                CompAirFlow compAirFlow = building.GetComps<CompAirFlow>().FirstOrDefault();
                if (compAirFlow == null)
                {
                    return;
                }

                if (compAirFlow.FlowType != FlowType && compAirFlow.FlowType != AirFlowType.Any)
                {
                    return;
                }

                subGraphic = compAirFlow.FlowType == AirFlowType.Any ? _anyGraphic : _flowGraphic;

                Printer_Plane.PrintPlane(layer, vector, Vector2.one, base.LinkedDrawMatFrom(parent, current), 0f, false, null, null, 0.01f);
                iterator.MoveNext();
            }
        }
    }
}

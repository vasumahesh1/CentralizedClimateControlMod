using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedTemperature
{
    public class AirFlowNetManager : MapComponent
    {
        public List<AirFlowNet> CachedNets = new List<AirFlowNet>();
        public List<CompAirFlowProducer> CachedProducers = new List<CompAirFlowProducer>();
        public List<CompAirFlowTempControl> CachedTempControls = new List<CompAirFlowTempControl>();
        public List<CompAirFlow> CachedPipes = new List<CompAirFlow>();

        public int[,] PipeGrid;
        public bool[] DirtyPipeFlag;

        private const int RebuildValue = -2;

        private int _masterId = 0;

        public AirFlowNetManager(Map map) : base(map)
        {
            int length = Enum.GetValues(typeof(AirFlowType)).Length;
            int num = map.AllCells.Count<IntVec3>();
            this.PipeGrid = new int[length, num];

            this.DirtyPipeFlag = new bool[length];
            for (int i = 0; i < this.DirtyPipeFlag.Length; i++)
            {
                this.DirtyPipeFlag[i] = true;
            }
        }

        public void RegisterPipe(CompAirFlow pipe)
        {
            if (!this.CachedPipes.Contains(pipe))
            {
                this.CachedPipes.Add(pipe);
                GenList.Shuffle<CompAirFlow>(this.CachedPipes);
            }

            this.DirtyPipeGrid(pipe.FlowType);
        }

        public void DeregisterPipe(CompAirFlow pipe)
        {
            if (this.CachedPipes.Contains(pipe))
            {
                this.CachedPipes.Remove(pipe);
                GenList.Shuffle<CompAirFlow>(this.CachedPipes);
            }

            this.DirtyPipeGrid(pipe.FlowType);
        }

        public void RegisterTempControl(CompAirFlowTempControl device)
        {
            if (!CachedTempControls.Contains(device))
            {
                CachedTempControls.Add(device);
                GenList.Shuffle<CompAirFlowTempControl>(CachedTempControls);
            }

            this.DirtyPipeGrid(device.FlowType);
        }

        public void DeregisterTempControl(CompAirFlowTempControl device)
        {
            if (this.CachedTempControls.Contains(device))
            {
                this.CachedTempControls.Remove(device);
                GenList.Shuffle<CompAirFlowTempControl>(CachedTempControls);
            }

            this.DirtyPipeGrid(device.FlowType);
        }

        public void RegisterProducer(CompAirFlowProducer pipe)
        {
            if (!this.CachedProducers.Contains(pipe))
            {
                this.CachedProducers.Add(pipe);
                GenList.Shuffle<CompAirFlowProducer>(this.CachedProducers);
            }

            this.DirtyPipeGrid(pipe.FlowType);
        }

        public void DeregisterProducer(CompAirFlowProducer pipe)
        {
            if (this.CachedProducers.Contains(pipe))
            {
                this.CachedProducers.Remove(pipe);
                GenList.Shuffle<CompAirFlowProducer>(this.CachedProducers);
            }

            this.DirtyPipeGrid(pipe.FlowType);
        }

        public void DirtyPipeGrid(AirFlowType p)
        {
            if (p == AirFlowType.Any)
            {
                this.DirtyPipeFlag[0] = true;
                this.DirtyPipeFlag[1] = true;
                return;
            }

            this.DirtyPipeFlag[(int)p] = true;
        }

        public bool ZoneAt(IntVec3 pos, AirFlowType flowType)
        {
            return this.PipeGrid[(int)flowType, this.map.cellIndices.CellToIndex(pos)] >= -1;
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();

            for (int i = 0; i < DirtyPipeFlag.Length; i++)
            {
                if (!DirtyPipeFlag[i])
                {
                    continue;
                }

                RebuildPipeGrid(i);
            }
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            foreach (var airFlowNet in CachedNets)
            {
                airFlowNet.AirFlowNetTick();
            }
        }

        private void ParseParentCell(CompAirFlow compAirFlow, int GridID, int flowIndex, AirFlowNet network)
        {
            foreach (IntVec3 current in compAirFlow.parent.OccupiedRect().EdgeCells)
            {
                ScanCell(current, GridID, flowIndex, network);
            }
        }

        public void ScanCell(IntVec3 pos, int GridID, int flowIndex, AirFlowNet network)
        {
            for (int i = 0; i < 4; i++)
            {
                var thingList = (pos + GenAdj.CardinalDirections[i]).GetThingList(this.map);
                var buildingList = thingList.OfType<Building>();

                List<CompAirFlow> list = new List<CompAirFlow>();

                foreach (Building current in buildingList)
                {
                    var buildingAirComps = current.GetComps<CompAirFlow>().Where(item => item.FlowType == (AirFlowType)flowIndex || item.FlowType == AirFlowType.Any);

                    foreach (var buildingAirComp in buildingAirComps)
                    {
                        ValidateBuilding(buildingAirComp, network);
                        list.Add(buildingAirComp);
                    }
                }

                if (list.Any())
                {
                    foreach (var compAirFlow in list)
                    {
                        if (compAirFlow.GridID == -2)
                        {
                            var iterator = compAirFlow.parent.OccupiedRect().GetIterator();
                            while (!iterator.Done())
                            {
                                IntVec3 currentItem = iterator.Current;
                                this.PipeGrid[flowIndex, this.map.cellIndices.CellToIndex(currentItem)] = GridID;
                                iterator.MoveNext();
                            }

                            compAirFlow.GridID = GridID;
                            ParseParentCell(compAirFlow, GridID, flowIndex, network);
                        }
                    }
                }
            }
        }

        private void RebuildPipeGrid(int flowIndex)
        {
            AirFlowType flowType = (AirFlowType) flowIndex;

            List<AirFlowNet> runtimeNets = new List<AirFlowNet>();

            for (int i = 0; i < this.PipeGrid.GetLength(1); i++)
            {
                this.PipeGrid[flowIndex, i] = RebuildValue;
            }

            var cachedPipes = CachedPipes.Where((item) => item.FlowType == flowType || item.FlowType == AirFlowType.Any).ToList();
            foreach (var compAirFlow in cachedPipes)
            {
                compAirFlow.GridID = RebuildValue;
            }

            Debug.Log("--- Start Rebuilding --- For Index: " + flowIndex);

            var listCopy = new List<CompAirFlow>(cachedPipes);

            for (var compAirFlow = listCopy.FirstOrDefault(); compAirFlow != null; compAirFlow = listCopy.FirstOrDefault())
            {
                compAirFlow.GridID = _masterId;

                AirFlowNet network = new AirFlowNet();
                network.GridID = compAirFlow.GridID;
                _masterId++;

                ValidateBuilding(compAirFlow, network);

                // Assign the Occipied Area to the same grid id
                CellRect.CellRectIterator iterator = compAirFlow.parent.OccupiedRect().GetIterator();
                while (!iterator.Done())
                {
                    IntVec3 current = iterator.Current;
                    this.PipeGrid[flowIndex, this.map.cellIndices.CellToIndex(current)] = compAirFlow.GridID;
                    iterator.MoveNext();
                }

                ParseParentCell(compAirFlow, compAirFlow.GridID, flowIndex, network);

                listCopy.RemoveAll(item => item.GridID != RebuildValue);

                network.AirFlowNetTick();
                Debug.Log(network.DebugString());

                runtimeNets.Add(network);
            }

            DirtyPipeFlag[flowIndex] = false;
            Debug.Log("--- Done Rebuilding ---");
            CachedNets = runtimeNets;
        }

        private void ValidateBuilding(CompAirFlow compAirFlow, AirFlowNet network)
        {
            ValidateAsProducer(compAirFlow, network);
            ValidateAsTempControl(compAirFlow, network);
        }

        private void ValidateAsProducer(CompAirFlow compAirFlow, AirFlowNet network)
        {
            var producer = compAirFlow as CompAirFlowProducer;
            if (producer != null)
            {
                if (!network.Producers.Contains(producer))
                {
                    network.Producers.Add(producer);
                }

                producer.AirFlowNet = network;
            }
        }

        private void ValidateAsTempControl(CompAirFlow compAirFlow, AirFlowNet network)
        {
            var tempControl = compAirFlow as CompAirFlowTempControl;
            if (tempControl != null)
            {
                if (!network.TempControls.Contains(tempControl))
                {
                    network.TempControls.Add(tempControl);
                }

                tempControl.AirFlowNet = network;
            }
        }
    }
}

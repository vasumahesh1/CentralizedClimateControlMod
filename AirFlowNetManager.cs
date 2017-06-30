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
        public List<CompAirFlowConsumer> CachedConsumers = new List<CompAirFlowConsumer>();
        public List<CompAirFlow> CachedPipes = new List<CompAirFlow>();

        public int[,] PipeGrid;
        public bool[] DirtyPipeFlag;
        public bool IsDirty;

        private const int RebuildValue = -2;

        private List<AirFlowNet> _backupNets = new List<AirFlowNet>();
        private int _pipeCount = 0;
        private int _masterId = 0;

        public AirFlowNetManager(Map map) : base(map)
        {
            int length = Enum.GetValues(typeof(AirFlowType)).Length;
            int num = map.AllCells.Count<IntVec3>();
            this.PipeGrid = new int[length, num];

            _pipeCount = length;

            this.DirtyPipeFlag = new bool[length];
            for (int i = 0; i < this.DirtyPipeFlag.Length; i++)
            {
                this.DirtyPipeFlag[i] = true;
            }

            IsDirty = true;
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
                GenList.Shuffle<CompAirFlowProducer>(CachedProducers);
            }

            this.DirtyPipeGrid(pipe.FlowType);
        }

        public void RegisterConsumer(CompAirFlowConsumer device)
        {
            if (!CachedConsumers.Contains(device))
            {
                CachedConsumers.Add(device);
                GenList.Shuffle<CompAirFlowConsumer>(CachedConsumers);
            }

            this.DirtyPipeGrid(device.FlowType);
        }

        public void DeregisterConsumer(CompAirFlowConsumer device)
        {
            if (this.CachedConsumers.Contains(device))
            {
                this.CachedConsumers.Remove(device);
                GenList.Shuffle<CompAirFlowConsumer>(this.CachedConsumers);
            }

            this.DirtyPipeGrid(device.FlowType);
        }

        public void DirtyPipeGrid(AirFlowType p)
        {
            IsDirty = true;
        }

        public bool ZoneAt(IntVec3 pos, AirFlowType flowType)
        {
            return this.PipeGrid[(int)flowType, this.map.cellIndices.CellToIndex(pos)] >= -1;
        }

        public bool PerfectMatch(IntVec3 pos, AirFlowType flowType, int id)
        {
            return this.PipeGrid[(int)flowType, this.map.cellIndices.CellToIndex(pos)] == id;
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();

            if (!IsDirty)
            {
                return;
            }

            foreach (var compAirFlow in CachedPipes)
            {
                compAirFlow.GridID = RebuildValue;
            }

            _backupNets.Clear();

            for (int i = 0; i < _pipeCount; i++)
            {
                if ((AirFlowType) i == AirFlowType.Any)
                {
                    continue;
                }

                RebuildPipeGrid(i);
            }

            CachedNets = _backupNets;

            map.mapDrawer.WholeMapChanged(MapMeshFlag.Buildings);

            IsDirty = false;
        }

        public override void MapComponentTick()
        {
            if (IsDirty)
            {
                return;
            }

            foreach (var airFlowNet in CachedNets)
            {
                airFlowNet.AirFlowNetTick();
            }

            base.MapComponentTick();
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
                    var buildingAirComps = current.GetComps<CompAirFlow>().Where(item => item.FlowType == (AirFlowType)flowIndex || (item.FlowType == AirFlowType.Any && item.GridID == RebuildValue));

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

            Debug.Log("--- Start Rebuilding --- For Index: " + flowIndex);

            var cachedPipes = CachedPipes.Where((item) => item.FlowType == flowType).ToList();
            PrintPipes(cachedPipes);

            var listCopy = new List<CompAirFlow>(cachedPipes);

            for (var compAirFlow = listCopy.FirstOrDefault(); compAirFlow != null; compAirFlow = listCopy.FirstOrDefault())
            {
                compAirFlow.GridID = _masterId;

                AirFlowNet network = new AirFlowNet();
                network.GridID = compAirFlow.GridID;
                network.FlowType = flowType;
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

            _backupNets.AddRange(runtimeNets);
        }

        private void ValidateBuilding(CompAirFlow compAirFlow, AirFlowNet network)
        {
            ValidateAsProducer(compAirFlow, network);
            ValidateAsTempControl(compAirFlow, network);
            ValidateAsConsumer(compAirFlow, network);
        }

        private void ValidateAsConsumer(CompAirFlow compAirFlow, AirFlowNet network)
        {
            var consumer = compAirFlow as CompAirFlowConsumer;
            if (consumer != null)
            {
                if (!network.Consumers.Contains(consumer))
                {
                    network.Consumers.Add(consumer);
                }

                consumer.AirFlowNet = network;
            }
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

        private void PrintPipes(List<CompAirFlow> comps)
        {
            string str = "\nPrinting CompAirFlows -"; 

            foreach (var compAirFlow in comps)
            {
                str += ("\n  - " + compAirFlow.parent + " (GRID ID: " + compAirFlow.GridID + ") ");
            }

            Debug.Log(str);
        }
    }
}

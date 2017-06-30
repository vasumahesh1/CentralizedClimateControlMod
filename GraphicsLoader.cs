using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedTemperature
{
    public class GraphicsLoader
    {
        public static Graphic HotPipeAtlas = GraphicDatabase.Get<Graphic_Single>("Things/Building/Hot_AirPipe_Atlas", ShaderDatabase.Transparent);
        public static Graphic HotPipeOverlayAtlas = GraphicDatabase.Get<Graphic_Single>("Things/Building/Hot_AirPipe_Overlay_Atlas", ShaderDatabase.MetaOverlay);

        public static Graphic God = GraphicDatabase.Get<Graphic_Single>("God", ShaderDatabase.Transparent);

        public static GraphicPipe_Overlay GraphicHotPipe = new GraphicPipe_Overlay(GraphicsLoader.HotPipeAtlas, AirFlowType.Hot);
        public static GraphicPipe GraphicHotPipeClear = new GraphicPipe(GraphicsLoader.God, AirFlowType.Hot);

        public static GraphicPipe_Overlay GraphicHotPipeOverlay = new GraphicPipe_Overlay(HotPipeOverlayAtlas, AirFlowType.Hot);
    }
}

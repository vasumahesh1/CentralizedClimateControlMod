using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedTemperature
{
    public class Building_HotAirPipe: Building_AirPipe
    {
        public override Graphic Graphic => GraphicsLoader.GraphicHotPipe;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedTemperature
{
    public class Building_ColdAirPipe: Building_AirPipe
    {
        public override Graphic Graphic => GraphicsLoader.GraphicColdPipe;
    }
}

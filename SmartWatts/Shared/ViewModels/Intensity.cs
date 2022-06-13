using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWatts.Shared.ViewModels
{
    public class Intensity
    {
        public string Description { get; set; }
        public string Color { get; set; }
        public string FontColor { get; set; } = "black";
        public float LowBand { get; set; }
        public float HighBand { get; set; }
        public int EffortIndex { get; set; }
    }
}

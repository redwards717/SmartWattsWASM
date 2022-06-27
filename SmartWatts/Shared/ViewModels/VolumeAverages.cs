using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWatts.Shared.ViewModels
{
    public class VolumeAverages
    {
        public int Time { get; set; }
        public int Intensity { get; set; }
        public int Anaerobic { get; set; }
        public int VO2 { get; set; }
        public int Threshold { get; set; }

    }
}

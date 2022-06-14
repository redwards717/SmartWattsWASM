using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartWatts.Shared.ViewModels;

namespace SmartWatts.Shared.UtilityModels
{
    public class EffortCalculations
    {
        public List<float> RollingValues { get; set; } = new();
        public double Target { get; set; }
        public bool InEffort { get; set; }
        public int TotalTime { get; set; }
        public int MinTime { get; set; }

        public EffortCalculations(PowerZone powerZone, int ftp)
        {
            Target = powerZone.PercentOfFTP * ftp;
            MinTime = powerZone.Time;
        }
    }
}

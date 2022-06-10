using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWatts.Shared.ViewModels
{
    public class PowerZone
    {
        public string EffortType { get; set; }
        public int Time { get; set; }
        public double PercentOfFTP { get; set; }
        public string Color { get; set; }
    }
}

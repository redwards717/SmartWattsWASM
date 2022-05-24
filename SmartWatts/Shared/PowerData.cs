using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWatts.Shared
{
    public class PowerData
    {
        public int PowerDataID { get; set; }
        public long StravaRideID { get; set; }
        public Dictionary<int, int> PowerPoints { get; set; }
        public string JsonPowerPoints { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWatts.Shared
{
    public class Activity
    {
        public int ActivityID { get; set; }
        public long StravaRideID { get; set; }
        public int StravaUserID { get; set; }
        public string Name { get; set; }
        public double Distance { get; set; }
        public double AvgSpeed { get; set; }   // comes from strava in meters/second....lol ?!
        public double MaxSpeed { get; set; }  // samsies
        public double AvgCadence { get; set; }
        public double AvgWatts { get; set; }
        public double WeightedAvgWatts { get; set; }
        public double MaxWatts { get; set; }
        public double Kilojoules { get; set; }
        public double AvgHeartrate { get; set; }
        public double MaxHeartrate { get; set; }
    }
}

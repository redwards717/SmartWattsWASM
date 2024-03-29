﻿using System;
using SmartWatts.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWatts.Shared.DBModels
{
    public class Activity
    {
        public int ActivityID { get; set; }
        public long StravaRideID { get; set; }
        public int StravaUserID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public bool IsRace { get; set; }
        public bool HasWatts { get; set; }
        public bool IsPeloton { get; set; }
        public int MovingTime { get; set; }  //seconds
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
        public int PowerDataID { get; set; }
        public PowerData PowerData { get; set; }
        public Intensity Intensity { get; set; }
        public int WeightedAvgBenchmark { get; set; }
        public PowerHistory PowerHistory { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartWatts.Shared.ViewModels;

namespace SmartWatts.Shared
{
    public static class Constants
    {
        public const string BASE_URI = "https://localhost:44306/";

        public const string STRAVA_CLIENT_SECRET = "648cca9f2c7df78a291f777faad2e371212c5848";
        public const string STRAVA_CLIENT_ID = "41884";

        public const string ANAEROBIC = "Anaerobic";
        public const string VO2 = "VO2";
        public const string THRESHOLD = "Threshold";

        public const double FTP_MULTIPLIER_450 = .85;
        public const double FTP_MULTIPLIER_1200 = .925;
        public const double FTP_MULTIPLIER_2700 = 1;

        public static List<int> PowerPoints { get; } = new List<int>(){
            5, 15, 30, 45, 60, 90, 60 * 2, 60 * 3, 60 * 5, (60 * 7) + 30, 60 * 10, 60 * 15, 60 * 20, 60 * 30, 60 * 45, 60 * 60, 60 * 90, 60 * 60 * 2, 60 * 60 * 3, 60 * 60 * 4, 60 * 60 * 5, 60 * 60 * 8, 60 * 60 * 24
        };

        public const int POWER_HISTORY_PERIOD = 42;

        public static readonly PowerZone AnaerobicPZ = new()
        {
            EffortType = ANAEROBIC,
            Time = 30,
            PercentOfFTP = 1.50,
            Color = "red"
        };

        public static readonly PowerZone VO2PZ = new()
        {
            EffortType = VO2,
            Time = 60 * 3,
            PercentOfFTP = 1.10,
            Color = "orange"
        };

        public static readonly PowerZone ThresholdPZ = new()
        {
            EffortType = THRESHOLD,
            Time = 60 * 12,
            PercentOfFTP = 1.00,
            Color = "#F1F100"
        };

        public static readonly List<PowerZone> PowerZones = new() { AnaerobicPZ, VO2PZ, ThresholdPZ };


        public static readonly List<Intensity> Intensities = new List<Intensity>()
        {
            new Intensity()
            {
                Description = "Well Above Target Intensity, Phenomenal Effort!",
                Color = "purple",
                FontColor = "white",
                LowBand = 1.1F,
                HighBand = 10000F,
                EffortIndex = 5
            },
            new Intensity()
            {
                Description = "Above Target Intensity, Great Effort",
                Color = "red",
                LowBand = 1.05F,
                HighBand = 1.1F,
                EffortIndex = 4
            },
            new Intensity()
            {
                Description = "Target Intensity Hit, Good Effort",
                Color = "orange",
                LowBand = 1.00F,
                HighBand = 1.05F,
                EffortIndex = 3
            },
            new Intensity()
            {
                Description = "Just Below Target Intensity (Maintaining)",
                Color = "#F1F100",
                LowBand = .92F,
                HighBand = 1.00F,
                EffortIndex = 2
            },
            new Intensity()
            {
                Description = "Below Target (Not Pushing)",
                Color = "lightgrey",
                LowBand = .60F,
                HighBand = .92F,
                EffortIndex = 1
            },
            new Intensity()
            {
                Description = "Easy Effort (Recovery)",
                Color = "lightgreen",
                LowBand = 0.00F,
                HighBand = .60F,
                EffortIndex = 0
            },
        };

        public enum Weeks
        {
            Monday = 1,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday,
        }

        public enum Months
        {
            January = 1,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }
    }
}

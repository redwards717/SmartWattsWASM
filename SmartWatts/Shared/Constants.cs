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

        public static List<int> PowerPoints { get; } = new List<int>(){
            5, 15, 30, 45, 60, 90, 60 * 2, 60 * 3, 60 * 5, (60 * 7) + 30, 60 * 10, 60 * 15, 60 * 20, 60 * 30, 60 * 45, 60 * 60, 60 * 90, 60 * 60 * 2, 60 * 60 * 3, 60 * 60 * 4, 60 * 60 * 5, 60 * 60 * 8, 60 * 60 * 24
        };

        public static readonly List<PowerZone> PowerZones = new()
        {
            new PowerZone()
            {
                EffortType = "Anaerobic",
                Time = 30,
                PercentOfFTP = 1.50,
                Color = "red"
            },
            new PowerZone()
            {
                EffortType = "VO2",
                Time = 60 * 3,
                PercentOfFTP = 1.20,
                Color = "orange"
            },
            new PowerZone()
            {
                EffortType = "Threshold",
                Time = 60 * 15,
                PercentOfFTP = 1.00,
                Color = "yellow"
            }
        };

        public static readonly List<Intensity> Intensities = new List<Intensity>()
        {
            new Intensity()
            {
                Description = "Well Above Target Intensity, Phenomenal Effort!",
                Color = "purple",
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
                Color = "yellow",
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
                Color = "green",
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

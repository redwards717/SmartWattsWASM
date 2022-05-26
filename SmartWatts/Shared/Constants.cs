using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWatts.Shared
{
    public static class Constants
    {
        public const string BASE_URI = "https://localhost:44306/";

        public const string STRAVA_CLIENT_SECRET = "648cca9f2c7df78a291f777faad2e371212c5848";
        public const string STRAVA_CLIENT_ID = "41884";
        public const string STRAVA_REFRESH_TOKEN = "72658f4b83ce4c476518e53e11ce50c807bd540d ";

        public static List<int> PowerPoints { get; } = new List<int>(){
            5, 15, 30, 45, 60, 90, 60 * 2, 60 * 3, 60 * 5, (60 * 7) + 30, 60 * 10, 60 * 15, 60 * 20, 60 * 30, 60 * 45, 60 * 60, 60 * 90, 60 * 60 * 2, 60 * 60 * 3, 60 * 60 * 4, 60 * 60 * 5, 60 * 60 * 8
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

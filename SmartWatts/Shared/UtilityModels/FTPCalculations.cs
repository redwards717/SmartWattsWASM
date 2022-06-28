using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWatts.Shared.UtilityModels
{
    public class FTPCalculations
    {
        public int IntervalTime { get; set; }
        public List<int> BestEfforts { get; set; }
        public double Multiplier { get; set; }
        public int BestFTP { get; set; }

        public void GetBestFTP()
        {
            int ftp = 0;

            int avgOf5 = (int)(BestEfforts.Average() * (Multiplier + .05));
            ftp = avgOf5 > ftp ? avgOf5 : ftp;

            var avgOf3 = (int)(BestEfforts.OrderByDescending(x => x).Take(3).Average() * (Multiplier + .025));
            ftp = avgOf3 > ftp ? avgOf3 : ftp;

            BestFTP = BestEfforts.Max() * Multiplier > ftp ? (int)(BestEfforts.Max() * Multiplier) : ftp;
        }
    }
}

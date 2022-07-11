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

            int avgOf3 = GetFTPCalcFor3();
            ftp = avgOf3 > ftp ? avgOf3 : ftp;

            var avgOf2 = GetFTPCalcFor2();
            ftp = avgOf2 > ftp ? avgOf2 : ftp;

            var best1 = GetFTPCalcFor1();

            BestFTP = best1 > ftp ? best1 : ftp;
        }

        public int GetFTPCalcFor3()
        {
            return (int)(BestEfforts.Average() * (Multiplier + .04));
        }

        public int GetFTPCalcFor2()
        {
            return (int)(BestEfforts.OrderByDescending(x => x).Take(2).Average() * (Multiplier + .02));
        }

        public int GetFTPCalcFor1()
        {
            return (int)(BestEfforts.Max() * Multiplier);
        }
    }
}

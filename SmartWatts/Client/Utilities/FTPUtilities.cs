using SmartWatts.Shared.UtilityModels;

namespace SmartWatts.Client.Utilities
{
    public static class FTPUtilities
    {
        public static List<FTPCalculations> GetFTPData(IEnumerable<Activity> activities)
        {
            List<FTPCalculations> ftpCalcs = new()
            {
                new() { IntervalTime = 600, Multiplier = Constants.FTP_MULTIPLIER_600, BestEfforts = new() },
                new() { IntervalTime = 1200, Multiplier = Constants.FTP_MULTIPLIER_1200, BestEfforts = new() },
                new() { IntervalTime = 1800, Multiplier = Constants.FTP_MULTIPLIER_1800, BestEfforts = new() },
                new() { IntervalTime = 2700, Multiplier = Constants.FTP_MULTIPLIER_2700, BestEfforts = new() },
                new() { IntervalTime = 3600, Multiplier = Constants.FTP_MULTIPLIER_3600, BestEfforts = new() }
            };

            foreach (Activity activity in activities)
            {
                foreach(FTPCalculations ftpCalc in ftpCalcs)
                {
                    GetBestEfforts(activity, ftpCalc, 5);
                }
            }

            return ftpCalcs;
        }

        private static void GetBestEfforts(Activity activity, FTPCalculations calc, int numberOfEfforts)
        {
            int intervalPower = activity.PowerData.PowerPoints.ContainsKey(calc.IntervalTime) ? activity.PowerData.PowerPoints[calc.IntervalTime] : 0;


            if (calc.BestEfforts.Count < numberOfEfforts || intervalPower > calc.BestEfforts.Min())
            {
                calc.BestEfforts.Add(intervalPower);
            }

            if (calc.BestEfforts.Count > numberOfEfforts)
            {
                calc.BestEfforts.Remove(calc.BestEfforts.Min());
            }
        }
    }
}

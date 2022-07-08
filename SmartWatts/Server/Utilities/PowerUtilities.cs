using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SmartWatts.Server.Utilities
{
    public static class PowerUtilities
    {
        public static PowerData CalculatePowerFromDataStream(List<StravaDataStream> sdss, int ftp)
        {
            PowerData powerData = new();
            Dictionary<int, int> powerPoints = new();
            Dictionary<int, List<float>> rollingWatts = new();

            var powerStream = sdss.Find(s => s.type == "watts");

            var powerPointsInRide = Constants.PowerPoints.Where(pp => pp <= powerStream.data.Length);

            foreach(int pp in powerPointsInRide)
            {
                rollingWatts.Add(pp, new List<float>());
                powerPoints.Add(pp, 0);
            }

            Dictionary<int, int> sustainedEfforts = new();
            EffortCalculations anEffort = new(Constants.AnaerobicPZ, ftp);
            EffortCalculations vo2Effort = new(Constants.VO2PZ, ftp);
            EffortCalculations thEffort = new(Constants.ThresholdPZ, ftp);

            for(int i = 0; i < powerStream.data.Length; i++)
            {
                //finding sustained efforts
                anEffort.RollingValues.Add(powerStream.data[i]);
                vo2Effort.RollingValues.Add(powerStream.data[i]);
                thEffort.RollingValues.Add(powerStream.data[i]);
                if(i % 5 == 0)  // potentially do this to the power curve portion to speed that up
                {
                    TrackSustainedEfforts(anEffort);
                    TrackSustainedEfforts(vo2Effort);
                    TrackSustainedEfforts(thEffort);
                }

                // calculating power curve
                foreach(int pp in powerPointsInRide)
                {
                    rollingWatts[pp].Add(powerStream.data[i]);
                    if (rollingWatts[pp].Count >= pp)
                    {
                        var rollingAvg = rollingWatts[pp].Sum() / pp;
                        if (powerPoints[pp] < rollingAvg)
                        {
                            powerPoints[pp] = (int)rollingAvg;
                        }
                        rollingWatts[pp].RemoveAt(0);
                    }
                }
            }

            ScanForFinalEfforts(anEffort);
            ScanForFinalEfforts(vo2Effort);
            ScanForFinalEfforts(thEffort);

            sustainedEfforts.Add(anEffort.MinTime, anEffort.TotalTime);
            sustainedEfforts.Add(vo2Effort.MinTime, vo2Effort.TotalTime);
            sustainedEfforts.Add(thEffort.MinTime, thEffort.TotalTime);

            powerData.SustainedEfforts = sustainedEfforts;
            powerData.JsonSustainedEfforts = JsonSerializer.Serialize(sustainedEfforts);
            powerData.PowerPoints = powerPoints;
            powerData.JsonPowerPoints = JsonSerializer.Serialize(powerPoints);

            return powerData;
        }

        public static int GetFTP(IEnumerable<Activity> activities)
        {
            FTPCalculations calc_10 = new() { IntervalTime = 600, Multiplier = Constants.FTP_MULTIPLIER_600, BestEfforts = new() };
            FTPCalculations calc_20 = new() { IntervalTime = 1200, Multiplier = Constants.FTP_MULTIPLIER_1200, BestEfforts = new() };
            FTPCalculations calc_30 = new() { IntervalTime = 1800, Multiplier = Constants.FTP_MULTIPLIER_1800, BestEfforts = new() };
            FTPCalculations calc_45 = new() { IntervalTime = 2700, Multiplier = Constants.FTP_MULTIPLIER_2700, BestEfforts = new() };
            FTPCalculations calc_60 = new() { IntervalTime = 3600, Multiplier = Constants.FTP_MULTIPLIER_3600, BestEfforts = new() };

            foreach(Activity activity in activities)
            {
                GetBestEfforts(activity, calc_10, 5);
                GetBestEfforts(activity, calc_20, 5);
                GetBestEfforts(activity, calc_30, 5);
                GetBestEfforts(activity, calc_45, 5);
                GetBestEfforts(activity, calc_60, 5);
            }

            calc_10.GetBestFTP();
            calc_20.GetBestFTP();
            calc_30.GetBestFTP();
            calc_45.GetBestFTP();
            calc_60.GetBestFTP();

            return new[] { calc_10.BestFTP, calc_20.BestFTP, calc_30.BestFTP, calc_45.BestFTP, calc_60.BestFTP }.Max();
        }

        private static void GetBestEfforts(Activity activity, FTPCalculations calc, int numberOfEfforts)
        {
            int intervalPower = activity.PowerData.PowerPoints.ContainsKey(calc.IntervalTime) ? activity.PowerData.PowerPoints[calc.IntervalTime] : 0;

            if(calc.BestEfforts.Count < numberOfEfforts || intervalPower > calc.BestEfforts.Min())
            {
                calc.BestEfforts.Add(intervalPower);
            }

            if(calc.BestEfforts.Count > numberOfEfforts)
            {
                calc.BestEfforts.Remove(calc.BestEfforts.Min());
            }
        }

        private static void TrackSustainedEfforts(EffortCalculations e)
        {
            if (e.InEffort)
            {
                if (e.RollingValues.Average() < (e.Target * .99))
                {
                    if(e.RollingValues.Count - 5 >= e.MinTime)
                    {
                        e.TotalTime += e.RollingValues.Count;
                    }
                    e.InEffort = false;
                    e.RollingValues.Clear();
                }
            }
            else if (e.RollingValues.Average() >= e.Target)
            {
                e.InEffort = true;
            }
            else
            {
                e.RollingValues.Clear();
            }
        }

        private static void ScanForFinalEfforts(EffortCalculations e)
        {
            if (e.InEffort)
            {
                e.TotalTime += (e.RollingValues.Count >= e.MinTime && e.RollingValues.Average() >= e.Target) ? e.RollingValues.Count : 0;
            }
        }
    }
}

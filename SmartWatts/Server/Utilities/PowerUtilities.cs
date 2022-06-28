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

        public static PowerHistory GetPowerHistory(Activity activity, List<Activity> activites, int lookbackDays = Constants.POWER_HISTORY_PERIOD)
        {
            DateTime endDate = activity.Date.AddMinutes(-10);
            DateTime startDate = endDate.AddDays(-lookbackDays);

            var intervals = Constants.PowerPoints.Take(activity.PowerData.PowerPoints.Count + 1);

            var ridesInRange = activites.Where(a => a.Date >= startDate && a.Date <= endDate);

            Dictionary<int, List<int>> bestPower = new();

            foreach (int pp in intervals)
            {
                bestPower.Add(pp, new List<int>() { 0, 0, 0 });
            }

            foreach (Activity r in ridesInRange)
            {
                foreach (var pp in bestPower)
                {
                    var powerPoint = r.PowerData.PowerPoints.FirstOrDefault(pc => pc.Key == pp.Key);
                    if (powerPoint.Value > pp.Value.Min())
                    {
                        pp.Value.Remove(pp.Value.Min());
                        pp.Value.Add(powerPoint.Value);
                    }
                }
            }

            Dictionary<int, int> bestPowerAvg = new();

            foreach (var bp in bestPower)
            {
                var avg = (int)(bp.Value.Sum() / 3);
                bestPowerAvg.Add(bp.Key, avg);
            }

            return new PowerHistory()
            {
                LookbackDays = lookbackDays,
                PowerPoints = bestPowerAvg
            };
        }

        public static Intensity GetRideIntensity(Activity activity)
        {
            Intensity topIntensity = Constants.Intensities.Find(i => i.EffortIndex == 0);

            foreach (var powerPoint in activity.PowerData.PowerPoints)
            {
                var intensity = GetEffortIntensity(powerPoint, activity.PowerHistory);
                if (intensity.EffortIndex > topIntensity.EffortIndex)
                {
                    topIntensity = intensity;
                }

                if (topIntensity.EffortIndex >= DefaultToMaxIntensity().EffortIndex)
                {
                    break;
                }
            }

            int bestEffortIntensity = topIntensity.EffortIndex;

            var weightedAvgBenchmark = GetWeightedAvgBenchmark(activity);

            activity.WeightedAvgBenchmark = weightedAvgBenchmark == 0 ? 1 : weightedAvgBenchmark;

            var compRatio = activity.WeightedAvgWatts / activity.WeightedAvgBenchmark;

            int fullRideEffort = Constants.Intensities.Find(i => compRatio >= i.LowBand && compRatio <= i.HighBand).EffortIndex;

            int avgEffort = (bestEffortIntensity + fullRideEffort) / 2;

            if ((bestEffortIntensity == 1 && fullRideEffort == 0) || (bestEffortIntensity == 0 && fullRideEffort == 1))
            {
                // cant be a recovery ride if any effort takes it out of the green
                avgEffort = 1;
            }

            return Constants.Intensities.Find(i => i.EffortIndex == avgEffort);
        }

        public static Intensity GetEffortIntensity(KeyValuePair<int, int> powerPoint, PowerHistory powerHistory)
        {
            int historicalEffort = powerHistory.PowerPoints.FirstOrDefault(pp => powerPoint.Key == pp.Key).Value;
            if (historicalEffort == 0)
            {
                return Constants.Intensities.Find(i => i.EffortIndex == 0);
            }

            double comp = (double)powerPoint.Value / historicalEffort;

            return Constants.Intensities.Find(i => comp >= i.LowBand && comp <= i.HighBand);
        }

        public static int GetFTP(IEnumerable<Activity> activities)
        {
            FTPCalculations calc_7point5 = new() { IntervalTime = 450, Multiplier = Constants.FTP_MULTIPLIER_450, BestEfforts = new() };
            FTPCalculations calc_20 = new() { IntervalTime = 1200, Multiplier = Constants.FTP_MULTIPLIER_1200, BestEfforts = new() };
            FTPCalculations calc_45 = new() { IntervalTime = 2700, Multiplier = Constants.FTP_MULTIPLIER_2700, BestEfforts = new() };

            foreach(Activity activity in activities)
            {
                GetBestEfforts(activity, calc_7point5);
                GetBestEfforts(activity, calc_20);
                GetBestEfforts(activity, calc_45);
            }

            calc_7point5.GetBestFTP();
            calc_20.GetBestFTP();
            calc_45.GetBestFTP();

            return new[] { calc_7point5.BestFTP, calc_20.BestFTP, calc_45.BestFTP }.Max();
        }

        private static void GetBestEfforts(Activity activity, FTPCalculations calc)
        {
            int intervalPower = activity.PowerData.PowerPoints.ContainsKey(calc.IntervalTime) ? activity.PowerData.PowerPoints[calc.IntervalTime] : 0;


            if(calc.BestEfforts.Count < 5 || intervalPower > calc.BestEfforts.Min())
            {
                calc.BestEfforts.Add(intervalPower);
            }

            if(calc.BestEfforts.Count > 5)
            {
                calc.BestEfforts.Remove(calc.BestEfforts.Min());
            }
        }

        private static Intensity DefaultToMaxIntensity()
        {
            var max = Constants.Intensities.Max(i => i.EffortIndex);
            return Constants.Intensities.Find(i => i.EffortIndex == max);
        }

        private static int GetWeightedAvgBenchmark(Activity activity)
        {
            var time = activity.MovingTime;

            int lowInterval = 0;
            int highInterval = 0;

            for (int i = 0; i <= activity.PowerHistory.PowerPoints.Count; i++)
            {
                if (Constants.PowerPoints[i] >= time)
                {
                    highInterval = Constants.PowerPoints[i];
                    lowInterval = Constants.PowerPoints[i - 1];
                    break;
                }
            }

            int lowIntervalPower = activity.PowerHistory.PowerPoints.ContainsKey(lowInterval) ? activity.PowerHistory.PowerPoints[lowInterval] : 0;
            int highIntervalPower = activity.PowerHistory.PowerPoints.ContainsKey(highInterval) ? activity.PowerHistory.PowerPoints[highInterval] : 0;

            if (lowIntervalPower == 0)
            {
                return 0;
            }

            int timeDiff = highInterval - lowInterval;
            double percentIntoInterval = (double)(time - lowInterval) / timeDiff;

            int powerIntervalDiff = Math.Abs(highIntervalPower - lowIntervalPower);
            double powerAdj = percentIntoInterval * powerIntervalDiff;

            double powerCompForExactTime = lowIntervalPower - powerAdj;
            return (int)powerCompForExactTime;
        }

        private static void TrackSustainedEfforts(EffortCalculations e)
        {
            try
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
            catch(Exception ex)
            {
                var test = ex;
                throw;
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

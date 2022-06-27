namespace SmartWatts.Client.Utilities
{
    public static class PowerUtlities
    {
        public static int GetVolumeInTime(List<Activity> activities, DateTime start, DateTime end)
        {
            int time = activities.Where(a => a.Date >= start && a.Date <= end).Sum(a => a.MovingTime);
            return time;
        }

        public static int GetAvgVolume(List<Activity> activities, DateTime start, DateTime end, int periods)
        {
            int time = activities.Where(a => a.Date >= start && a.Date <= end).Sum(a => a.MovingTime);
            return time / periods;
        }

        public static int GetIntensityForTimeframe(List<Activity> activities, DateTime start, DateTime end)
        {
            return activities.Where(a => a.Date >= start && a.Date <= end && a.Intensity.EffortIndex > 1).Sum(a => a.Intensity.EffortIndex);
        }

        public static int GetAvgIntensity(List<Activity> activities, DateTime start, DateTime end, int periods)
        {
            return activities.Where(a => a.Date >= start && a.Date <= end).Sum(a => a.Intensity.EffortIndex) /periods;
        }

        public static int GetSustainedEfforts(List<Activity> activities, DateTime start, DateTime end, int effortTime)
        {
            var efforts = activities.Where(a => a.Date >= start && a.Date <= end).Select(a => a.PowerData.SustainedEfforts);
            int time = 0;

            foreach(var effort in efforts)
            {
                time += effort[effortTime];
            }

            return time;
        }

        public static int GetAvgSustainedEfforts(List<Activity> activities, DateTime start, DateTime end, int effortTime, int periods)
        {
            var efforts = activities.Where(a => a.Date >= start && a.Date <= end).Select(a => a.PowerData.SustainedEfforts);
            int time = 0;

            foreach (var effort in efforts)
            {
                time += effort[effortTime];
            }

            return time / periods;
        }

        public static VolumeAverages GetAllAverages(List<Activity> activities, DateTime start, DateTime end, int periods)
        {
            return new VolumeAverages()
            {
                Time = GetAvgVolume(activities, start, end, periods),
                Intensity = GetAvgIntensity(activities, start, end, periods),
                Anaerobic = GetAvgSustainedEfforts(activities, start, end, Constants.AnaerobicPZ.Time, periods),
                VO2 = GetAvgSustainedEfforts(activities, start, end, Constants.VO2PZ.Time, periods),
                Threshold = GetAvgSustainedEfforts(activities, start, end, Constants.ThresholdPZ.Time, periods)
            };
        }

        public static Intensity GetRideIntensity(Activity activity)
        {
            Intensity topIntensity = Constants.Intensities.Find(i => i.EffortIndex == 0);

            foreach(var powerPoint in activity.PowerData.PowerPoints)
            {
                var intensity = GetEffortIntensity(powerPoint, activity.PowerHistory);
                if(intensity.EffortIndex > topIntensity.EffortIndex)
                {
                    topIntensity = intensity;
                }

                if(topIntensity.EffortIndex >= DefaultToMaxIntensity().EffortIndex)
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

            if((bestEffortIntensity == 1 && fullRideEffort == 0) || (bestEffortIntensity == 0 && fullRideEffort == 1))
            {
                // cant be a recovery ride if any effort takes it out of the green
                avgEffort = 1;
            }

            return Constants.Intensities.Find(i => i.EffortIndex == avgEffort);
        }

        public static Intensity GetIntensityFromEffortPercent(int percent)
        {
            var compRatio = (double)percent / 100;

            return Constants.Intensities.Find(i => compRatio >= i.LowBand && compRatio <= i.HighBand);
        }

        public static Intensity GetEffortIntensity(KeyValuePair<int,int> powerPoint, PowerHistory powerHistory)
        {
            int historicalEffort = powerHistory.PowerPoints.FirstOrDefault(pp => powerPoint.Key == pp.Key).Value;
            if(historicalEffort == 0)
            {
                return Constants.Intensities.Find(i => i.EffortIndex == 0);
            }

            double comp = (double)powerPoint.Value / historicalEffort;

            return Constants.Intensities.Find(i => comp >= i.LowBand && comp <= i.HighBand);
        }

        public static PowerHistory GetPowerHistory(Activity activity, List<Activity> activites, int lookbackDays = Constants.POWER_HISTORY_PERIOD)
        {
            DateTime endDate = activity.Date.AddMinutes(-10);
            DateTime startDate = endDate.AddDays(-lookbackDays);

            var intervals = Constants.PowerPoints.Take(activity.PowerData.PowerPoints.Count + 1);

            var ridesInRange = activites.Where(a => a.Date >= startDate && a.Date <= endDate);

            Dictionary<int, List<int>> bestPower = new();

            foreach(int pp in intervals)
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

        private static int GetWeightedAvgBenchmark(Activity activity)
        {
            var time = activity.MovingTime;

            int lowInterval = 0;
            int highInterval = 0;

            for(int i = 0; i <= activity.PowerHistory.PowerPoints.Count; i++)
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

            if(lowIntervalPower == 0)
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

        private static Intensity DefaultToMaxIntensity()
        {
            var max = Constants.Intensities.Max(i => i.EffortIndex);
            return Constants.Intensities.Find(i => i.EffortIndex == max);
        }
    }
}

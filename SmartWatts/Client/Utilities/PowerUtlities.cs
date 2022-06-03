namespace SmartWatts.Client.Utilities
{
    public static class PowerUtlities
    {
        public static string GetVolumeInTime(List<Activity> activities, DateTime start, DateTime end)
        {
            int time = activities.Where(a => a.Date >= start && a.Date <= end).Sum(a => a.MovingTime);
            return DateTimeUtilities.ConvertSecToReadable(time);
        }

        public static Intensity GetRideIntensity(Activity activity, PowerHistory powerHistory)
        {
            Intensity topIntensity = new();

            foreach(var powerPoint in activity.PowerData.PowerPoints)
            {
                var intensity = GetEffortIntensity(powerPoint, powerHistory);
                if(intensity.EffortIndex > topIntensity.EffortIndex)
                {
                    topIntensity = intensity;
                }
                if(topIntensity.EffortIndex >= 6)
                {
                    break;
                }
            }

            int bestEffortIntensity = topIntensity.EffortIndex;

            int fullRideEffort = GetFullRideIntensity(activity, powerHistory).EffortIndex;

            int avgEffort = (bestEffortIntensity + fullRideEffort) / 2;

            return Constants.Intensities.Find(i => i.EffortIndex == avgEffort);
        }

        public static Intensity GetEffortIntensity(KeyValuePair<int,int> powerPoint, PowerHistory powerHistory)
        {
            int historicalEffort = powerHistory.PowerPoints.FirstOrDefault(pp => powerPoint.Key == pp.Key).Value;
            if(historicalEffort == 0)
            {
                return Constants.Intensities[0];
            }

            double comp = (double)powerPoint.Value / historicalEffort;

            return Constants.Intensities.Find(i => comp >= i.LowBand && comp <= i.HighBand);
        }

        public static PowerHistory GetPowerHistory(Activity activity, int lookbackDays, List<Activity> activites)
        {
            DateTime endDate = activity.Date.AddMinutes(-10);
            DateTime startDate = endDate.AddDays(-lookbackDays);

            int longestInterval = activity.PowerData.PowerPoints.Keys.Max();

            var ridesInRange = activites.Where(a => a.Date >= startDate && a.Date <= endDate);

            Dictionary<int, List<int>> bestPower = new();

            foreach(int pp in Constants.PowerPoints.Where(pp => pp <= longestInterval))
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

        private static Intensity GetFullRideIntensity(Activity activity, PowerHistory powerHistory)
        {
            var time = activity.MovingTime;

            int lowInterval = 0;
            int highInterval = 0;

            for(int i = 0; i <= powerHistory.PowerPoints.Count; i++)
            {
                if (Constants.PowerPoints[i] >= time)
                {
                    highInterval = Constants.PowerPoints[i];
                    lowInterval = Constants.PowerPoints[i - 1];
                }
            }

            int lowIntervalPower = powerHistory.PowerPoints[lowInterval];
            int highIntervalPower = powerHistory.PowerPoints[highInterval];

            int timeDiff = highInterval - lowInterval;
            double percentIntoInterval = (double)time / timeDiff;

            int powerIntervalDiff = Math.Abs(highIntervalPower - lowIntervalPower);
            double powerAdj = percentIntoInterval * powerIntervalDiff;

            double powerCompForExactTime = lowIntervalPower - powerAdj;

            var compRatio = activity.WeightedAvgWatts / powerCompForExactTime;

            return Constants.Intensities.Find(i => compRatio >= i.LowBand && compRatio <= i.HighBand);
        }
    }
}

namespace SmartWatts.Client.Utilities
{
    public static class IntensityUtilities
    {
        public static int GetIntensityForTimeframe(List<Activity> activities, DateTime start, DateTime end)
        {
            int intensitySum = 0;
            var actInRange = activities.Where(a => a.Date >= start.EndOfDay() && a.Date <= end.EndOfDay() && a.Intensity.EffortIndex > 1);
            var daysOfYear = actInRange.Select(a => a.Date.DayOfYear).Distinct();

            foreach (int day in daysOfYear)
            {
                // multiple rides in a single day shouldnt hold more weight then a single longer ride. but a slight bonus for 2 equally intense rides done in one day.
                var actForDay = actInRange.Where(a => a.Date.DayOfYear == day);
                int maxIntensity = actForDay.Max(a => a.Intensity.EffortIndex);
                int maxRides = actForDay.Count(a => a.Intensity.EffortIndex == maxIntensity);
                intensitySum += maxRides > 1 ? maxIntensity + 1 : maxIntensity;
            }

            return intensitySum;

            //return activities.Where(a => a.Date >= start.EndOfDay() && a.Date <= end.EndOfDay() && a.Intensity.EffortIndex > 1).Sum(a => a.Intensity.EffortIndex);
        }

        //need to update this function to match the one above which removes extra intensity for multiple rides in a single day
        public static int GetIntensityForTimeframe(List<Activity> activities, int year, int month)
        {
            DateTime start = new(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DateTime end = new(year, month, daysInMonth);

            return GetIntensityForTimeframe(activities, start, end.AddDays(1).AddSeconds(-1));
            //var activitiesInRange = activities.Where(a => a.Date.Year == year && a.Date.Month == month && a.Intensity.EffortIndex > 1);
            //return activitiesInRange.Sum(a => a.Intensity.EffortIndex);
        }

        public static int GetAvgIntensity(List<Activity> activities, DateTime start, DateTime end, int periods)
        {
            return activities.Where(a => a.Date >= start.EndOfDay() && a.Date <= end.EndOfDay()).Sum(a => a.Intensity.EffortIndex) / periods;
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

        public static Intensity GetIntensityFromEffortPercent(int percent)
        {
            var compRatio = (double)percent / 100;

            return Constants.Intensities.Find(i => compRatio >= i.LowBand && compRatio <= i.HighBand);
        }

        public static Intensity GetEffortIntensity(KeyValuePair<int, int> powerPoint, PowerHistory powerHistory)
        {
            int historicalEffort = powerHistory.PowerPoints.FirstOrDefault(pp => powerPoint.Key == pp.Key).Value;
            if (historicalEffort == 0)
            {
                return DefaultToMaxIntensity();
            }

            double comp = (double)powerPoint.Value / historicalEffort;

            return Constants.Intensities.Find(i => comp >= i.LowBand && comp <= i.HighBand);
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

        private static Intensity DefaultToMaxIntensity()
        {
            var max = Constants.Intensities.Max(i => i.EffortIndex);
            return Constants.Intensities.Find(i => i.EffortIndex == max);
        }
    }
}

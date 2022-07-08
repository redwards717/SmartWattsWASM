namespace SmartWatts.Client.Utilities
{
    public static class PowerCurveUtilities
    {
        public static PowerHistory GetPowerHistory(Activity activity, List<Activity> activites, int lookbackDays = Constants.POWER_HISTORY_PERIOD)
        {
            DateTime endDate = activity.Date.AddMinutes(-5);
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

        public static PowerHistory GetPowerHistoryForProgression(DateTime lookBackFrom, List<Activity> activites, int lookbackDays = Constants.POWER_HISTORY_PERIOD)
        {
            DateTime endDate = lookBackFrom.Date;
            DateTime startDate = endDate.AddDays(-lookbackDays);

            var activitiesInRange = activites.Where(a => a.Date >= startDate && a.Date <= endDate);

            Dictionary<int, List<int>> bestPower = new();

            foreach (int pp in Constants.PowerPoints)
            {
                bestPower.Add(pp, new List<int>() { 0, 0, 0 });
            }

            foreach (Activity a in activitiesInRange)
            {
                foreach (var pp in bestPower)
                {
                    var powerPoint = a.PowerData.PowerPoints.FirstOrDefault(pc => pc.Key == pp.Key);
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

        public static int GetSinglePowerPoint(List<Activity> activity, int pp, int year, int month = 0)
        {
            List<int> bestEfforts = new() { 0,0,0};
            var activitiesInRange = activity.Where(a => a.Date.Year == year);

            if(month >= 1)
            {
                activitiesInRange = activitiesInRange.Where(a => a.Date.Month == month);
            }

            foreach(Activity a in activitiesInRange)
            {
                var powerPoint = a.PowerData.PowerPoints.FirstOrDefault(pc => pc.Key == pp);
                if (powerPoint.Value > bestEfforts.Min())
                {
                    bestEfforts.Remove(bestEfforts.Min());
                    bestEfforts.Add(powerPoint.Value);
                }
            }

            return (int)bestEfforts.Average();
        }
    }
}

namespace SmartWatts.Client.Utilities
{
    public static class VolumeUtilities
    {
        public static int GetVolumeInTime(List<Activity> activities, DateTime start, DateTime end)
        {
            return activities.Where(a => a.Date >= start.EndOfDay() && a.Date <= end.EndOfDay()).Sum(a => a.MovingTime);
        }

        public static int GetVolumeInTime(List<Activity> activities, int year, int month = 0)
        {
            var activitiesInRange = month == 0 ? activities.Where(a => a.Date.Year == year) : activities.Where(a => a.Date.Year == year && a.Date.Month == month);
            return activitiesInRange.Sum(a => a.MovingTime);
        }

        public static int GetAvgVolume(List<Activity> activities, DateTime start, DateTime end, int periods)
        {
            int time = activities.Where(a => a.Date >= start.EndOfDay() && a.Date <= end.EndOfDay()).Sum(a => a.MovingTime);
            return time / periods;
        }

        public static int GetSustainedEfforts(List<Activity> activities, DateTime start, DateTime end, int effortTime)
        {
            var efforts = activities.Where(a => a.Date >= start.EndOfDay() && a.Date <= end.EndOfDay()).Select(a => a.PowerData.SustainedEfforts);
            int time = 0;

            foreach (var effort in efforts)
            {
                time += effort[effortTime];
            }

            return time;
        }

        public static int GetSustainedEfforts(List<Activity> activities, int effortTime, int year, int month = 0)
        {
            var activitiesInRange = month == 0 ? activities.Where(a => a.Date.Year == year) : activities.Where(a => a.Date.Year == year && a.Date.Month == month);
            var efforts = activitiesInRange.Where(a => a.Date.Year == year && a.Date.Month <= month).Select(a => a.PowerData.SustainedEfforts);
            int time = 0;

            foreach (var effort in efforts)
            {
                time += effort[effortTime];
            }

            return time;
        }

        public static int GetAvgSustainedEfforts(List<Activity> activities, DateTime start, DateTime end, int effortTime, int periods)
        {
            var efforts = activities.Where(a => a.Date >= start.EndOfDay() && a.Date <= end.EndOfDay()).Select(a => a.PowerData.SustainedEfforts);
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
                Intensity = IntensityUtilities.GetAvgIntensity(activities, start, end, periods),
                Anaerobic = GetAvgSustainedEfforts(activities, start, end, Constants.AnaerobicPZ.Time, periods),
                VO2 = GetAvgSustainedEfforts(activities, start, end, Constants.VO2PZ.Time, periods),
                Threshold = GetAvgSustainedEfforts(activities, start, end, Constants.ThresholdPZ.Time, periods)
            };
        }

        public static int GetActivityCount(List<Activity> activities, int year, int month = 0)
        {
            return month == 0 ? activities.Count(a => a.Date.Year == year) : activities.Count(a => a.Date.Year == year && a.Date.Month == month);
        }
    }
}

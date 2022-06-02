namespace SmartWatts.Client.Utilities
{
    public static class PowerUtlities
    {
        public static string GetVolumeInTime(List<Activity> activities, DateTime start, DateTime end)
        {
            var time = activities.Where(a => a.Date >= start && a.Date <= end).Sum(a => a.MovingTime);
            return DateTimeUtilities.ConvertSecToReadable(time);
        }
    }
}

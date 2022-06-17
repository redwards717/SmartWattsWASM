namespace SmartWatts.Client.Utilities
{
    public static class DateTimeUtilities
    {
        public static DateTime UnixToDateTime(long unixTime)
        {
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(unixTime).ToLocalTime();
        }

        public static long ToUnixSeconds(this DateTime dateTime)
        {
            return ((DateTimeOffset)dateTime.AddDays(-1)).ToUnixTimeSeconds();
        }

        public static List<int> GetActiveYears(List<Activity> activities)
        {
            var years = activities.Select(a => a.Date.Year).Distinct().ToList();

            return years;
        }

        public static string ConvertSecToReadable(int secs, bool includeSeconds = true)
        {
            var hours = (int)(secs / 3600);
            var secsLeftInHour = secs % 3600;
            var minutes = (int)(secsLeftInHour / 60);
            var seconds = secsLeftInHour % 60;

            var strHours = hours >= 1 ? $"{hours}h" : " ";
            var strMins = minutes >= 1 ? $" {minutes}m " : " ";
            var strSecs = seconds >= 1 ? $"{seconds}s" : "";

            if (includeSeconds)
            {
                return strHours + strMins + strSecs;
            }
            else
            {
                return strHours + strMins;
            }
        }
    }
}

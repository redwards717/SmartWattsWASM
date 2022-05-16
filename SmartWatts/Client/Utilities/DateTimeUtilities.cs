namespace SmartWatts.Client.Utilities
{
    public static class DateTimeUtilities
    {
        public static DateTime UnixToDateTime(long unixTime)
        {
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(unixTime).ToLocalTime();
        }
    }
}

namespace SmartWatts.Server.Utilities
{
    public static class ConverstionUtilities
    {
        public static List<Activity> ConvertStravaActivity(IEnumerable<StravaActivity> stravaActivities)
        {
            List<Activity> activities = new();
            foreach (StravaActivity sa in stravaActivities)
            {
                activities.Add(new Activity()
                {
                    StravaRideID = sa.id,
                    StravaUserID = sa.athlete.id,
                    Name = sa.name,
                    Date = sa.start_date_local,
                    Type = sa.type,
                    IsRace = sa.workout_type == 11,
                    IsPeloton = (sa.name.Contains("Ride with") && sa.name.Contains(" min "))
                            || (sa.name.Contains("RIde with") && sa.name.Contains(" min "))
                            || (sa.name.Contains("Just Ride") && sa.name.Contains(" min "))
                            || (sa.name.Contains("Scenic Ride") && sa.name.Contains(" min "))
                            || (sa.name.Contains("Ministry of Sound:") && sa.name.Contains(" min "))
                            || (sa.name.Contains(" Mood Ride: ") && sa.name.Contains(" min "))
                            || (sa.name.Contains(" Ride: ") && sa.name.Contains(" min AFO")),
                    HasWatts = sa.device_watts,
                    MovingTime = sa.moving_time,
                    Distance = sa.distance,
                    AvgSpeed = sa.average_speed,
                    MaxSpeed = sa.max_speed,
                    AvgCadence = sa.average_cadence,
                    AvgWatts = sa.average_watts,
                    MaxWatts = sa.max_watts,
                    WeightedAvgWatts = sa.weighted_average_watts,
                    Kilojoules = sa.kilojoules,
                    AvgHeartrate = sa.average_heartrate,
                    MaxHeartrate = sa.max_heartrate
                });
            }

            return activities;
        }
    }
}

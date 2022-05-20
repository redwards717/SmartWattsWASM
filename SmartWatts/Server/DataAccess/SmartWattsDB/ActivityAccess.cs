namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public class ActivityAccess : IActivityAccess
    {
        private readonly ISqlDataAccess _db;

        public ActivityAccess(ISqlDataAccess db)
        {
            _db = db;
        }

        public Task<List<Activity>> GetActivitiesByUser(string id)
        {
            var parameters = new { id };

            const string sql = @"SELECT * FROM Activities
                                    WHERE StravaUserID = @id";

            return _db.LoadData<Activity, dynamic>(sql, parameters);
        }

        public Task InsertActivities(List<Activity> activities)
        {
            const string sql = @"INSERT INTO Activities(StravaRideID, StravaUserID, Name, Distance, AvgSpeed, MaxSpeed, AvgCadence, AvgWatts, WeightedAvgWatts, MaxWatts, Kilojoules, AvgHeartrate, MaxHeartrate)
                                    VALUES(@StravaRideID, @StravaUserID, @Name, @Distance, @AvgSpeed, @MaxSpeed, @AvgCadence, @AvgWatts, @WeightedAvgWatts, @MaxWatts, @Kilojoules, @AvgHeartrate, @MaxHeartrate)";

            return _db.SaveData(sql, activities);
        }
    }
}

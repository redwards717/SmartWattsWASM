using SmartWatts.Server.DataAccess.SmartWattsDB.Interfaces;
using SmartWatts.Shared.DBModels;

namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public class ActivityAccess : IActivityAccess
    {
        private readonly ISqlDataAccess _db;

        public ActivityAccess(ISqlDataAccess db)
        {
            _db = db;
        }

        public Task<List<Activity>> GetActivitiesByStravaUserID(string id)
        {
            var parameters = new { id };

            const string sql = @"SELECT * FROM Activities
                                    WHERE StravaUserID = @id
                                    ORDER BY Date DESC";

            return _db.LoadData<Activity, dynamic>(sql, parameters);
        }

        public Task<List<long>> GetRecentActivityIDsForUser(string id, string count)
        {
            var parameters = new { id, count };

            const string sql = @"SELECT TOP(@count) StravaRideID
                                    FROM Activities
                                    WHERE StravaUserID = @id
                                    ORDER BY Date desc";

            return _db.LoadData<long, dynamic>(sql, parameters);
        }

        public async Task<Activity> GetActivityByStravaRideID(string id)
        {
            var parameters = new { id };

            const string sql = @"SELECT * FROM Activities
                                    WHERE StravaRideID = @id";

            return (await _db.LoadData<Activity, dynamic>(sql, parameters)).FirstOrDefault();
        }

        public Task InsertActivities(List<Activity> activities)
        {
            const string sql = @"INSERT INTO Activities(StravaRideID, StravaUserID, Name, Date, Type, IsRace, HasWatts, IsPeloton, MovingTime, Distance, AvgSpeed, MaxSpeed, AvgCadence, AvgWatts, WeightedAvgWatts, MaxWatts, Kilojoules, AvgHeartrate, MaxHeartrate)
                                    VALUES(@StravaRideID, @StravaUserID, @Name, @Date, @Type, @IsRace, @HasWatts, @IsPeloton, @MovingTime, @Distance, @AvgSpeed, @MaxSpeed, @AvgCadence, @AvgWatts, @WeightedAvgWatts, @MaxWatts, @Kilojoules, @AvgHeartrate, @MaxHeartrate)";

            return _db.SaveData(sql, activities);
        }

        public Task UpdatePower(List<Activity> activities)
        {
            const string sql = @"UPDATE Activities
                                    SET AvgWatts = @AvgWatts, WeightedAvgWatts = @WeightedAvgWatts, MaxWatts = @MaxWatts, Kilojoules = @Kilojoules
                                    WHERE StravaRideID = @StravaRideID";

            return _db.SaveData(sql, activities);
        }

        public Task SetIsRace(Activity activity)
        {
            const string sql = @"UPDATE Activities
                                    SET IsRace = @IsRace
                                    WHERE StravaRideID = @StravaRideID";

            return _db.SaveData(sql, activity);
        }
    }
}

namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public class PowerDataAccess : IPowerDataAccess
    {
        private readonly ISqlDataAccess _db;

        public PowerDataAccess(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<PowerData> GetPowerDataForActivity(Activity activity)
        {
            const string sql = @"SELECT * FROM PowerData
                            WHERE StravaRideID = @StravaRideID";

            return (await _db.LoadData<PowerData, Activity>(sql, activity)).FirstOrDefault();
        }

        public Task InsertPowerData(PowerData powerData)
        {
            const string sql = @"INSERT INTO PowerData(StravaRideID, JsonPowerPoints, JsonSustainedEfforts, FTPAtTimeOfRide)
                            VALUES(@StravaRideID, @JsonPowerPoints, @JsonSustainedEfforts, @FTPAtTimeOfRide)";

            return _db.SaveData(sql, powerData);
        }

        public Task InsertPowerData(List<PowerData> powerData)
        {
            const string sql = @"INSERT INTO PowerData(StravaRideID, JsonPowerPoints, JsonSustainedEfforts, FTPAtTimeOfRide)
                            VALUES(@StravaRideID, @JsonPowerPoints, @JsonSustainedEfforts, @FTPAtTimeOfRide)";

            return _db.SaveData(sql, powerData);
        }

        public Task UpdatePowerData(List<PowerData> powerData)
        {
            const string sql = @"UPDATE PowerData
                                    SET JsonPowerPoints = @JsonPowerPoints
                                    WHERE StravaRideID = @StravaRideID";

            return _db.SaveData(sql, powerData);
        }
    }
}

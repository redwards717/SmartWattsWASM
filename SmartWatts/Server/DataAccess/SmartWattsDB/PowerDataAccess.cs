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
            string sql = @"SELECT * FROM Activities
                            WHERE ActivityID = @ActivityID";

            return (await _db.LoadData<PowerData, Activity>(sql, activity)).FirstOrDefault();
        }

        public Task InsertPowerData(PowerData powerData)
        {
            string sql = @"INSERT INTO PowerData(ActivityID, JsonPowerPoints)
                            VALUES(@ActivityID, @JsonPowerPoints)";

            return _db.SaveData(sql, powerData);
        }
    }
}

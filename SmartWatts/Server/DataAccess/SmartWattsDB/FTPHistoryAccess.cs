namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public class FTPHistoryAccess : IFTPHistoryAccess
    {
        private readonly ISqlDataAccess _db;

        public FTPHistoryAccess(ISqlDataAccess db)
        {
            _db = db;
        }

        public Task InsertFTPHistory(FTPHistory fTPHistory)
        {
            const string sql = @"INSERT INTO FTPHistory(StravaUserID, Date, FTP)
                                    VALUES(@StravaUserID, @Date, @FTP)";

            return _db.SaveData(sql, fTPHistory);
        }

        public Task<List<FTPHistory>> GetFTPHistoryByStravaUserID(string id)
        {
            var parameters = new { id };

            const string sql = @"SELECT * FROM FTPHistory
                                    WHERE StravaUserID = @id";

            return _db.LoadData<FTPHistory, dynamic>(sql, parameters);
        }
    }
}

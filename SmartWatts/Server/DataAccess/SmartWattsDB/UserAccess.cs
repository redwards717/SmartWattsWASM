using SmartWatts.Shared.DBModels;

namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public class UserAccess : IUserAccess
    {
        private readonly ISqlDataAccess _db;

        public UserAccess(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<User> GetUserById(string id)
        {
            var parameters = new { id };

            const string sql = @"SELECT * 
                            FROM Users
                            WHERE UserID = @id";

            var result = await _db.LoadData<User, dynamic>(sql, parameters);
            return result.FirstOrDefault();
        }
        public async Task<User> GetUser(string email, string password)
        {
            var parameter = new { email, password };

            const string sql = @"SELECT * FROM Users
                            WHERE Email = @email AND Password = @password";

            var user = await _db.LoadData<User, dynamic>(sql, parameter);

            return user.FirstOrDefault();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var parameter = new { email };

            const string sql = @"SELECT * FROM Users
                            WHERE Email = @email";

            var foundUser = await _db.LoadData<User, dynamic>(sql, parameter);

            return foundUser.FirstOrDefault();
        }

        public Task InsertUser(User user)
        {
            const string sql = @"INSERT INTO Users (UserID, Email, Password, StravaUserID)
                                        VALUES(@UserId, @Email, @Password, @StravaUserID)";

            return _db.SaveData(sql, user);
        }

        public Task UpdateUser(User user)
        {
            const string sql = @"UPDATE Users
                            SET Email = @Email, FTP = @FTP, StravaUserID = @StravaUserID, StravaAccessToken = @StravaAccessToken, TokenExpiration = @TokenExpiration, RefreshToken = @RefreshToken
                            WHERE UserID = @UserId";

            return _db.SaveData(sql, user);
        }
    }
}

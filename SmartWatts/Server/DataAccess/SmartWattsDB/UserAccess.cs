using SmartWatts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

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

            string sql = @"SELECT UserID, Email, StravaAccessCode 
                            FROM Users
                            WHERE UserID = @id";

            var result = await _db.LoadData<User, dynamic>(sql, parameters);
            return result.FirstOrDefault();
        }
        public async Task<User> GetUser(string email, string password)
        {

            var parameter = new { email, password };

            string sql = @"SELECT * FROM Users
                            WHERE Email = @email AND Password = @password";

            var user = await _db.LoadData<User, dynamic>(sql, parameter);

            return user.FirstOrDefault();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var parameter = new { email };

            string sql = @"SELECT * FROM Users
                            WHERE Email = @email";

            var foundUser = await _db.LoadData<User, dynamic>(sql, parameter);

            return foundUser.FirstOrDefault();
        }

        public async Task InsertUser(User user)
        {
            string sql = @"INSERT INTO Users (UserID, Email, Password, StravaAccessCode)
                                        VALUES(@UserId, @Email, @Password, @StravaAccessCode)";

            await _db.SaveData(sql, user);                       
        }

        public async Task UpdateUser(User user)
        {
            string sql = @"UPDATE Users
                            SET Email = @Email, StravaAccessCode = @StravaAccessCode
                            WHERE UserID = @UserId";

            await _db.SaveData(sql, user);
        }
    }
}

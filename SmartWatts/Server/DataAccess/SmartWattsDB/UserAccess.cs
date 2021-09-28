using SmartWatts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<User> GetUser(int id)
        {

            var parameter = new { ID = id };

            string sql = @"SELECT * FROM user
                            WHERE UserID = @ID";

            var user = await _db.LoadData<User, dynamic>(sql, parameter);

            return user.FirstOrDefault();
        }
    }
}

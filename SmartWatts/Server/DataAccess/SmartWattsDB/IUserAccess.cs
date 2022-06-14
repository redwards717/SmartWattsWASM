using SmartWatts.Shared.DBModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public interface IUserAccess
    {
        Task<User> GetUserById(string id);
        Task<User> GetUser(string email, string password);
        Task<User> GetUserByEmail(string email);
        Task InsertUser(User user);
        Task UpdateUser(User user);
        Task<User> GetUserByStravaId(string id);
    }
}
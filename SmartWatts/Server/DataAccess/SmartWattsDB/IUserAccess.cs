using SmartWatts.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public interface IUserAccess
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUser(string email, string password);
        Task<User> GetUserByEmail(string email);
        Task InsertUser(User user);
    }
}
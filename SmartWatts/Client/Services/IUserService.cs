using SmartWatts.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWatts.Client.Services
{
    public interface IUserService
    {
        Task AddTokenToUser(string uri, User user);
        Task<User> GetUserById(string Id);
        Task<User> LoadUser(User user);
        Task RefreshUserToken(User user);
        Task RegisterUser(User user);
        Task UpdateUser(User user);
    }
}
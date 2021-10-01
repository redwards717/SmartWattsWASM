using SmartWatts.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWatts.Client.Services
{
    public interface IUserService
    {
        Task<User> GetUserById(string Id);
        Task<User> LoadUser(User user);
        Task RegisterUser(User user);
        Task UpdateUser(User user);
    }
}
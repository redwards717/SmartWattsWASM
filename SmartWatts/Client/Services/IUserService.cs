using SmartWatts.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWatts.Client.Services
{
    public interface IUserService
    {
        Task<List<User>> LoadAllUsers();
        Task<User> LoadUser(User user);
        Task RegisterUser(User user);
    }
}
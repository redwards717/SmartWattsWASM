using SmartWatts.Shared;
using System.Threading.Tasks;

namespace SmartWatts.Client.Services
{
    public interface IUserService
    {
        Task<User> LoadUser(string email, string password);
        Task RegisterUser(User user);
    }
}
using SmartWatts.Shared;
using System.Threading.Tasks;

namespace SmartWatts.Client.Services
{
    public interface IUserService
    {
        Task<User> LoadUser(int id);
    }
}
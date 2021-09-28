using SmartWatts.Shared;
using System.Threading.Tasks;

namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public interface IUserAccess
    {
        Task<User> GetUser(int id);
    }
}
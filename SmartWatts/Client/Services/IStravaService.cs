using SmartWatts.Shared;
using System.Threading.Tasks;

namespace SmartWatts.Client.Services
{
    public interface IStravaService
    {
        Task<List<Activity>> GetActivitiesForUser(User user, long? before = null, long? after = null, int? page = null, int? per_page = null);
    }
}
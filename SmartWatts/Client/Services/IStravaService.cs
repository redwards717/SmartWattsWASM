using SmartWatts.Shared;
using System.Threading.Tasks;

namespace SmartWatts.Client.Services
{
    public interface IStravaService
    {
        Task<List<Activity>> GetActivitiesForUser(long? before = null, long? after = null, int? page = null, int? per_page = null);
        Task<List<StravaDataStream>> GetDataStreamForActivity(Activity activity, string data);
    }
}
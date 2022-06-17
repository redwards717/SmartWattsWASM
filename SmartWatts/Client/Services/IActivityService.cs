
using SmartWatts.Shared.DBModels;

namespace SmartWatts.Client.Services
{
    public interface IActivityService
    {
        Task AddPowerDataToActivity(Activity activity, List<StravaDataStream> sds);
        Task<int> FindAndAddNewActivities(User user, int count);
        Task<List<Activity>> GetAllActivitiesByUser(User user);
        Task<int> SyncAllRidesFromStrava();
        Task<int> GetStravaRideCount();
        Task<List<Activity>> FindAndAddNewStravaActivities(int count, int? page = null);
    }
}
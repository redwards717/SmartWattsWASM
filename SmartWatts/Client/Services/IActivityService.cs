
using SmartWatts.Shared.DBModels;

namespace SmartWatts.Client.Services
{
    public interface IActivityService
    {
        Task AddPowerDataToActivity(Activity activity, List<StravaDataStream> sds);
        Task<int> FindAndAddNewActivities(User user, int count);
        Task<List<Activity>> GetAllActivitiesByUser(User user);
        Task NormalizePelotonData(DateTime before, int percentAdj, List<Activity> activities);
    }
}
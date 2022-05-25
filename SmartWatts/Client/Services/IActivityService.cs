
using SmartWatts.Shared.DBModels;

namespace SmartWatts.Client.Services
{
    public interface IActivityService
    {
        Task AddActivities(List<Activity> activities);
        Task AddPowerDataToActivity(Activity activity, StravaDataStream sds);
        Task<int> FindAndAddNewActivities(User user, int count);
        Task<List<Activity>> GetAllActivitiesByUser(User user);
    }
}
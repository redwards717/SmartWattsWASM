
namespace SmartWatts.Client.Services
{
    public interface IActivityService
    {
        Task AddActivities(List<Activity> activities);
        Task AddPowerDataToActivity(Activity activity, StravaDataStream sds);
        Task<List<Activity>> GetAllActivitiesByUser(User user);
    }
}
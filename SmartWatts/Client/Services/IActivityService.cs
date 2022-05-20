
namespace SmartWatts.Client.Services
{
    public interface IActivityService
    {
        Task AddActivities(List<Activity> activities);
        Task<List<Activity>> GetAllActivitiesByUser(User user);
    }
}
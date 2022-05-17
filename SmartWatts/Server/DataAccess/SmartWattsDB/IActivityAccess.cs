
namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public interface IActivityAccess
    {
        Task<List<StravaActivity>> GetActivitiesByUser(User user);
        Task InsertActivities(List<Activity> activities);
    }
}
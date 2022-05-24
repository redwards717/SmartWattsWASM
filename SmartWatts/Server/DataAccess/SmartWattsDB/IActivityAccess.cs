
namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public interface IActivityAccess
    {
        Task<List<Activity>> GetActivitiesByStravaUserID(string id);
        Task<Activity> GetActivityByStravaRideID(string id);
        Task InsertActivities(List<Activity> activities);
    }
}
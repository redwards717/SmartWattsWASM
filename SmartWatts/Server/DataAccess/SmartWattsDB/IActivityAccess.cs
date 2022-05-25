
using SmartWatts.Shared.DBModels;

namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public interface IActivityAccess
    {
        Task<List<Activity>> GetActivitiesByStravaUserID(string id);
        Task<Activity> GetActivityByStravaRideID(string id);
        Task<List<long>> GetRecentActivityIDsForUser(string id, string count);
        Task InsertActivities(List<Activity> activities);
    }
}
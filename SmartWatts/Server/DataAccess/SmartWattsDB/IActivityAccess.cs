
namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public interface IActivityAccess
    {
        Task<List<Activity>> GetActivitiesByUser(string id);
        Task InsertActivities(List<Activity> activities);
    }
}
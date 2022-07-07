
using SmartWatts.Shared.DBModels;

namespace SmartWatts.Client.Services
{
    public interface IActivityService
    {
        Task<List<Activity>> GetAllActivitiesByUser(User user);
        Task<(int, int)> SyncRidesFromStrava(ActivityParams activityParams, bool multiplePages = true);
        Task<int> GetStravaRideCount();
        Task<List<Activity>> FindAndAddNewStravaActivities(ActivityParams activityParams);
        void AttachViewingData(int daysBack);
        Task<(int, int)> InitialDataLoadForExistingUsers();
        void AttachViewingDataByYear(int year);
    }
}
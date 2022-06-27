namespace SmartWatts.Server.DataAccess.StravaApi
{
    public interface IStravaAccess
    {
        Task<IEnumerable<StravaActivity>> GetActivities(string token, long? before = null, long? after = null, int? page = null, int? per_page = null);
        Task<AthleteStats> GetAthleteStats(string token, string stravaUserId);
        Task<List<StravaDataStream>> GetDataStreamForActivity(Activity activity, User user, string v);
    }
}
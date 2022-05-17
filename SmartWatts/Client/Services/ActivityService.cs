using SmartWatts.Shared;

namespace SmartWatts.Client.Services
{
    public class ActivityService : IActivityService
    {
        private readonly HttpClient _http;

        public ActivityService(HttpClient http)
        {
            _http = http;
        }

        public async Task AddActivities(List<Activity> activities)
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync("api/Activity/Add", activities);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}

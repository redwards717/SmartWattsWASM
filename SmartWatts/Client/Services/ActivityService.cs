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

        public async Task<List<Activity>> GetAllActivitiesByUser(User user)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.BASE_URI + "api/Activity/GetAllByUser");

            request.Headers.Add("id", user.StravaUserID.ToString());

            using HttpResponseMessage response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var test = await response.Content.ReadFromJsonAsync<List<Activity>>();
            return test;
        }
    }
}

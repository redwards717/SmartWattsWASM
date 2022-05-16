using Newtonsoft.Json;
using System.Web;

namespace SmartWatts.Client.Services
{
    public class StravaService : IStravaService
    {
        private readonly HttpClient _http;

        public StravaService(HttpClient http)
        {
            _http = http;
        }

        public async Task GetActivitiesForUser(User user, long? before = null, long? after = null, int? page = null, int? per_page = null)
        {
            try
            {
                UriBuilder uriBuilder = new()
                {
                    Scheme = "https",
                    Host = "strava.com",
                    Path = "api/v3/athlete/activities"
                };

                var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
                paramValues.Add("before", before.ToString());
                paramValues.Add("after", after.ToString());
                paramValues.Add("page", page.ToString());
                paramValues.Add("per_page", per_page.ToString());

                using HttpRequestMessage request = new(new HttpMethod("GET"), "https://www.strava.com/api/v3/athlete/activities");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.StravaAccessToken);

                using HttpResponseMessage response = await _http.SendAsync(request);
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var activities = JsonConvert.DeserializeObject<List<StravaActivity>>(jsonString);
            }
            catch(Exception ex)
            {
                var test = ex;
            }
        }
    }
}

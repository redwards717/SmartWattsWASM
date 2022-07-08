global using Newtonsoft.Json;
global using System.Net.Http.Headers;
global using System.Web;

namespace SmartWatts.Server.DataAccess.StravaApi
{
    public class StravaAccess : IStravaAccess
    {
        private readonly HttpClient _http;

        public StravaAccess(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<StravaActivity>> GetActivities(string token, long? before = null, long? after = null, int? page = null, int? per_page = null)
        {
            UriBuilder uriBuilder = new("https://www.strava.com/api/v3/athlete/activities")
            {
                Port = -1
            };

            var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (before is not null)
            {
                paramValues.Add("before", before.ToString());
            }
            if (after is not null)
            {
                paramValues.Add("after", after.ToString());
            }
            if (page is not null)
            {
                paramValues.Add("page", page.ToString());
            }
            if (per_page is not null)
            {
                paramValues.Add("per_page", per_page.ToString());
            }
            uriBuilder.Query = paramValues.ToString();

            using HttpRequestMessage request = new(new HttpMethod("GET"), uriBuilder.ToString());
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using HttpResponseMessage response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<StravaActivity>>(jsonString);
        }

        public async Task<List<StravaDataStream>> GetDataStreamForActivity(Activity activity, User user, string data)
        {
            UriBuilder uriBuilder = new($"https://www.strava.com/api/v3/activities/{activity.StravaRideID}/streams")
            {
                Port = -1
            };

            var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
            paramValues.Add("keys", data); // other options [time,distance,latlng,altitude,velocity_smooth,heartrate,cadance,watts,temp,moving,grade_smooth]
            paramValues.Add("series_type", "time");

            uriBuilder.Query = paramValues.ToString();

            using HttpRequestMessage request = new(new HttpMethod("GET"), uriBuilder.ToString());
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.StravaAccessToken);

            using HttpResponseMessage response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            jsonString = jsonString.Replace("null", "0");
            return JsonConvert.DeserializeObject<List<StravaDataStream>>(jsonString);
        }

        public async Task<AthleteStats> GetAthleteStats(string token, string stravaUserId)
        {
            UriBuilder uriBuilder = new($"https://www.strava.com/api/v3/athletes/{stravaUserId}/stats")
            {
                Port = -1
            };

            using HttpRequestMessage request = new(new HttpMethod("GET"), uriBuilder.ToString());
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using HttpResponseMessage response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AthleteStats>(jsonString);
        }
    }
}

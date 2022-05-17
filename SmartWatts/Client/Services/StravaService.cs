using System.Diagnostics.Metrics;

namespace SmartWatts.Client.Services
{
    public class StravaService : IStravaService
    {
        private readonly HttpClient _http;

        public StravaService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Activity>> GetActivitiesForUser(User user, long? before = null, long? after = null, int? page = null, int? per_page = null)
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

                //using HttpRequestMessage request = new(new HttpMethod("GET"), "https://www.strava.com/api/v3/athlete/activities");
                using HttpRequestMessage request = new(new HttpMethod("GET"), uriBuilder.Uri.ToString());  // this isnt working for the url
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.StravaAccessToken);

                using HttpResponseMessage response = await _http.SendAsync(request);
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var sa = JsonConvert.DeserializeObject<List<StravaActivity>>(jsonString);
                return ConvertStravaActivity(sa);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task GetActivityStream(Activity activity, string data)
        {
            try
            {
                UriBuilder uriBuilder = new()
                {
                    Scheme = "https",
                    Host = "strava.com",
                    Path = $"api/v3/athlete/activities/{activity.StravaRideID}/streams"
                };

                var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
                paramValues.Add("keys", $"time,{data}"); // always grabs distance as well. other options [time,distance,latlng,altitude,velocity_smooth,heartrate,cadance,watts,temp,moving,grade_smooth]
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static List<Activity> ConvertStravaActivity(List<StravaActivity> stravaActivities)
        {
            List<Activity> activities = new();
            foreach(StravaActivity sa in stravaActivities)
            {
                activities.Add(new Activity()
                {
                    StravaRideID = sa.id,
                    StravaUserID = sa.athlete.id,
                    Name = sa.name,
                    Distance = sa.distance,
                    AvgSpeed = sa.average_speed,
                    MaxSpeed = sa.max_speed,
                    AvgCadence = sa.average_cadence,
                    AvgWatts = sa.average_watts,
                    Kilojoules = sa.kilojoules,
                    AvgHeartrate = sa.average_heartrate,
                    MaxHeartrate = sa.max_heartrate
                });
            }

            return activities;
        }
    }
}

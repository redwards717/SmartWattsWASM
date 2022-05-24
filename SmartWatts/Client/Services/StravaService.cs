using SmartWatts.Shared;
using System.Diagnostics.Metrics;

namespace SmartWatts.Client.Services
{
    public class StravaService : IStravaService
    {
        private readonly HttpClient _http;
        private readonly AppState _appState;

        public StravaService(HttpClient http, AppState appState)
        {
            _http = http;
            _appState = appState;
        }

        public async Task<List<Activity>> GetActivitiesForUser(long? before = null, long? after = null, int? page = null, int? per_page = null)
        {
            try
            {
                UriBuilder uriBuilder = new("https://www.strava.com/api/v3/athlete/activities");
                uriBuilder.Port = -1;

                var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
                if(before is not null)
                {
                    paramValues.Add("before", before.ToString());
                }
                if(after is not null)
                {
                    paramValues.Add("after", after.ToString());
                }
                if(page is not null)
                {
                    paramValues.Add("page", page.ToString());
                }
                if(per_page is not null)
                {
                    paramValues.Add("per_page", per_page.ToString());
                }

                uriBuilder.Query = paramValues.ToString();

                using HttpRequestMessage request = new(new HttpMethod("GET"), uriBuilder.ToString());
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _appState.LoggedInUser.StravaAccessToken);

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

        public async Task<List<StravaDataStream>> GetDataStreamForActivity(Activity activity, string data)
        {
            try
            {
                UriBuilder uriBuilder = new($"https://www.strava.com/api/v3/activities/{activity.StravaRideID}/streams");
                uriBuilder.Port = -1;

                var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
                paramValues.Add("keys", data); // other options [time,distance,latlng,altitude,velocity_smooth,heartrate,cadance,watts,temp,moving,grade_smooth]
                paramValues.Add("series_type", "time");

                uriBuilder.Query = paramValues.ToString();

                using HttpRequestMessage request = new(new HttpMethod("GET"), uriBuilder.ToString());
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _appState.LoggedInUser.StravaAccessToken);

                using HttpResponseMessage response = await _http.SendAsync(request);
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<StravaDataStream>>(jsonString);
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
                    MaxWatts = sa.max_watts,
                    WeightedAvgWatts = sa.weighted_average_watts,
                    Kilojoules = sa.kilojoules,
                    AvgHeartrate = sa.average_heartrate,
                    MaxHeartrate = sa.max_heartrate
                });
            }

            return activities;
        }
    }
}

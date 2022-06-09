using System.Net.NetworkInformation;

namespace SmartWatts.Client.Services
{
    public class ActivityService : IActivityService
    {
        private readonly HttpClient _http;
        private readonly AppState _appState;

        public ActivityService(HttpClient http, AppState appState)
        {
            _http = http;
            _appState = appState;
        }

        public async Task<int> FindAndAddNewActivities(User user, int count)
        {
            var stravaActivities = await GetActivitiesFromStrava(per_page: count);
            var existingStravaRideIDs = _appState.UsersActivities.Select(ua => ua.StravaRideID);

            var newActivities = stravaActivities.Where(sa => !existingStravaRideIDs.Contains(sa.StravaRideID)).ToList();

            await AddActivities(newActivities);

            int rideNo = 1;
            foreach (Activity activity in newActivities)
            {
                _appState.SetLoadingMsg($"Loading ride {activity.Name} - ( {rideNo} / {newActivities.Count} )");
                var data = await GetDataStreamForActivity(activity, "watts");
                await AddPowerDataToActivity(activity, data.Find(d => d.type == "watts"));
                rideNo++;
            }

            _appState.AddUsersActivities(newActivities);

            return newActivities?.Count ?? 0;
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

            var activities = await response.Content.ReadFromJsonAsync<List<Activity>>();

            _appState.SetUsersActivities(activities);

            return activities;
        }

        public async Task AddPowerDataToActivity(Activity activity, StravaDataStream sds)
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync($"api/Activity/{activity.StravaRideID}/AddPower", sds);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

            activity.PowerData = await response.Content.ReadFromJsonAsync<PowerData>();
        }

        public async Task NormalizePelotonData(DateTime before, int percentAdj, List<Activity> activities)
        {

            var activitiesToNormalize = activities.Where(a => (a.Name.Contains("Ride with") && a.Name.Contains("min"))
                                                                || (a.Name.Contains("Just Ride") && a.Name.Contains("min"))
                                                                || (a.Name.Contains("Scenic Ride") && a.Name.Contains("min"))).ToList();

            using HttpResponseMessage response = await _http.PutAsJsonAsync($"api/Activity/NormalizePower/{percentAdj}", activities);

            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

        }

        private async Task<List<Activity>> GetActivitiesFromStrava(long? before = null, long? after = null, int? page = null, int? per_page = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.BASE_URI + $"api/Strava/Activities/{_appState.LoggedInUser.UserId}");

            request.Headers.Add("befor", before.ToString());
            request.Headers.Add("after", after.ToString());
            request.Headers.Add("page", page.ToString());
            request.Headers.Add("per_page", per_page.ToString());

            using HttpResponseMessage response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

            return await response.Content.ReadFromJsonAsync<List<Activity>>();
        }

        private async Task<List<StravaDataStream>> GetDataStreamForActivity(Activity activity, string data)
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
                jsonString = jsonString.Replace("null", "0");
                return JsonConvert.DeserializeObject<List<StravaDataStream>>(jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

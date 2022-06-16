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
            var existingStravaRideIDs = _appState.LoggedInUser.Activities.Select(ua => ua.StravaRideID);

            var newActivities = stravaActivities.Where(sa => !existingStravaRideIDs.Contains(sa.StravaRideID)).ToList();

            await AddActivities(newActivities);

            int rideNo = 1;
            foreach (Activity activity in newActivities)
            {
                _appState.SetLoadingMsg($"Loading ride {activity.Name} - ( {rideNo} / {newActivities.Count} )");

                var data = await GetDataStreamForActivity(activity, "watts");
                await AddPowerDataToActivity(activity, data);
                AttachObjects(activity, _appState.LoggedInUser.Activities);
                _appState.AddUsersActivities(activity);

                rideNo++;
            }

            return newActivities?.Count ?? 0;
        }

        public async Task<int> SyncAllRidesFromStrava()
        {
            var count = await GetStravaRideCount();
            var extraMsg = count >= 100 ? "this may take a few minutes" : "";

            _appState.SetLoadingMsg($"Found {count} rides on Strava...{extraMsg}");

            int countLoaded = 0;
            
            for(int i = 1; i <= (count/100) + 1; i++)
            {
                countLoaded += await FindAndAddNewStravaActivities(100, i);

                _appState.SetLoadingMsg($"{countLoaded} / {count} rides loaded...");
            }

            return count;
        }

        public async Task<int> FindAndAddNewStravaActivities(int count, int? page = null)
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync($"api/Activity/FindAndAddNew/{count}/{page}", _appState.LoggedInUser);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var activities = await response.Content.ReadFromJsonAsync<List<Activity>>();

            _appState.AddUsersActivities(activities);

            return activities.Count;
        }

        public async Task<int> GetStravaRideCount()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.BASE_URI + "api/Activity/StravaRideCount");

            request.Headers.Add("stravaUserID", _appState.LoggedInUser.StravaUserID.ToString());
            request.Headers.Add("token", _appState.LoggedInUser.StravaAccessToken);

            using HttpResponseMessage response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var count = await response.Content.ReadAsStringAsync();
            return Int32.Parse(count);
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

            //AttachObjects(activities, activities); // if this gets slow consider only attaching history as far back as it required.  365 days + 45 maybe.

            return activities;
        }

        public async Task AddPowerDataToActivity(Activity activity, List<StravaDataStream> sds)
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync($"api/Activity/{activity.StravaRideID}/AddPower", sds);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

            activity.PowerData = await response.Content.ReadFromJsonAsync<PowerData>();
        }

        private async Task AddActivities(List<Activity> activities)
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync("api/Activity/Add", activities);
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

        private void AttachObjects(Activity activity, List<Activity> activitiesForComp)
        {
            activity.PowerHistory = PowerUtlities.GetPowerHistory(activity, activitiesForComp);
            activity.Intensity = PowerUtlities.GetRideIntensity(activity);
        }

        private void AttachObjects(List<Activity> activities, List<Activity> activitiesForComp)
        {
            foreach(Activity activity in activities)
            {
                AttachObjects(activity, activitiesForComp);
            }
        }
    }
}

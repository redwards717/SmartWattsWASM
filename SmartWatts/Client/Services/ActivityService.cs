using SmartWatts.Shared.DBModels;
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

        public async Task<int> SyncRidesFromStrava(ActivityParams activityParams, bool multiplePages = true)
        {
            int countLoaded = 0;
            bool cancel = !multiplePages;

            do
            {
                var activities = await FindAndAddNewStravaActivities(activityParams);

                if (activities is null || activities.Count == 0)
                {
                    activityParams.Page++;
                    string elipsis = new string('.', (int)activityParams.Page);
                    _appState.SetLoadingMsg($"Loading rides from Strava...{elipsis}");
                    continue;
                }
                else if (activities[0].Name == "CancToken")
                {
                    cancel = true;
                }
                else
                {
                    countLoaded += activities.Count;

                    _appState.SetLoadingMsg($"{countLoaded} rides loaded - up till {activities[0].Date:MMM} / {activities[0].Date.Year} done!...");
                }
                activityParams.Page++;
            } while (cancel == false);

            AttachObjects(365 + 45);

            return countLoaded;
        }

        public async Task<List<Activity>> FindAndAddNewStravaActivities(ActivityParams activityParams)
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync($"api/Activity/FindAndAddNew", activityParams);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var activities = await response.Content.ReadFromJsonAsync<List<Activity>>();

            if(activities.Count > 0 && activities[0].Name == "CancToken")
            {
                return activities;
            }

            _appState.AddUsersActivities(activities);

            return activities;
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

            return activities;
        }

        private void AttachObjects(int daysBack)
        {
            foreach (Activity activity in _appState.LoggedInUser.Activities.Where(a => a.Date <= DateTime.Now.AddDays(-daysBack)))
            {
                activity.PowerHistory = PowerUtlities.GetPowerHistory(activity, _appState.LoggedInUser.Activities);
                activity.Intensity = PowerUtlities.GetRideIntensity(activity);
            }
        }
    }
}

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

        public async Task<(int, int)> InitialDataLoadForExistingUsers()
        {
            _appState.LoaderOn("Scanning for new activities...");

            await GetAllActivitiesByUser(_appState.LoggedInUser);

            if(_appState.LoggedInUser.Activities.Count < 1)
            {
                return (0,0);
            }

            ActivityParams activityParams = new()
            {
                User = _appState.LoggedInUser,
                PerPage = 100,
                Page = 1,
                After = DateTime.Now.AddDays(-30).ToUnixSeconds()
            };

            var newData = await SyncRidesFromStrava(activityParams, false);

            AttachViewingData(400);

            return newData;
        }

        public async Task<(int, int)> SyncRidesFromStrava(ActivityParams activityParams, bool multiplePages = true)
        {
            int countLoaded = 0;
            int newFTP = 0;
            bool cancel = !multiplePages;

            do
            {
                var activities = await FindAndAddNewStravaActivities(activityParams);

                if (activities is null || activities.Count == 0)
                {
                    activityParams.Page++;
                    string elipsis = new string('.', (int)activityParams.Page);
                    _appState.SetLoadingMsg($"Loading rides from Strava..{elipsis}");
                    continue;
                }
                else if (activities[0].Name == "CancToken")
                {
                    cancel = true;
                }
                else
                {
                    countLoaded += activities.Count;

                    newFTP = await CheckForNewFTP(_appState.LoggedInUser);
                    _appState.SetLoadingMsg($"{countLoaded} rides loaded - up till {activities[0].Date:MMM} / {activities[0].Date.Year} done!...");
                    if(newFTP > 0)
                    {
                        _appState.SetLoadingMsg(Environment.NewLine + $"FTP updated from {_appState.LoggedInUser.FTP} to {newFTP}", false);
                        _appState.LoggedInUser.FTP = newFTP;
                    }
                }

                activityParams.Page++;
            } while (cancel == false);

            return (countLoaded, newFTP);
        }

        public async Task<List<Activity>> FindAndAddNewStravaActivities(ActivityParams activityParams)
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync("api/Activity/FindAndAddNew", activityParams);
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

            AttachViewingData(400);

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

        public void AttachViewingData(int daysBack)
        {
            foreach (Activity activity in _appState.LoggedInUser.Activities.Where(a => a.Date >= DateTime.Now.AddDays(-daysBack) && (a.PowerHistory is null || a.Intensity is null)))
            {
                activity.PowerHistory = PowerUtlities.GetPowerHistory(activity, _appState.LoggedInUser.Activities);
                activity.Intensity = PowerUtlities.GetRideIntensity(activity);
            }
        }

        public void AttachViewingDataByYear(int year)
        {
            var activities = _appState.LoggedInUser.Activities.Where(a => a.Date.Year == year
                                                                        && (a.Intensity is null || a.PowerHistory is null));

            foreach(Activity activity in activities)
            {
                activity.PowerHistory = PowerUtlities.GetPowerHistory(activity, _appState.LoggedInUser.Activities);
                activity.Intensity = PowerUtlities.GetRideIntensity(activity);
            }
        }

        public async Task ToggleIsRace(Activity activity)
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync($"api/Activity/SetRace", activity);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        private async Task<int> CheckForNewFTP(User user)
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync($"api/User/CheckForFtpChange", user);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var newFtp = Int32.Parse(await response.Content.ReadAsStringAsync());

            return newFtp;
        }
    }
}

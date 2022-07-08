using SmartWatts.Client.Services.Interfaces;
using SmartWatts.Shared.DBModels;

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
                jsonString = jsonString.Replace("null", "0");
                return JsonConvert.DeserializeObject<List<StravaDataStream>>(jsonString);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

using SmartWatts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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

        public async Task Authorize()
        {
            var response = await _http.GetAsync($"https://www.strava.com/oauth/authorize?client_id={Constants.STRAVA_CLIENT_ID}&redirect_uri={Constants.BASE_URI}&response_type=code&scope=activity:read");
        }
        public async Task LinkToStrava()
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync("api/Strava/LinkToStrava", _appState.LoggedInUser);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}

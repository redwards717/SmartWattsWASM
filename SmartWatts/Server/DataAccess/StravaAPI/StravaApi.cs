using SmartWatts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace SmartWatts.Server.DataAccess.StravaAPI
{
    public class StravaApi : IStravaApi
    {
        private readonly HttpClient _http;

        public StravaApi(HttpClient http)
        {
            _http = http;
        }
        public async Task AuthorizeStrava()
        {
            var response = await _http.GetAsync($"https://www.strava.com/oauth/authorize?client_id={Constants.STRAVA_CLIENT_ID}&redirect_uri={Constants.BASE_URI}&response_type=code&scope=activity:read");
            var result = await response.Content.ReadAsStringAsync();
        }
    }
}

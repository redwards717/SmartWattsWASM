using SmartWatts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace SmartWatts.Server.DataAccess.StravaAPI
{
    public class StravaApi : IStravaApi
    {
        private readonly HttpClient _http;

        public StravaApi(HttpClient http)
        {
            _http = http;
        }


        public async Task TokenExchange(string code)
        {
            UriBuilder uriBuilder = new()
            {
                Scheme = "https",
                Host = "strava.com",
                Path = "oauth/token"
            };

            var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
            paramValues.Add("client_id", Constants.STRAVA_CLIENT_ID);
            paramValues.Add("client_secret", Constants.STRAVA_CLIENT_SECRET);
            paramValues.Add("code", code);
            paramValues.Add("grant_type", "authorization_code");

            uriBuilder.Query = paramValues.ToString();
            // still doesnt work
            var response = await _http.PostAsync(uriBuilder.Uri, null);
        }
    }
}

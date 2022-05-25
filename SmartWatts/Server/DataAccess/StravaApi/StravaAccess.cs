using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;

namespace SmartWatts.Server.DataAccess.StravaApi
{
    public class StravaAccess : IStravaAccess
    {
        private readonly HttpClient _http;

        public StravaAccess(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<StravaActivity>> GetActivities(string token, long? before = null, long? after = null, int? page = null, int? per_page = null)
        {
            UriBuilder uriBuilder = new("https://www.strava.com/api/v3/athlete/activities");
            uriBuilder.Port = -1;

            var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (before is not null)
            {
                paramValues.Add("before", before.ToString());
            }
            if (after is not null)
            {
                paramValues.Add("after", after.ToString());
            }
            if (page is not null)
            {
                paramValues.Add("page", page.ToString());
            }
            if (per_page is not null)
            {
                paramValues.Add("per_page", per_page.ToString());
            }
            uriBuilder.Query = paramValues.ToString();

            using HttpRequestMessage request = new(new HttpMethod("GET"), uriBuilder.ToString());
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using HttpResponseMessage response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<StravaActivity>>(jsonString);
        }
    }
}

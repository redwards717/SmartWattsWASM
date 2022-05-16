using SmartWatts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace SmartWatts.Server.DataAccess.StravaAPI
{
    //public class StravaApi : IStravaApi
    //{
    //    private readonly HttpClient _http;

    //    public StravaApi(HttpClient http)
    //    {
    //        _http = http;
    //    }


    //    public async Task TokenExchange(string code)
    //    {
    //        UriBuilder uriBuilder = new()
    //        {
    //            Scheme = "https",
    //            Host = "strava.com",
    //            Path = "oauth/token"
    //        };

    //        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    //        var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
    //        paramValues.Add("client_id", Constants.STRAVA_CLIENT_ID);
    //        paramValues.Add("client_secret", Constants.STRAVA_CLIENT_SECRET);
    //        paramValues.Add("code", code);
    //        paramValues.Add("grant_type", "authorization_code");

    //        uriBuilder.Query = paramValues.ToString();

    //        // still doesnt work
    //        var response = await _http.PostAsync(uriBuilder.Uri, null);
    //    }

    //    public async Task<StravaUser> TokenExchange(string code)
    //    {
    //        using HttpRequestMessage request = new(new HttpMethod("POST"), "https://www.strava.com/api/v3/oauth/token");
    //        List<string> contentList = new()
    //        {
    //            $"client_id={Constants.STRAVA_CLIENT_ID}",
    //            $"client_secret={Constants.STRAVA_CLIENT_SECRET}",
    //            $"code={code}",
    //            "grant_type=authorization_code"
    //        };
    //        request.Content = new StringContent(string.Join("&", contentList));
    //        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

    //        using HttpResponseMessage response = await _http.SendAsync(request);
    //        if (response.IsSuccessStatusCode == false)
    //        {
    //            throw new Exception(response.ReasonPhrase);
    //        }
    //        return await response.Content.ReadFromJsonAsync<StravaUser>();
    //    }


    //}
}

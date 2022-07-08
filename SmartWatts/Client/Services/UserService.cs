using SmartWatts.Client.Services.Interfaces;

namespace SmartWatts.Client.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;

        public UserService(HttpClient http)
        {
            _http = http;
        }

        public Task<User> GetUserById(string Id)
        {
            return _http.GetFromJsonAsync<User>($"api/User/{Id}");
        }

        public async Task<User> LoadUser(User user)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.BASE_URI + "api/User/Login");

            request.Headers.Add("email", user.Email);
            request.Headers.Add("password", user.Password);

            using HttpResponseMessage response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

            return await response.Content.ReadFromJsonAsync<User>();
        }

        public async Task RegisterUser(User user)
        {
            using HttpResponseMessage response = await _http.PostAsJsonAsync("api/User/Register", user);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task UpdateUser(User user)
        {
            using HttpResponseMessage response = await _http.PutAsJsonAsync("api/User/Update", user);
            if(response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task AddTokenToUser(string uri, User user)
        {
            string code = uri.Split("code=")[1].Split("&")[0];

            var stravaUser = await GetStravaUserToken(code);

            user.StravaAccessToken = stravaUser.access_token;
            user.TokenExpiration = DateTimeUtilities.UnixToDateTime(stravaUser.expires_at);
            user.RefreshToken = stravaUser.refresh_token;
            user.StravaUserID = stravaUser.athlete.id;

            using HttpResponseMessage response = await _http.PutAsJsonAsync("api/User/Update", user);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task RefreshUserToken(User user)
        {
            var stravaUser = await GetRefreshToken(user);

            user.StravaAccessToken = stravaUser.access_token;
            user.TokenExpiration = DateTimeUtilities.UnixToDateTime(stravaUser.expires_at);
            user.RefreshToken = stravaUser.refresh_token;

            using HttpResponseMessage response = await _http.PutAsJsonAsync("api/User/Update", user);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        private async Task<StravaModels> GetStravaUserToken(string code)
        {
            using HttpRequestMessage request = new(new HttpMethod("POST"), "https://www.strava.com/api/v3/oauth/token");
            List<string> contentList = new()
            {
                $"client_id={Constants.STRAVA_CLIENT_ID}",
                $"client_secret={Constants.STRAVA_CLIENT_SECRET}",
                $"code={code}",
                "grant_type=authorization_code"
            };
            request.Content = new StringContent(string.Join("&", contentList));
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            using HttpResponseMessage response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }
            return await response.Content.ReadFromJsonAsync<StravaModels>();
        }

        private async Task<StravaModels> GetRefreshToken(User user)
        {
            using HttpRequestMessage request = new(new HttpMethod("POST"), "https://www.strava.com/api/v3/oauth/token");
            List<string> contentList = new()
            {
                $"client_id={Constants.STRAVA_CLIENT_ID}",
                $"client_secret={Constants.STRAVA_CLIENT_SECRET}",
                $"refresh_token={user.RefreshToken}",
                "grant_type=refresh_token"
            };
            request.Content = new StringContent(string.Join("&", contentList));
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            using HttpResponseMessage response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }

            return await response.Content.ReadFromJsonAsync<StravaModels>();
        }
    }
}

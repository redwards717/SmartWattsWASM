using SmartWatts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SmartWatts.Client.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;

        public UserService(HttpClient http)
        {
            _http = http;
        }
        public async Task<User> LoadUser(string email, string pw)
        {
            var user = await _http.GetFromJsonAsync<User>("api/User");
            return user;
        }

        public async Task RegisterUser(User user)
        {
            using (HttpResponseMessage response = await _http.PostAsJsonAsync("api/User/Register", user))
            {
                if(response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}

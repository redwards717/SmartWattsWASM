using Microsoft.AspNetCore.WebUtilities;
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

        public async Task<User> GetUserById(string Id)
        {
            return await _http.GetFromJsonAsync<User>($"api/User/{Id}");
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

        public async Task AddCodeToUser(string uri)
        {
            //write code to parse uri for stravatoken;
            using HttpResponseMessage reponse = await _http.GetAsync($"api/User/AddCode/{uri}");
        }
    }
}

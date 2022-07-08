using SmartWatts.Client.Services.Interfaces;
using SmartWatts.Shared.DBModels;

namespace SmartWatts.Client.Services
{
    public class FTPHistoryService : IFTPHistoryService
    {
        private readonly HttpClient _http;
        private readonly AppState _appState;

        public FTPHistoryService(HttpClient http, AppState appState)
        {
            _http = http;
            _appState = appState;
        }

        public async Task<List<FTPHistory>> GetFTPHistoriesByUser()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.BASE_URI + "api/FTPHistory/Get");

            request.Headers.Add("id", _appState.LoggedInUser.StravaUserID.ToString());

            using HttpResponseMessage response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            return await response.Content.ReadFromJsonAsync<List<FTPHistory>>();
        }

        public async Task InsertFTPHistory(FTPHistory fTPHistory)
{
            using HttpResponseMessage response = await _http.PostAsJsonAsync("api/FTPHistory/Add", fTPHistory);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}

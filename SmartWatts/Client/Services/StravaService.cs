using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartWatts.Client.Services
{
    public class StravaService
    {
        private readonly HttpClient _http;

        public StravaService(HttpClient http)
        {
            _http = http;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SmartWatts.Server.DataAccess.StravaAPI;
using SmartWatts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StravaController : ControllerBase
    {
        private readonly IStravaApi _stravaApi;

        public StravaController(IStravaApi stravaApi)
        {
            _stravaApi = stravaApi;
        }

        [HttpGet]
        [Route("TokenExchange/{code}")]
        public async Task<IActionResult> TokenExchange(string code)
        {
            var stravaUser = await _stravaApi.TokenExchange(code);
            return Ok(stravaUser);
        }


    }
}

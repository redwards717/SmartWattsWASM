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

        [HttpPost]
        [Route("LinkToStrava")]
        public async Task<IActionResult> LinkToStrava(User user)
        {
            await _stravaApi.AuthorizeStrava();
            return Ok();
        }


    }
}

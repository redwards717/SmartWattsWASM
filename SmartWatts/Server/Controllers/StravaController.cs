using Microsoft.AspNetCore.Mvc;
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
        public StravaController()
        {

        }

        [HttpPost]
        [Route("LinkToStrava")]
        public async Task<IActionResult> LinkToStrava(User user)
        {
            return Ok();
        }


    }
}

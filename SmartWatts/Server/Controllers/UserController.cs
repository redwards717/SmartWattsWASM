using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWatts.Server.DataAccess.SmartWattsDB;
using SmartWatts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserAccess _user;

        public UserController(IUserAccess user)
        {
            _user = user;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(string email, string password)
        {
            var user = _user.GetUser(email, password);
            if(user is null)
            {
                return NotFound("Incorrect login information");
            }

            return Ok(user);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> PostUser(User user)
        {
            var existingUser = await _user.GetUserByEmail(user.Email);
            if(existingUser is null)
            {
                await _user.InsertUser(user);
                return Ok();
            }

            return BadRequest();
        }

    }
}

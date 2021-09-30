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
        [Route("All")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _user.GetAllUsers();
            return Ok(users);
        }

        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> GetUser([FromHeader] string email, [FromHeader] string password)
        {
            var matchedUser = await _user.GetUser(email, password);
            if(matchedUser is null)
            {
                return NotFound("Incorrect login information");
            }

            return Ok(matchedUser);
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

            return BadRequest("That Email address is already registered");
        }

    }
}

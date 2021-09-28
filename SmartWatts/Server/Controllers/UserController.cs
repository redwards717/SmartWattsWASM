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
        public ActionResult<User> GetUser(int id)
        {
            var user = _user.GetUser(id);
            return Ok(user);
        }
    }
}

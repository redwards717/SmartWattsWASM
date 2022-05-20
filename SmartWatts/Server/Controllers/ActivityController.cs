﻿using SmartWatts.Shared;

namespace SmartWatts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController: ControllerBase
    {
        private readonly IActivityAccess _activity;

        public ActivityController(IActivityAccess activity)
        {
            _activity = activity;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> PostActivities(List<Activity> activities)
        {
            await _activity.InsertActivities(activities);
            return Ok();
        }

        [HttpGet]
        [Route("GetAllByUser")]
        public async Task<IActionResult> GetAllActivitiesByUser([FromHeader] string id)
        {
            var activities = await _activity.GetActivitiesByUser(id);
            return Ok(activities);
        }
    }
}

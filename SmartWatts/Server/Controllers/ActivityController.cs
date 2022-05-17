using SmartWatts.Shared;

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
    }
}

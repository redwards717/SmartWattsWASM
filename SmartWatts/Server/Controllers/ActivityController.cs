namespace SmartWatts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityAccess _activity;
        private readonly IPowerDataAccess _powerDataAccess;

        public ActivityController(IActivityAccess activity, IPowerDataAccess powerDataAccess)
        {
            _activity = activity;
            _powerDataAccess = powerDataAccess;
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
            var activities = await _activity.GetActivitiesByStravaUserID(id);
            return Ok(activities);
        }

        [HttpPost]
        [Route("{Id}/AddPower")]
        public async Task<IActionResult> AddPowerDataToActivity(string id, [FromBody]StravaDataStream sds)
        {
            var powerData = PowerUtilities.CalculatePowerFromDataStream(sds);
            powerData.StravaRideID = (await _activity.GetActivityByStravaRideID(id)).StravaRideID;
            await _powerDataAccess.InsertPowerData(powerData);
            return Ok();
        }
    }
}

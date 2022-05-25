using SmartWatts.Shared.DBModels;

namespace SmartWatts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityAccess _activityAccess;
        private readonly IPowerDataAccess _powerDataAccess;
        private readonly IStravaAccess _stravaAccess;
        private readonly IUserAccess _userAccess;

        public ActivityController(IActivityAccess activityAccess, IPowerDataAccess powerDataAccess, IStravaAccess stravaAccess, IUserAccess userAccess)
        {
            _activityAccess = activityAccess;
            _powerDataAccess = powerDataAccess;
            _stravaAccess = stravaAccess;
            _userAccess = userAccess;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> PostActivities(List<Activity> activities)
        {
            await _activityAccess.InsertActivities(activities);
            return Ok();
        }

        [HttpGet]
        [Route("GetAllByUser")]
        public async Task<IActionResult> GetAllActivitiesByUser([FromHeader] string id)
        {
            var activities = await _activityAccess.GetActivitiesByStravaUserID(id);
            return Ok(activities);
        }

        //[HttpGet]
        //[Route("FindNew")]
        //public async Task<IActionResult> FindNewActivities([FromHeader] string userid, [FromHeader] string count)
        //{
        //    var user = await _userAccess.GetUserById(userid);
        //    var activityIds = await _activityAccess.GetRecentActivityIDsForUser(user.StravaUserID.ToString(), count);

        //    var stravaActivities = await _stravaAccess.GetActivities(user.StravaAccessToken, per_page: 30);

        //    var newActivities = stravaActivities.Where(sa => !activityIds.Contains(sa.id));

        //    return Ok(newActivities);
        //}

        [HttpPost]
        [Route("{id}/AddPower")]
        public async Task<IActionResult> AddPowerDataToActivity(string id, [FromBody]StravaDataStream sds)
        {
            var powerData = PowerUtilities.CalculatePowerFromDataStream(sds);
            powerData.StravaRideID = (await _activityAccess.GetActivityByStravaRideID(id)).StravaRideID;
            await _powerDataAccess.InsertPowerData(powerData);
            return Ok();
        }
    }
}

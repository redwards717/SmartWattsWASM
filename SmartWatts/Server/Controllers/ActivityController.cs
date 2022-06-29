using SmartWatts.Client.Utilities;
using SmartWatts.Shared.APIParams;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
            var powerData = await _powerDataAccess.GetAllPowerDataByUser(id);

            foreach (Activity activity in activities)
            {
                activity.PowerData = powerData.Find(pd => pd.StravaRideID == activity.StravaRideID);
                activity.PowerData.PowerPoints = JsonSerializer.Deserialize<Dictionary<int, int>>(activity.PowerData.JsonPowerPoints);
                activity.PowerData.SustainedEfforts = JsonSerializer.Deserialize<Dictionary<int, int>>(activity.PowerData.JsonSustainedEfforts);
            }
            return Ok(activities);
        }

        //[HttpPost]
        //[Route("{id}/AddPower")]
        //public async Task<IActionResult> AddStreamAsPowerData(string id, [FromBody] List<StravaDataStream> sdss)
        //{
        //    var activity = await _activityAccess.GetActivityByStravaRideID(id);
        //    var user = await _userAccess.GetUserByStravaId(activity.StravaUserID.ToString());

        //    PowerData powerData = PowerUtilities.CalculatePowerFromDataStream(sdss, user.FTP);

        //    powerData.StravaRideID = activity.StravaRideID;
        //    powerData.FTPAtTimeOfRide = user.FTP;

        //    await _powerDataAccess.InsertPowerData(powerData);
        //    return Ok(powerData);
        //}

        [HttpPost]
        [Route("FindAndAddNew")]
        public async Task<IActionResult> FindAndAddNew(ActivityParams activityParams)
        {
            var existingIDs = activityParams.User.Activities.Select(a => a.StravaRideID);
            var allActivities = await _stravaAccess.GetActivities(activityParams.User.StravaAccessToken, per_page: activityParams.PerPage, page: activityParams.Page, after: activityParams.After);

            var stravaRides = allActivities.Where(sa => sa.device_watts && !existingIDs.Contains(sa.id));

            if (allActivities is null || allActivities.Count() == 0)
            {
                List<Activity> cancAct = new() { new Activity { Name = "CancToken" } };

                return Ok(cancAct);
            }

            var activities = ConverstionUtilities.ConvertStravaActivity(stravaRides);
            List<PowerData> newPowerData = new();

            foreach(Activity activity in activities)
            {
                var stravaDataStreams = await _stravaAccess.GetDataStreamForActivity(activity, activityParams.User, "watts");
                if (activity.IsPeloton && DateTime.Compare(activity.Date, new DateTime(2022, 3, 18)) < 0)
                {
                    PelotonUtilities.AddMissingDataPointsWithCorrection(stravaDataStreams, -30);
                    PelotonUtilities.NormalizePowerMetaData(activity, -30);
                }
                else if(activity.IsPeloton)
                {
                    PelotonUtilities.AddMissingDataPoints(stravaDataStreams);
                }
                PowerData powerData = PowerUtilities.CalculatePowerFromDataStream(stravaDataStreams, activityParams.User.FTP);

                powerData.StravaRideID = activity.StravaRideID;
                powerData.FTPAtTimeOfRide = activityParams.User.FTP;
                powerData.StravaUserID = activity.StravaUserID;

                newPowerData.Add(powerData);

                activity.PowerData = powerData;
            }

            await _powerDataAccess.InsertPowerData(newPowerData);
            await _activityAccess.InsertActivities(activities);

            return Ok(activities);
        }

        [HttpGet]
        [Route("StravaRideCount")]
        public async Task<IActionResult> GetStravaRideCount([FromHeader] string token, [FromHeader]string stravaUserID)
        {
            var athleteStats = await _stravaAccess.GetAthleteStats(token, stravaUserID);
            return Ok(athleteStats.all_ride_totals.count);
        }
    }
}

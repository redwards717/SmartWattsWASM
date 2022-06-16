using SmartWatts.Client.Utilities;
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
            foreach (Activity activity in activities)
            {
                activity.PowerData = await _powerDataAccess.GetPowerDataForActivity(activity);
                activity.PowerData.PowerPoints = JsonSerializer.Deserialize<Dictionary<int, int>>(activity.PowerData.JsonPowerPoints);
                activity.PowerData.SustainedEfforts = JsonSerializer.Deserialize<Dictionary<int, int>>(activity.PowerData.JsonSustainedEfforts);

                // only attach extra data for year and a bit.  will grab more if user views historical data
                if(DateTime.Compare(activity.Date, DateTime.Now.AddDays(-365 + 50)) > 0)
                {
                    activity.PowerHistory = PowerUtilities.GetPowerHistory(activity, activities);
                    activity.Intensity = PowerUtilities.GetRideIntensity(activity);
                }
            }
            return Ok(activities);
        }

        [HttpPost]
        [Route("{id}/AddPower")]
        public async Task<IActionResult> AddStreamAsPowerData(string id, [FromBody] List<StravaDataStream> sdss)
        {
            var activity = await _activityAccess.GetActivityByStravaRideID(id);
            var user = await _userAccess.GetUserByStravaId(activity.StravaUserID.ToString());

            PowerData powerData = PowerUtilities.CalculatePowerFromDataStream(sdss, user.FTP);

            powerData.StravaRideID = activity.StravaRideID;
            powerData.FTPAtTimeOfRide = user.FTP;

            await _powerDataAccess.InsertPowerData(powerData);
            return Ok(powerData);
        }

        [HttpPost]
        [Route("FindAndAddNew/{count}/{page}")]
        public async Task<IActionResult> FindAndAddNew(int count, int? page, User user)
        {
            int? after = page > 0 ? 1 : null;
            var existingIDs = user.Activities.Select(a => a.StravaRideID);
            var stravaActivities = (await _stravaAccess.GetActivities(user.StravaAccessToken, per_page:count, page:page, after:after))
                .Where(sa => sa.device_watts && !existingIDs.Contains(sa.id));

            var activities = ConverstionUtilities.ConvertStravaActivity(stravaActivities);
            List<PowerData> newPowerData = new();

            foreach(Activity activity in activities)
            {
                var stravaDataStreams = await _stravaAccess.GetDataStreamForActivity(activity, user, "watts");
                if (activity.IsPeloton && DateTime.Compare(activity.Date, new DateTime(2022, 3, 18)) < 0)
                {
                    PelotonUtilities.AddMissingDataPointsWithCorrection(stravaDataStreams, -30);
                    PelotonUtilities.NormalizePowerMetaData(activity, -30);
                }
                else if(activity.IsPeloton)
                {
                    PelotonUtilities.AddMissingDataPoints(stravaDataStreams);
                }
                PowerData powerData = PowerUtilities.CalculatePowerFromDataStream(stravaDataStreams, user.FTP);

                powerData.StravaRideID = activity.StravaRideID;
                powerData.FTPAtTimeOfRide = user.FTP;

                newPowerData.Add(powerData);

                activity.PowerData = powerData;
                if (DateTime.Compare(activity.Date, DateTime.Now.AddDays(-365 + 50)) > 0)
                {
                    activity.PowerHistory = PowerUtilities.GetPowerHistory(activity, activities);
                    activity.Intensity = PowerUtilities.GetRideIntensity(activity);
                }
                user.Activities.Add(activity);
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

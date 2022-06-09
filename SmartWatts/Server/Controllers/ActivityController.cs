﻿using SmartWatts.Shared.DBModels;

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
            }
            return Ok(activities);
        }

        [HttpPost]
        [Route("{id}/AddPower")]
        public async Task<IActionResult> AddStreamAsPowerData(string id, [FromBody] StravaDataStream sds)
        {
            PowerData powerData = PowerUtilities.CalculatePowerFromDataStream(sds);
            powerData.StravaRideID = (await _activityAccess.GetActivityByStravaRideID(id)).StravaRideID;
            await _powerDataAccess.InsertPowerData(powerData);
            return Ok(powerData);
        }

        [HttpPut]
        [Route("NormalizePower/{percentadj}")]
        public async Task<IActionResult> NormalizePower(string percentAdj, [FromBody] List<Activity> activities)
        {
            double multiplier = (double)(Int32.Parse(percentAdj) + 100) / 100;
            foreach(Activity activity in activities)
            {
                activity.Kilojoules *= multiplier;
                activity.WeightedAvgWatts *= multiplier;
                activity.AvgWatts *= multiplier;
                activity.MaxWatts *= multiplier;

                foreach(KeyValuePair<int,int> pp in activity.PowerData.PowerPoints)
                {
                    activity.PowerData.PowerPoints[pp.Key] = (int)(pp.Value * multiplier);
                }

                activity.PowerData.JsonPowerPoints = JsonSerializer.Serialize(activity.PowerData.PowerPoints);
            }

            var allPowerData = activities.ConvertAll(a => a.PowerData);

            await _activityAccess.UpdatePower(activities);
            await _powerDataAccess.UpdatePowerData(allPowerData);

            return Ok();
        }
    }
}

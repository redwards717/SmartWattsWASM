using SmartWatts.Server.DataAccess.SmartWattsDB.Interfaces;

namespace SmartWatts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StravaController : ControllerBase
    {
        private readonly IUserAccess _userAccess;
        private readonly IStravaAccess _stravaAcess;

        public StravaController(IUserAccess userAccess, IStravaAccess stravaAcess)
        {
            _userAccess = userAccess;
            _stravaAcess = stravaAcess;
        }

        [HttpGet]
        [Route("Activities/{id}")]
        public async Task<IActionResult> GetActivitiesFromStrava(string id, [FromHeader] long? before = null, [FromHeader] long? after = null, [FromHeader] int? page = null, [FromHeader] int? per_page = null)
        {
            var user = await _userAccess.GetUserById(id);
            var stravaActivities = await _stravaAcess.GetActivities(user.StravaAccessToken, before, after, page, per_page);
            stravaActivities = stravaActivities.Where(sa => sa.device_watts);
            var activities = ConvertStravaActivity(stravaActivities);

            return Ok(activities);
        }

        private static List<Activity> ConvertStravaActivity(IEnumerable<StravaActivity> stravaActivities)
        {
            List<Activity> activities = new();
            foreach (StravaActivity sa in stravaActivities)
            {
                activities.Add(new Activity()
                {
                    StravaRideID = sa.id,
                    StravaUserID = sa.athlete.id,
                    Name = sa.name,
                    Date = sa.start_date_local,
                    Type = sa.type,
                    IsRace = sa.workout_type == 11,
                    IsPeloton = (sa.name.Contains("Ride with") && sa.name.Contains(" min "))
                            || (sa.name.Contains("Just Ride") && sa.name.Contains(" min "))
                            || (sa.name.Contains("Scenic Ride") && sa.name.Contains(" min ")),
                    HasWatts = sa.device_watts,
                    MovingTime = sa.moving_time,
                    Distance = sa.distance,
                    AvgSpeed = sa.average_speed,
                    MaxSpeed = sa.max_speed,
                    AvgCadence = sa.average_cadence,
                    AvgWatts = sa.average_watts,
                    MaxWatts = sa.max_watts,
                    WeightedAvgWatts = sa.weighted_average_watts,
                    Kilojoules = sa.kilojoules,
                    AvgHeartrate = sa.average_heartrate,
                    MaxHeartrate = sa.max_heartrate
                });
            }

            return activities;
        }
    }
}

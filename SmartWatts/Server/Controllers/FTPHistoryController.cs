using SmartWatts.Server.DataAccess.SmartWattsDB.Interfaces;

namespace SmartWatts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FTPHistoryController : ControllerBase
    {
        private readonly IFTPHistoryAccess _fTPHistory;

        public FTPHistoryController(IFTPHistoryAccess fTPHistory)
        {
            _fTPHistory = fTPHistory;
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> GetFTPHistoryById([FromHeader] string id)
        {
            var ftpHistory = await _fTPHistory.GetFTPHistoryByStravaUserID(id);
            return Ok(ftpHistory);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> InsertFTPHistory(FTPHistory fTPHistory)
        {
            await _fTPHistory.InsertFTPHistory(fTPHistory);
            return Ok();
        }
    }
}

using SmartWatts.Shared;
using System.Threading.Tasks;

namespace SmartWatts.Client.Services
{
    public interface IStravaService
    {
        Task Authorize();
        Task LinkToStrava();
    }
}
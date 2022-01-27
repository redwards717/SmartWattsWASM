using System.Threading.Tasks;

namespace SmartWatts.Server.DataAccess.StravaAPI
{
    public interface IStravaApi
    {
        Task TokenExchange(string code);
    }
}
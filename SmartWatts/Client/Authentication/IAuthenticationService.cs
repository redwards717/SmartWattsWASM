using SmartWatts.Client.Models;
using System.Threading.Tasks;

namespace SmartWatts.Client.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication);
        Task Logout();
    }
}
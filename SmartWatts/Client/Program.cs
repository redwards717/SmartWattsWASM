#pragma warning disable AsyncFixer01 // Unnecessary async/await usage
#pragma warning disable RCS1102 // Make class static.

namespace SmartWatts.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddAuthorizationCore();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredToast();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IStravaService, StravaService>();
            builder.Services.AddScoped<IActivityService, ActivityService>();

            builder.Services.AddSingleton<AppState>();

            await builder.Build().RunAsync();
        }
    }
}

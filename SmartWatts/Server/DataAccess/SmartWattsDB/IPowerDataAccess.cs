using SmartWatts.Shared.DBModels;

namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public interface IPowerDataAccess
    {
        Task<PowerData> GetPowerDataForActivity(Activity activity);
        Task InsertPowerData(PowerData powerData);
        Task UpdatePowerData(List<PowerData> powerData);
    }
}
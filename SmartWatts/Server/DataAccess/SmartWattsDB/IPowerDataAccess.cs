using SmartWatts.Shared.DBModels;

namespace SmartWatts.Server.DataAccess.SmartWattsDB
{
    public interface IPowerDataAccess
    {
        Task<List<PowerData>> GetAllPowerDataByUser(string id);
        Task<PowerData> GetPowerDataForActivity(Activity activity);
        //Task InsertPowerData(PowerData powerData);
        Task InsertPowerData(List<PowerData> powerData);
        //Task UpdatePowerData(List<PowerData> powerData);
    }
}
namespace SmartWatts.Client.Services.Interfaces
{
    public interface IFTPHistoryService
    {
        Task<List<FTPHistory>> GetFTPHistoriesByUser();
        Task InsertFTPHistory(FTPHistory fTPHistory);
    }
}
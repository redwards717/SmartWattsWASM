namespace SmartWatts.Client.Services
{
    public interface IFTPHistoryService
    {
        Task<List<FTPHistory>> GetFTPHistoriesByUser();
        Task InsertFTPHistory(FTPHistory fTPHistory);
    }
}
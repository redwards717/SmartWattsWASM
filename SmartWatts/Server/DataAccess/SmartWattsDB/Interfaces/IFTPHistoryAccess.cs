namespace SmartWatts.Server.DataAccess.SmartWattsDB.Interfaces
{
    public interface IFTPHistoryAccess
    {
        Task<List<FTPHistory>> GetFTPHistoryByStravaUserID(string id);
        Task InsertFTPHistory(FTPHistory fTPHistory);
    }
}
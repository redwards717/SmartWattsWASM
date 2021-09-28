using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWatts.Server.DataAccess
{
    public interface ISqlDataAccess
    {
        Task<List<T>> LoadData<T, U>(string sql, U parameters);
        Task SaveData<T>(string sql, T parameters);
    }
}
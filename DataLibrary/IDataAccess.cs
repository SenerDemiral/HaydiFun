using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public interface IDataAccess
    {
        Task<T> LoadRecAsync<T, U>(string sql, U parameters);
        Task<IEnumerable<T>> LoadDataAsync<T, U>(string sql, U parameters);
        IEnumerable<T> LoadData<T, U>(string sql, U parameters);

        T StoreProc<T, U>(string storeProc, U parameters);
        Task<T> StoreProcAsync<T, U>(string storeProc, U parameters);
        //Task<dynamic> StoreProc2<T>(string storeProc, T parameters);
        Task<bool> SaveData<T>(string sql, T parameters);
        int GetTablePK(string tblName);
    }
}

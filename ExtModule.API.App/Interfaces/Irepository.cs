using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtModule.API.Application.Interfaces
{
    public interface IRepository
    {
        Task<DataTable> GetDataTableByStoredProcedureAsync(string spName, Hashtable paramList, string compid);
        Task<List<DataTable>> GetMultipleResultSetsBySPAsync(string spName, Hashtable paramList, string compid);
        Task<string> GetScalarByStoredProcedureAsync(string spName, Hashtable paramList, string compid);
        Task<DataTable> GetMasterDataAsync(int iMasterTypeId, string[] columnList, string condition, string compId, string[] orderColoumns);
        Task<DataTable> GetMasterData_MAsync(int iMasterTypeId, string[] columnList, string condition, string compId, string[] orderColoumns);     
        Task<string> InsertToTableAsync(string sTableName, Hashtable paramList, string compId);
        Task<string> BulkInsertToTableAsync(string tableName, List<Hashtable> paramList , string compId);
        Task<Hashtable> CreateAndBulkInsertToTableAsync(string tableName, List<Hashtable> lstParams, Hashtable coloumns, string compId);
        Task<string> DeleteRowFromTableAsync(string tableName, string condition, string compId);
        Task<string> UpdateTableAsync(string tableName, Hashtable paramList, string condition, string compId);           
        Task<DataTable> GetDataTableByViewAsync(string viewName, string[] columnList, string condition, string[] orderColoumns, string compId);
        Task<string> GetScalarByFuncAsync(string functionName,  string compId);
      
    }
}

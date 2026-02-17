using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtModule.API.Core;

namespace ExtModule.API.Application.Interfaces
{
    public interface IRepository
    {
        Task<DataTable> GetDataTableByStoredProcedureAsync(DbCallStoredProcedureInput obj);
        Task<List<DataTable>> GetMultipleResultSetsBySPAsync(DbCallStoredProcedureInput obj);
        Task<string> GetScalarByStoredProcedureAsync(DbCallStoredProcedureInput obj);
        Task<DataTable> GetMasterDataAsync(DbCallMasterInput obj);
        Task<DataTable> GetMasterData_MAsync(int iMasterTypeId, string[] columnList, string condition, string compId, string[] orderColoumns);     
        Task<string> InsertToTableAsync(DbCallAddToTableInput obj);
        Task<string> BulkInsertToTableAsync(DbCallBulkInputByTable obj);
        Task<Hashtable> CreateAndBulkInsertToTableAsync(string tableName, List<Hashtable> lstParams, Hashtable coloumns, string compId);
        Task<string> DeleteRowFromTableAsync(DbCallDeleteTableInput obj);
        Task<string> UpdateTableAsync(DbCallUpdateToTableInput obj);           
        Task<DataTable> GetDataTableByViewAsync(string viewName, string[] columnList, string condition, string[] orderColoumns, string compId);
        Task<string> GetScalarByFuncAsync(string functionName,  string compId);
      
    }
}

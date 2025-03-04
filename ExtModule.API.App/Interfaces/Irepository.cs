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
        Task<DataTable> GetDataTableByStoredProcedure(string spName, Hashtable Params, string Compid);
        Task<string> GetScalarByStoredProcedure(string spName, Hashtable Params, string Compid);
        Task<DataTable> GetMasterData(int iMasterTypeId, string[] Columns, string Condition, string CompId, string[] OrderColoumns);
        Task<DataTable> GetMasterData_M(int iMasterTypeId, string[] Columns, string Condition, string CompId, string[] OrderColoumns);
        Task<string> InsertByStoredProcedure(string spName, Hashtable Params, string CompId);
        Task<string> InsertToTable(string sTableName, Hashtable Params, string CompId);
        Task<string> BulkInsertToTable(string TableName, List<Hashtable> Params , string CompId);
        Task<Hashtable> CreateAndBulkInsertToTable(string TableName, List<Hashtable> lstParams, Hashtable coloumns, string CompId);
        Task<string> DeleteRowFromTable(string TableName, string condition, string CompId);
        Task<string> UpdateTable(string TableName, Hashtable Params, string condition, string CompId);
        Task<string> BulkInsertByStoredProcedure(string spName, List<Hashtable> Params, string CompId);
        Task<string> DateToInt(string sDate, string CompId, string connString = "");
        Task<DataTable> GetDataTableByView(string ViewName, string[] Columns, string Condition, string[] OrderColoumns, string CompId);
        Task<string> GetScalarByFunc(string FunctionName,  string CompId);
        Task<Hashtable> SendEmail(string From,string Password,string[] To, string[] Cc, string[] Bcc, string Subject, string Body, string[] Attachment, string SMPT_Host, int SMPT_Port, string CompId = "");
        Task<Hashtable> SendEmailTest(string CompId = "");
    }
}

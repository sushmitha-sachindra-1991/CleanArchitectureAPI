using ExtModule.API.Application.Interfaces;
using ExtModule.API.Core.ERP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Focus.DatabaseFactory;
using Focus.Transactions;
using Focus.TranSettings;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Xml.Linq;
using static ExtModule.API.Core.ERP.F8API;

using ExtModule.API.Logging;
using ExtModule.API.Core;
using Newtonsoft.Json;

using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using static ExtModule.API.Core.CRM;
using System.Xml;
using Focus.TranSettings.DataStructs;
using static System.Net.Mime.MediaTypeNames;

namespace ExtModule.API.Infrastructure.Repositories
{
    public class RepositoryERP : IERPRepository
    {
        string ErrLogName = "Error";
        string InfoLogName = "Info";
        public async Task<DataTable> GetDataTableByStoredProcedureAsync(string spName, Hashtable paramList, string compId)
        {
            var dt = new DataTable();
            string conString = GetConnectionString(compId);                    
            try
            {
                Logger.Instance.LogInfo("Info","Opening connection");

                using (SqlConnection con = new SqlConnection(conString))
                {                    
                    SqlCommand cmd = new SqlCommand(spName, con);
                    Logger.Instance.LogInfo("Info", "Sp Name  "+spName);
                    if (paramList != null)
                    {
                        foreach (DictionaryEntry s in paramList)
                        {
                            cmd.Parameters.AddWithValue("@" + s.Key, s.Value.ToString());
                            Logger.Instance.LogInfo("Info", "Key  " + s.Key+" Value "+s.Value.ToString());
                        }
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    int ConnTimeOut = 600;
                    cmd.CommandTimeout = ConnTimeOut;
                    await con.OpenAsync();
                    
                    using (SqlDataReader reader =await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                   
                }
                Logger.Instance.LogInfo("Info", " connection closed");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError("Error", "GetDataTableByStoredProcedure", ex);
                throw new CustomException("GetDataTableByStoredProcedure",ex);
               
            }
           
            return dt;
        }

        public async Task<List<DataTable>> GetMultipleResultSetsBySPAsync(string spName, Hashtable paramList, string compId)
        {
            var resultTables = new List<DataTable>();
            string conString = GetConnectionString(compId);

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlCommand cmd = new SqlCommand(spName, con))
                    {
                        if (paramList != null)
                        {
                            foreach (DictionaryEntry s in paramList)
                            {
                                cmd.Parameters.AddWithValue("@" + s.Key, s.Value.ToString());
                            }
                        }

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;

                        await con.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            do
                            {
                                var dt = new DataTable();
                                dt.Load(reader);
                                resultTables.Add(dt);
                            } while (!reader.IsClosed && await reader.NextResultAsync());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("GetMultipleResultSets", ex);
            }

            return resultTables;
        }
        public async Task<DataSet> GetDataSetByStoredProcedureAsync(string spName, Hashtable paramList, string compId)
        {
            var ds = new DataSet();
            string conString = GetConnectionString(compId);         

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    SqlCommand cmd = new SqlCommand(spName, con);
                    if (paramList != null)
                    {
                        foreach (DictionaryEntry s in paramList)
                        {

                            cmd.Parameters.AddWithValue("@" + s.Key, s.Value.ToString());
                        }
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    int ConnTimeOut = 600;
                    cmd.CommandTimeout = ConnTimeOut;
                    await con.OpenAsync();

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        ds.Load(reader, LoadOption.PreserveChanges, "Table1");
                    }

                }
            }
            catch (Exception ex)
            {
                throw new CustomException("GetDataSetByStoredProcedureAsync", ex);
            }
            
            return ds;
        }
        public async Task<string> GetScalarByStoredProcedureAsync(string spName, Hashtable paramList, string compId)
        {
            string retVal= "" ;
            string connStr = GetConnectionString(compId);
          
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand sqlCommand = new SqlCommand(spName, con);
                    if (paramList != null)
                    {
                        foreach (DictionaryEntry Param in paramList)
                        {
                            sqlCommand.Parameters.AddWithValue("@" + Param.Key, Param.Value);
                        }
                    }

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    await con.OpenAsync();

                    object result = await sqlCommand.ExecuteScalarAsync();
                    retVal = Convert.ToString(result);

                }
            }
            catch (Exception ex)
            {
                throw new CustomException("GetScalarByStoredProcedureAsync", ex);
            }
           
            return retVal;

        }
        public async Task<DataTable> GetDataTableByViewAsync(
    string viewName,
    string[] columns,
    string condition,
    string[] orderColumns,
    string compId)
{
    var dt = new DataTable();
    string conString = GetConnectionString(compId);

    try
    {
        await using (var con = new SqlConnection(conString))
        {
            var sql = new StringBuilder();
            sql.Append("SELECT ").Append(string.Join(",", columns))
               .Append(" FROM ").Append(viewName)
               .Append(" WHERE 1=1 ");

            if (!string.IsNullOrWhiteSpace(condition))
                sql.Append(" AND ").Append(condition);

            if (orderColumns != null && orderColumns.Length > 0)
                sql.Append(" ORDER BY ").Append(string.Join(",", orderColumns));

            await using (var cmd = new SqlCommand(sql.ToString(), con))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 600;

                await con.OpenAsync();

                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
        }
    }
    catch (Exception ex)
    {
        throw new CustomException("GetDataTableByViewAsync", ex);
    }

    return dt;
}
       
        public async Task<DataTable> GetMasterDataAsync(int iMasterTypeId, string[] columnList, string condition, string compId, string[] orderColoumns)
        {
            var dt = new DataTable();
            string conString = GetConnectionString(compId);
            Logger.Instance.LogInfo("Info",conString);
           
            try
            {
               
                    string text = "Select " + string.Join(",", columnList) + " From " + GetTableNameOfTag(iMasterTypeId, compId, conString) + " Where iStatus <> 5 and sCode <>'' ";
                    if (!string.IsNullOrEmpty(condition))
                    {
                        text = text + " and " + condition;
                    }
                    if (orderColoumns != null)
                    {
                        text = text + " order by " + string.Join(",", orderColoumns);
                    }
                    Logger.Instance.LogInfo("Info", text);
                    Logger.Instance.LogInfo("Info", compId);
                    dt = await GetDataTableByQueryAsync(text, compId);
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
                
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ErrLogName, "GetMasterDataAsync", ex);
                throw new CustomException("GetMasterDataAsync", ex);
            }
           
            return dt;
        }
        public async Task<DataTable> GetMasterData_MAsync(int iMasterTypeId, string[] coloumList, string condition, string compId, string[] orderColoumns)
        {
            var dt = new DataTable();
         
            try
            {
                string conString = GetConnectionString(compId);
                string text = "Select " + string.Join(",", coloumList) + " From " + GetTableNameOfTag_m(iMasterTypeId, compId, conString) + " Where iStatus <> 5 and sCode <>'' ";
                    if (!string.IsNullOrEmpty(condition))
                    {
                        text = text + " and " + condition;
                    }
                    if (orderColoumns != null)
                    {
                        text = text + " order by " + string.Join(",", orderColoumns);
                    }
                   
                    dt =await GetDataTableByQueryAsync(text, compId);
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
                

            }
            catch (Exception ex)
            {
                throw new CustomException("GetMasterData_MAsync", ex);
            }
            
            return dt;
        }
        public async Task<string> GetScalarByFuncAsync(string FunctionName, string CompId)
        {
            var result = "";
            try
            {
                string text = "";
                text = "select dbo."+FunctionName;
                result =await  GetScalarByQueryAsync(text, CompId);
                
            }
            catch (Exception err)
            {
                throw err;
            }

            return result;
        }
             
        public async Task<string> BulkInsertToTableAsync(string tableName, List<Hashtable> lstParams, string compId)
        {
            string retVal = "";
            string connStr = GetConnectionString(compId);
          
            try
            {
                string query = "";
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    List<string> coulumnNames = new List<string>();
                List<string> values = new List<string>();
                    if (lstParams != null)
                    {

                        foreach (Hashtable obj in lstParams)
                        {
                            foreach (DictionaryEntry Param in obj)
                            {
                                if (Param.Value != null)
                                {


                                    coulumnNames.Add(Param.Key.ToString());
                                    values.Add(Param.Value.ToString());

                                }
                            }
                            //insert
                            query = @"insert into " + tableName;
                            string valueList = string.Join(", ", values.Select(item => $"'{item}'"));
                            query = query + "(" + string.Join(',', coulumnNames) + ") values (" + valueList + ");SELECT SCOPE_IDENTITY();";
                            SqlCommand sqlCommand = new SqlCommand(query, con);

                            sqlCommand.CommandType = CommandType.Text;
                           await con.OpenAsync();
                            object result = await sqlCommand.ExecuteScalarAsync();
                            retVal = Convert.ToString(result);
                        }
                    }
                  

                }


            }
            catch (Exception ex)
            {
                throw new CustomException("BulkInsertToTableAsync", ex);
            }
            
            return retVal;
        }
        public async Task<Hashtable> CreateAndBulkInsertToTableAsync(string tableName, List<Hashtable> lstParams,Hashtable coloumns, string compId)
        {
            Hashtable retval = new Hashtable();
            string connStr = GetConnectionString(compId);

            Logger.Instance.LogInfo("EventLog", "ConnStr :" + connStr);
            SqlCommand sqlCommand = null;
            int flag = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                   await con.OpenAsync();
                    string query = "";
                    //drop table
                    string dropSql = $"DROP TABLE IF EXISTS [{tableName}]";
                    await using (var dropCmd = new SqlCommand(dropSql, con))
                    {
                        await dropCmd.ExecuteNonQueryAsync();
                    }


                    //create table
                    var createSql = new StringBuilder();
                    createSql.Append($"CREATE TABLE [{tableName}] (");

                    foreach (DictionaryEntry col in coloumns)
                    {
                        createSql.Append($"[{col.Key}] {col.Value},");
                    }

                    createSql.Length--; // remove last comma
                    createSql.Append(")");

                    await using (var createCmd = new SqlCommand(createSql.ToString(), con))
                    {
                        await createCmd.ExecuteNonQueryAsync();
                    }

                    if (flag != 0)
                    {

                        if (lstParams != null)
                        {

                            foreach (Hashtable obj in lstParams)
                            {
                                List<string> coulumnNames = new List<string>();
                                List<string> values = new List<string>();
                                foreach (DictionaryEntry Param in obj)
                                {
                                    if (Param.Value != null)
                                    {
                                        coulumnNames.Add(Param.Key.ToString());
                                        Logger.Instance.LogInfo("EventLog", "key :" + Param.Key);
                                        Logger.Instance.LogInfo("EventLog","value :"+ Param.Value);
                                        values.Add(Param.Value.ToString());

                                    }
                                }
                              
                                //insert
                                query = @"insert into " + tableName;
                                string valueList = string.Join(", ", values.Select(item => $"'{item}'"));
                                Logger.Instance.LogInfo("EventLog", valueList);
                                query = query + "(" + string.Join(',', coulumnNames) + ") values (" + valueList + ");SELECT SCOPE_IDENTITY();";
                                sqlCommand = new SqlCommand(query, con);

                                sqlCommand.CommandType = CommandType.Text;

                                retval["rows"] = (sqlCommand.ExecuteNonQuery());
                                retval["message"] = "";
                            }
                        }

                    }
                    else
                    {

                        retval["rows"] = 0;
                        retval["message"] = " table " + tableName + " could not be created";
                        return retval;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError("error", ex.InnerException.Message, ex);
                throw new CustomException("CreateAndBulkInsertToTableAsync", ex);
            }
          
            return retval;
        }
        public async Task<string> InsertToTableAsync(string tableName, Hashtable paramList, string compId)
        {
            string retval = "";
            string connStr = GetConnectionString(compId);
           
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "";

                    List<string> coulumnNames = new List<string>();
                    List<string> values = new List<string>();
                    if (paramList != null)
                    {


                        foreach (DictionaryEntry Param in paramList)
                        {
                            if (Param.Value != null)
                            {
                                coulumnNames.Add(Param.Key.ToString());
                                values.Add(Param.Value.ToString());

                            }
                        }
                        //insert
                        query = @"insert into " + tableName;
                        string valueList = string.Join(", ", values.Select(item => $"'{item}'"));
                        query = query + "(" + string.Join(',', coulumnNames) + ") values (" + valueList + ");SELECT SCOPE_IDENTITY();";
                        SqlCommand sqlCommand = new SqlCommand(query, con);

                        sqlCommand.CommandType = CommandType.Text;
                       await con.OpenAsync();
                        object result = await sqlCommand.ExecuteScalarAsync();
                        retval = Convert.ToString(result);
                    }
                }
                
               
            }
            catch (Exception ex)
            {
                throw new CustomException("InsertToTableAsync", ex);
            }
           
            return retval;
        }

        public async Task<string> UpdateTableAsync(string tableName, Hashtable paramList,string condition, string compId)
        {
            string retval = "";
            string connStr = GetConnectionString(compId);
            
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "";

                    List<string> coulumnNames = new List<string>();
                    List<string> values = new List<string>();
                    if (paramList != null)
                    {

                        query = query + "update " + tableName + " set ";
                        string valueList = "";
                        foreach (DictionaryEntry Param in paramList)
                        {
                            if (Param.Value != null)
                            {

                                valueList = valueList + Param.Key.ToString() + "=" + $"'{Param.Value}',";
                            }
                        }
                        valueList = valueList.TrimEnd(',');

                        query = query + valueList + " where 1=1 and " + condition;
                        SqlCommand sqlCommand = new SqlCommand(query, con);

                        sqlCommand.CommandType = CommandType.Text;
                       await con.OpenAsync();
                        object result =await sqlCommand.ExecuteNonQueryAsync();
                        retval = Convert.ToString(result);

                    }
                }


            }
            catch (Exception ex)
            {
                throw new CustomException("UpdateTableAsync", ex);
            }
            
            return retval;
        }
        public async Task<string> DeleteRowFromTableAsync(string tableName, string condition, string compId)
        {
            string retval = "";
            string connStr = GetConnectionString(compId);
          
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "";

                    List<string> coulumnNames = new List<string>();
                    List<string> values = new List<string>();
                    if (!string.IsNullOrEmpty(tableName))
                    {

                        //delete
                        query = @"delete  " + tableName;

                        query = query + " where " + condition;
                        SqlCommand sqlCommand = new SqlCommand(query, con);

                        sqlCommand.CommandType = CommandType.Text;
                       await con.OpenAsync();
                        object result = await (sqlCommand.ExecuteNonQueryAsync());
                        retval = Convert.ToString(result);

                    }

                }
            }
            catch (Exception ex)
            {
                throw new CustomException("DeleteRowFromTableAsync", ex);
            }
           
            return retval;
        }


        #region common functions
        public string GetConnectionString(string CompId)
        {
            string rtnConnStr = "";
            try
            {
                if (Convert.ToString(CompId) == "")
                {
                    CompId = "36";
                }

                string CompCode = DatabaseWrapper.GetCompanyCode(Convert.ToInt32(CompId));
                string[] SqlCon = DatabaseWrapper.GetDatabaseDetails();

                string SQLServer = SqlCon[0];
                string SQLUser = SqlCon[1];
                string SQLPW = SqlCon[2];

                rtnConnStr = "server=" + SQLServer + ";Database=Focus8" + CompCode + " ;Integrated Security=false;User ID=" + SQLUser + ";password=" + SQLPW;

            }
            catch (Exception ex)
            {
                //ErrLog(ex, "DevLib.GetConnectionString()");
                //throw;
            }
            return rtnConnStr;
        }

        private string GetTableNameOfTag(int iMasterTypeId, string CompId, string connString = "")
        {
            string result = "";
            try
            {
                string text = "";
                text = "Select 'v' + sModule +'_' + sMasterName [TableName] From cCore_MasterDef Where iMasterTypeId = " + Convert.ToString(iMasterTypeId);
                result = GetScalarByQuery(text, CompId);
            }
            catch (Exception err)
            {
                //ErrLog(err, "DevLib.GetTableNameOfTag()");
            }

            return result;
        }
        private string GetTableNameOfTag_m(int iMasterTypeId, string CompId, string connString = "")
        {
            string result = "";
            try
            {
                string text = "";
                text = "Select 'vm' + sModule +'_' + sMasterName [TableName] From cCore_MasterDef Where iMasterTypeId = " + Convert.ToString(iMasterTypeId);
                result = GetScalarByQuery(text, CompId);
            }
            catch (Exception err)
            {
                //ErrLog(err, "DevLib.GetTableNameOfTag()");
            }

            return result;
        }


        public async Task<string>  GetScalarByQueryAsync(string strQry, string compId)
        {
            string retVal = "";

            string ConnStr = GetConnectionString(compId);



            try
            {
                using (SqlConnection con = new SqlConnection(ConnStr))
                {
                    SqlCommand sqlCommand = new SqlCommand(strQry, con);
                    await con.OpenAsync();
                    object result=await sqlCommand.ExecuteScalarAsync();
                    retVal = Convert.ToString(result);
                }
            }
            catch (Exception err)
            {
                // ErrLog(err, "DevLib.GetScalarByQuery()");
            }


            return retVal;
        }
        public string GetScalarByQuery(string strQry, string CompId)
        {
            string result = "";
          
             string ConnStr=GetConnectionString(CompId);
            

           
            try
            {
                using (SqlConnection con = new SqlConnection(ConnStr))
                {
                    SqlCommand sqlCommand = new SqlCommand(strQry, con);
                    con.Open();
                    result = Convert.ToString(sqlCommand.ExecuteScalar());
                }
            }
            catch (Exception err)
            {
               // ErrLog(err, "DevLib.GetScalarByQuery()");
            }
           

            return result;
        }
        public async Task<DataTable> GetDataTableByQueryAsync(string query, string compId = "")
        {
            var dt = new DataTable();
            string connStr = GetConnectionString(compId);

            try
            {
                using (var con = new SqlConnection(connStr))
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.CommandTimeout = 6000;

                    await con.OpenAsync();

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ErrLogName, "GetDataTableByQueryAsync", ex);
                // Log your exception here
                // ErrLog(ex, $"GetDataTableByQueryAsync({query})");
                throw;
            }

            return dt;
        }
        public DataTable GetDataTableByQuery(string strQry, string CompId = "")
        {

            DataTable dataTable = new DataTable();
            string ConnStr = GetConnectionString(CompId);


            try
            {
                using (SqlConnection con = new SqlConnection(ConnStr))
                {
                    SqlCommand sqlCommand = new SqlCommand(strQry, con);
                    int commandTimeout = 6000;
                    sqlCommand.CommandTimeout = commandTimeout;
                    con.Open();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    sqlDataAdapter.Fill(dataTable);
                }
            }
            catch (Exception err)
            {
                // ErrLog(err, "DevLibWeb.GetDataTableByQuery(" + strQry + ")");
            }
            finally
            {
            }

            //EventLog("GetDataTable - OUT");
            return dataTable;
        }
     
        public Hashtable GetCompanyDetails(string company)
        {
            Hashtable obj = new Hashtable();
            try
            {
                if (!string.IsNullOrEmpty(company))
                {
                    //  company = company.Replace("&", "and");
                    XmlDocument doc = new XmlDocument();
                    string strPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);  //System.IO.Directory.GetCurrentDirectory();

                    doc.Load(strPath + @"\CompanyDetails.xml");
                    
                        foreach (XmlNode comp in doc.SelectNodes("Company"))
                        {
                            string name = comp.SelectSingleNode("Name").InnerText;
                            if (name == company)
                            {
                                obj.Add("FromEmailId", comp.SelectSingleNode("ExtDailyAttd_EmailUserName").InnerText);
                                obj.Add("FromPassword", comp.SelectSingleNode("ExtDailyAttd_EmailPassword").InnerText);
                             
                                return obj;
                            }
                        }
                    
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }

        #endregion

    }
}

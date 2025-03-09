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

using ExtModule.API.Core;
using Newtonsoft.Json;
using static ExtModule.API.Infrastructure.Repositories.RepositoryERP.Focus8API;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using static ExtModule.API.Core.CRM;
using System.Xml;

namespace ExtModule.API.Infrastructure.Repositories
{
    public class RepositoryERP : IERPRepository
    {
        public async Task<DataTable> GetDataTableByStoredProcedure(string spName, Hashtable Params, string CompId)
        {
            var dt = new DataTable();
            string conString = GetConnectionString(CompId);
          
            SqlConnection con = new SqlConnection(conString);

            try
            {
                SqlCommand cmd = new SqlCommand(spName, con);
                if (Params != null)
                {
                    foreach (DictionaryEntry s in Params)
                    {
                        cmd.Parameters.AddWithValue("@" + s.Key, s.Value.ToString());
                    }
                }

                cmd.CommandType = CommandType.StoredProcedure;
                int ConnTimeOut = 600;
                cmd.CommandTimeout = ConnTimeOut;
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new CustomException("GetDataTableByStoredProcedure",ex);
               
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            return dt;
        }
        public async Task<DataSet> GetDataSetByStoredProcedure(string spName, Hashtable Params, string CompId)
        {
            var ds = new DataSet();
            string conString = GetConnectionString(CompId);

            SqlConnection con = new SqlConnection(conString);

            try
            {
                SqlCommand cmd = new SqlCommand(spName, con);
                if (Params != null)
                {
                    foreach (DictionaryEntry s in Params)
                    {

                        cmd.Parameters.AddWithValue("@" + s.Key, s.Value.ToString());
                    }
                }

                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw new CustomException("GetDataSetByStoredProcedure", ex);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            return ds;
        }
        public async Task<string> GetScalarByStoredProcedure(string spName, Hashtable Params, string CompId)
        {
            string retval= "" ;
            string ConnStr = GetConnectionString(CompId);
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            try
            {
                SqlCommand sqlCommand = new SqlCommand(spName, sqlConnection);
                if (Params != null)
                {
                    foreach (DictionaryEntry Param in Params)
                    {
                        sqlCommand.Parameters.AddWithValue("@" + Param.Key, Param.Value);
                    }
                }

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlConnection.Open();
                retval = Convert.ToString(sqlCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new CustomException("GetScalarByStoredProcedure", ex);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            return await Task.FromResult(retval);

        }
        public async Task<DataTable> GetDataTableByView(string ViewName, string[] Columns,string Condition, string[] OrderColoumns, string CompId)
        {
            var dt = new DataTable();
            string conString = GetConnectionString(CompId);

            SqlConnection con = new SqlConnection(conString);

            try
            {
                string text = "Select " + string.Join(",", Columns) + " From " + ViewName + " Where 1=1 ";
                if (!string.IsNullOrEmpty(Condition))
                {
                    text = text + " and " + Condition;
                }
                if (OrderColoumns != null)
                {
                    text = text + " order by " + string.Join(",", OrderColoumns);
                }
                SqlCommand cmd = new SqlCommand(text, con);
                

                cmd.CommandType = CommandType.Text;
                int ConnTimeOut = 600;
                cmd.CommandTimeout = ConnTimeOut;
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new CustomException("GetDataTableByView", ex);

            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            return dt;
        }
        public async Task<string> DateToInt(string sDate, string CompId, string connString = "")
        {
            var result = "";
            try
            {
                string text = "";
                text = "select dbo.datetoint('" + Convert.ToString(sDate) + "')";
                result = GetScalarByQuery(text, CompId);
            }
            catch (Exception err)
            {
                //ErrLog(err, "DevLib.GetTableNameOfTag()");
            }

            return await Task.FromResult(result);
        }
        public async Task<DataTable> GetMasterData(int iMasterTypeId, string[] Columns, string Condition, string CompId, string[] OrderColoumns )
        {
            var dt = new DataTable();
            string conString = GetConnectionString(CompId);

            SqlConnection con = new SqlConnection(conString);

            try
            {
                string text = "Select " + string.Join(",", Columns) + " From " + GetTableNameOfTag(iMasterTypeId, CompId, conString) + " Where iStatus <> 5 and sCode <>'' ";
                if (!string.IsNullOrEmpty(Condition))
                {
                    text = text + " and " + Condition;
                }
                if (OrderColoumns !=null)
                {
                    text =text+" order by " + string.Join(",",OrderColoumns);
                }

                dt = GetDataTableByQuery(text, CompId);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
               
            }
            catch (Exception ex)
            {
                throw new CustomException("GetMasterData", ex);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            return dt;
        }
        public async Task<DataTable> GetMasterData_M(int iMasterTypeId, string[] Columns, string Condition, string CompId, string[] OrderColoumns)
        {
            var dt = new DataTable();
            string conString = GetConnectionString(CompId);

            SqlConnection con = new SqlConnection(conString);

            try
            {
                string text = "Select " + string.Join(",", Columns) + " From " + GetTableNameOfTag_m(iMasterTypeId, CompId, conString) + " Where iStatus <> 5 and sCode <>'' ";
                if (!string.IsNullOrEmpty(Condition))
                {
                    text = text + " and " + Condition;
                }
                if (OrderColoumns != null)
                {
                    text = text + " order by " + string.Join(",", OrderColoumns);
                }

                dt = GetDataTableByQuery(text, CompId);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }

            }
            catch (Exception ex)
            {
                throw new CustomException("GetMasterData", ex);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            return dt;
        }
        public async Task<string> GetScalarByFunc(string FunctionName, string CompId)
        {
            var result = "";
            try
            {
                string text = "";
                text = "select dbo."+FunctionName;
                result = GetScalarByQuery(text, CompId);
            }
            catch (Exception err)
            {
                //ErrLog(err, "DevLib.GetTableNameOfTag()");
            }

            return await Task.FromResult(result);
        }
        public async Task<string> InsertByStoredProcedure(string spName, Hashtable Params, string CompId)
         {
            string retval = "";
            string ConnStr = GetConnectionString(CompId);
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            try
            {
                SqlCommand sqlCommand = new SqlCommand(spName, sqlConnection);
                if (Params != null)
                {
                    foreach (DictionaryEntry Param in Params)
                    {
                        sqlCommand.Parameters.AddWithValue("@" + Param.Key, Param.Value);
                    }
                }

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlConnection.Open();
                retval = Convert.ToString(sqlCommand.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new CustomException("InsertByStoredProcedure", ex);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            return await Task.FromResult(retval);
         }

        public async Task<string> BulkInsertByStoredProcedure(string spName, List<Hashtable> Params, string CompId)
        {

            int rows = 0;
            string ConnStr = GetConnectionString(CompId);
            SqlCommand sqlCommand = null;
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            try
            {
                sqlConnection.Open();
                foreach (Hashtable s in Params)
                {
                    sqlCommand = null;
                    sqlCommand = new SqlCommand(spName, sqlConnection);
                    if (Params != null)
                    {
                        foreach (DictionaryEntry Param in s)
                        {
                            sqlCommand.Parameters.AddWithValue("@" + Param.Key, Param.Value);
                        }
                    }

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                
                    rows = rows+ (sqlCommand.ExecuteNonQuery());
                }
                 
            }
            catch (Exception ex)
            {
                throw new CustomException("BulkInsertByStoredProcedure", ex);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            return await Task.FromResult(rows.ToString());
        }
        public async Task<string> BulkInsertToTable(string TableName, List<Hashtable> lstParams, string CompId)
        {
            string retval = "";
            string ConnStr = GetConnectionString(CompId);
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            try
            {
                string query = "";

                List<string> coulumnNames = new List<string>();
                List<string> values = new List<string>();
                if (lstParams != null)
                {
                    
           foreach(Hashtable obj in lstParams)
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
                        query = @"insert into " + TableName;
                        string valueList = string.Join(", ", values.Select(item => $"'{item}'"));
                        query = query + "(" + string.Join(',', coulumnNames) + ") values (" + valueList + ");SELECT SCOPE_IDENTITY();";
                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                        sqlCommand.CommandType = CommandType.Text;
                        sqlConnection.Open();
                        retval = Convert.ToString(sqlCommand.ExecuteScalar());
                    }
                  

                }


            }
            catch (Exception ex)
            {
                throw new CustomException("BulkInsertToTable", ex);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            return await Task.FromResult(retval);
        }
        public async Task<Hashtable> CreateAndBulkInsertToTable(string TableName, List<Hashtable> lstParams,Hashtable coloumns, string CompId)
        {
            Hashtable retval = new Hashtable();
            string ConnStr = GetConnectionString(CompId);
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            sqlConnection.Open();
            SqlCommand sqlCommand = null;
            try
            {
                string query = "";
                //drop table
                query = "Drop table if exists " + TableName;
                sqlCommand = new SqlCommand(query, sqlConnection);

                sqlCommand.CommandType = CommandType.Text;
                 sqlCommand.ExecuteNonQuery();
                //create table
                query = "Create Table " + TableName +" (";
                foreach (DictionaryEntry param in coloumns)
                {
                    query = query + param.Key.ToString() +" "+param.Value.ToString()+" , ";
                }
                query = query.Trim().TrimEnd(',');
                query = query + ")";
                sqlCommand = new SqlCommand(query, sqlConnection);

                sqlCommand.CommandType = CommandType.Text;

               int flag = (sqlCommand.ExecuteNonQuery());
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
                                    values.Add(Param.Value.ToString());

                                }
                            }
                            //insert
                            query = @"insert into " + TableName;
                            string valueList = string.Join(", ", values.Select(item => $"'{item}'"));
                            query = query + "(" + string.Join(',', coulumnNames) + ") values (" + valueList + ");SELECT SCOPE_IDENTITY();";
                            sqlCommand = new SqlCommand(query, sqlConnection);

                            sqlCommand.CommandType = CommandType.Text;

                            retval["rows"] = (sqlCommand.ExecuteNonQuery());
                            retval["message"] = "";
                        }
                    }

                }
                else
                {

                    retval["rows"] = 0;
                    retval["message"] = " table " + TableName + " could not be created";
                    return retval;
                }               

            }
            catch (Exception ex)
            {
                throw new CustomException("CreateAndBulkInsertToTable", ex);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            return await Task.FromResult(retval);
        }
        public async Task<string> InsertToTable(string TableName, Hashtable Params, string CompId)
        {
            string retval = "";
            string ConnStr = GetConnectionString(CompId);
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            try
            {
                string query = "";
               
                List<string> coulumnNames = new List<string>();
                List<string> values = new List<string>();
                if (Params != null)
                {
                 
                    
                        foreach (DictionaryEntry Param in Params)
                        {                        
                        if (Param.Value!=null)
                        {
                            coulumnNames.Add(Param.Key.ToString());
                            values.Add(Param.Value.ToString());
                                
                        }                        
                        }
                        //insert
                        query = @"insert into " + TableName;
                        string valueList = string.Join(", ", values.Select(item => $"'{item}'"));
                        query = query + "(" + string.Join(',', coulumnNames) + ") values (" + valueList + ");SELECT SCOPE_IDENTITY();";
                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                        sqlCommand.CommandType = CommandType.Text;
                        sqlConnection.Open();
                        retval = Convert.ToString(sqlCommand.ExecuteScalar());                    
                }
                
               
            }
            catch (Exception ex)
            {
                throw new CustomException("InsertToTable", ex);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            return await Task.FromResult(retval);
        }

        public async Task<string> UpdateTable(string TableName, Hashtable Params,string condition, string CompId)
        {
            string retval = "";
            string ConnStr = GetConnectionString(CompId);
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            try
            {
                string query = "";

                List<string> coulumnNames = new List<string>();
                List<string> values = new List<string>();
                if (Params != null)
                {
                    
                        query = query + "update " + TableName + " set ";
                        string valueList = "";
                        foreach (DictionaryEntry Param in Params)
                        {
                            if (Param.Value != null)
                            {
                                
                                valueList = valueList + Param.Key.ToString() + "=" + $"'{Param.Value}',";
                            }
                        }
                        valueList = valueList.TrimEnd(',');

                        query = query + valueList + " where 1=1 and "+condition;
                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                        sqlCommand.CommandType = CommandType.Text;
                        sqlConnection.Open();
                        retval = Convert.ToString(sqlCommand.ExecuteNonQuery());
                    
                }


            }
            catch (Exception ex)
            {
                throw new CustomException("InsertToTable", ex);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            return await Task.FromResult(retval);
        }
        public async Task<string> DeleteRowFromTable(string TableName, string condition, string CompId)
        {
            string retval = "";
            string ConnStr = GetConnectionString(CompId);
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            try
            {
                string query = "";

                List<string> coulumnNames = new List<string>();
                List<string> values = new List<string>();
                if (!string.IsNullOrEmpty(TableName ))
                {

                    //delete
                    query = @"delete  " + TableName;
                   
                    query = query + " where "+condition;
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                    sqlCommand.CommandType = CommandType.Text;
                    sqlConnection.Open();
                    retval = Convert.ToString(sqlCommand.ExecuteNonQuery());

                }


            }
            catch (Exception ex)
            {
                throw new CustomException("DeleteRowFromTable", ex);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            return await Task.FromResult(retval);
        }
        public async Task<FocusPostResponse<F8API.PostResponse>>  InterCompanyPostingByStoredProcedureVoucherWise(string spName, Hashtable Param, string SessionId, string CompId)
        {
            FocusPostResponse<F8API.PostResponse> res = new FocusPostResponse<F8API.PostResponse>();
            F8API obj = new F8API();
            string err = "";
            try
            {

                if (!string.IsNullOrEmpty(CompId))
                {
                    // Hashtable Param = JsonConvert.DeserializeObject<Hashtable>(obj.Param);


                    DataSet ds = await GetDataSetByStoredProcedure(spName, Param, CompId);
                    if (ds.Tables.Count == 2)
                    {
                        DataTable dt_h = ds.Tables[0];
                        DataTable dt_b = ds.Tables[1];
                        if (dt_h.Rows.Count > 0)
                        {
                            Hashtable header = new Hashtable();
                            foreach (DataColumn dc in dt_h.Columns)
                            {
                                if (!dc.ColumnName.Contains('*'))
                                    header.Add(dc.ColumnName, dt_h.Rows[0][dc.ColumnName]);
                            }
                            string PostVoucher = dt_h.Rows[0]["*PostToVoucher"].ToString();
                            if (dt_b.Rows.Count > 0)
                            {
                                List<Hashtable> body = new List<Hashtable>();
                                foreach (DataRow dr in dt_b.Rows)
                                {
                                    Hashtable row = new Hashtable();
                                    foreach (DataColumn dc in dt_b.Columns)
                                    {
                                        if (!dc.ColumnName.Contains('*'))
                                        {
                                            row.Add(dc.ColumnName, dr[dc.ColumnName]);
                                        }
                                    }
                                    //Adding discount amount
                                    if (dt_b.Columns.Contains("Discount Amt_Input_*"))
                                    {
                                        Hashtable discountAmnt = new Hashtable();
                                        discountAmnt.Add("Input", dr["Discount Amt_Input_*"]);
                                        discountAmnt.Add("FieldName", "Discount Amt");
                                        discountAmnt.Add("FieldId", dr["Discount Amt_FieldId_*"]);
                                        discountAmnt.Add("Value", dr["Discount Amt_Value_*"]);
                                        row.Add("Discount Amt", discountAmnt);
                                    }
                                    //Adding discount %
                                    if (dt_b.Columns.Contains("Discount %_Input_*"))
                                    {
                                        Hashtable discountpercent = new Hashtable();
                                        discountpercent.Add("Input", dr["Discount %_Input_*"]);
                                        discountpercent.Add("FieldName", "Discount %");
                                        discountpercent.Add("FieldId", dr["Discount %_FieldId_*"]);
                                        discountpercent.Add("Value", dr["Discount %_Value_*"]);
                                        row.Add("Discount %", discountpercent);
                                    }
                                    //Adding Vat
                                    if (dt_b.Columns.Contains("VAT_Input_*"))
                                    {
                                        Hashtable vat = new Hashtable();
                                        vat.Add("Input", dr["VAT_Input_*"]);
                                        vat.Add("FieldName", "VAT");
                                        vat.Add("FieldId", dr["VAT_FieldId_*"]);
                                        vat.Add("Value", dr["VAT_Value_*"]);
                                        row.Add("VAT", vat);
                                    }
                                    body.Add(row);
                                }


                                F8API.PostingData postingData = new F8API.PostingData();
                                postingData.data.Add(new Hashtable { { "Header", header }, { "Body", body } });
                                string sContent = JsonConvert.SerializeObject(postingData);
                                //xlib.EventLog(sContent);
                                string url = obj.urlVouchers + PostVoucher;
                                var response = Focus8API.Post(url, sContent, SessionId, ref err);
                                if (response != null)
                                {
                                    var responseData = JsonConvert.DeserializeObject<F8API.PostResponse>(response);
                                    if (responseData.result == 1)
                                    {
                                        res.sMessage = "success";
                                        res.F8APIPost = responseData;
                                        res.status = 1;
                                        Hashtable updateParams = new Hashtable();
                                        foreach (DictionaryEntry s in Param)
                                        {
                                            if (s.Key.ToString().ToUpper() == "ACTION")
                                            {
                                                updateParams[s.Key] = "1";//update the base voucher with posted doc no
                                            }
                                            else
                                            {
                                                updateParams.Add(s.Key, s.Value);
                                            }

                                        }
                                        updateParams.Add("postedVoucherNo", Convert.ToInt32(responseData.data[0]["VoucherNo"]));

                                        string row = await InsertByStoredProcedure(spName, updateParams, CompId);

                                        return res;
                                    }
                                    else
                                    {
                                        res.sMessage = "Failed";
                                        res.F8APIPost = responseData;
                                        res.status = 0;
                                    }
                                }

                            }
                            else
                            {
                                res.sMessage = "No data for the body part";
                                res.F8APIPost = null;
                                res.status = 0;
                            }
                        }
                        else
                        {
                            res.sMessage = "No data for the header part";
                            res.F8APIPost = null;
                            res.status = 0;
                        }


                        return res;
                    }

                }
            }
            catch (Exception ex)
            {
                //throw new CustomException("BulkInsertByStoredProcedure", ex);
                res.F8APIPost = null;
                res.sMessage = ex.Message;
                res.status = 0;

                return res;
            }

            return res;

        }
        public async Task<FocusPostResponse<F8API.PostResponse>> DeleteVoucher(string VoucherName, string VoucherNo, string sessionId,  string baseFocusAPIUrl = "")
        {          
            FocusPostResponse<F8API.PostResponse> res = new FocusPostResponse<F8API.PostResponse>();
            try
            {
                string sUrl = "";                
                sUrl = baseFocusAPIUrl + "/Transactions/" + VoucherName + "/" + VoucherNo.Replace("/","~~");
              
                using (var client = new WebClientDel())
                {
                    client.Headers.Add("fSessionId", sessionId);
                    client.Headers.Add("Content-Type", "application/json");
    
                    string strResponse = client.UploadString(sUrl, "DELETE", "");
              
                   var response = JsonConvert.DeserializeObject<PostResponse>(strResponse);
                    if (response != null)
                    {
                        res.sMessage = "success";
                        res.F8APIPost = response;
                        res.status = 1;
                    }

                    return res;

                }
            }
            catch (Exception e)
            {
                throw new CustomException("DeleteVoucher", e);
            }

        }
        public async Task<FocusPostResponse<F8API.PostResponse>> PostToJVByStoredProcedureVoucherWise(string spName, Hashtable Param, string SessionId, string CompId,string baseFocusAPIUrl)
        {
            FocusPostResponse<F8API.PostResponse> res = new FocusPostResponse<F8API.PostResponse>();
            F8API obj = new F8API();
            string err = "";
            try
            {

                if (!string.IsNullOrEmpty(CompId))
                {
                    // Hashtable Param = JsonConvert.DeserializeObject<Hashtable>(obj.Param);


                    DataSet ds = await GetDataSetByStoredProcedure(spName, Param, CompId);
                    if (ds.Tables.Count == 2)
                    {
                        DataTable dt_h = ds.Tables[0];
                        DataTable dt_b = ds.Tables[1];
                        if (dt_h.Rows.Count > 0)
                        {
                            Hashtable header = new Hashtable();
                            foreach (DataColumn dc in dt_h.Columns)
                            {
                                if (!dc.ColumnName.Contains('*'))
                                    header.Add(dc.ColumnName, dt_h.Rows[0][dc.ColumnName]);
                            }
                            string PostVoucher = dt_h.Rows[0]["*PostToVoucher"].ToString();
                            string DocNo = dt_h.Rows[0]["DocNo"].ToString();
                            if (dt_b.Rows.Count > 0)
                            {
                                List<Hashtable> body = new List<Hashtable>();
                                foreach (DataRow dr in dt_b.Rows)
                                {
                                    Hashtable row = new Hashtable();
                                    foreach (DataColumn dc in dt_b.Columns)
                                    {
                                        if (!dc.ColumnName.Contains('*'))
                                        {
                                            row.Add(dc.ColumnName, dr[dc.ColumnName]);
                                        }
                                    }
                                   
                                    body.Add(row);
                                }


                                F8API.PostingData postingData = new F8API.PostingData();
                                postingData.data.Add(new Hashtable { { "Header", header }, { "Body", body } });
                                if(!string.IsNullOrEmpty( DocNo ))
                                {
                                   var result= DeleteVoucher(PostVoucher, DocNo, SessionId, baseFocusAPIUrl);
                                }
                                string sContent = JsonConvert.SerializeObject(postingData);
                                //xlib.EventLog(sContent);
                                string url =baseFocusAPIUrl+ obj.urlVouchers + PostVoucher;
                                var response = Focus8API.Post(url, sContent, SessionId, ref err);
                                if (response != null)
                                {
                                    var responseData = JsonConvert.DeserializeObject<F8API.PostResponse>(response);
                                    if (responseData.result == 1)
                                    {
                                        res.sMessage = "success";
                                        res.F8APIPost = responseData;
                                        res.status = 1;
                                        
                                        return res;
                                    }
                                    else
                                    {
                                        res.sMessage = "Failed";
                                        res.F8APIPost = responseData;
                                        res.status = 0;
                                    }
                                }

                            }
                            else
                            {
                                res.sMessage = "No data for the body part";
                                res.F8APIPost = null;
                                res.status = 0;
                            }
                        }
                        else
                        {
                            res.sMessage = "No data for the header part";
                            res.F8APIPost = null;
                            res.status = 0;
                        }


                        return res;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new CustomException("PostToJVByStoredProcedureVoucherWise", ex);
            }

            return res;

        }

        #region GetSessionId
        public async Task<loginRes> Login(string username, string password, string companycode, string sUrl)
        {
            loginRes obj = new loginRes();
            try
            {
                 obj=(Focus8API.Login(username, password, companycode, sUrl));
            }
            catch (Exception ex)
            {
                throw new CustomException("Login", ex);

            }
            return obj;
        }
        #endregion
        #region ValidateSessionId
        public async Task<loginRes> validateFocusSessionId(string sesessionId,string companycode, string sUrl)
        {
            loginRes obj = new loginRes();
            try
            {
                obj = (Focus8API.validateSession( companycode,sesessionId, sUrl));
            }
            catch (Exception ex)
            {
                throw new CustomException("Login", ex);

            }
            return obj;
        }
        #endregion
        #region LogOut
        public async Task<loginRes> LogOut(string sessionId, string sUrl)
        {
            loginRes obj = new loginRes();
            try
            {
                
                obj = (Focus8API.GetLogOut(sessionId, sUrl));
            }
            catch (Exception ex)
            {
                throw new CustomException("Login", ex);

            }
            return obj;
        }
        #endregion

        #region Email Test
        public async Task<Hashtable> SendEmailTest( string CompId = "")
        {

            Hashtable response = new Hashtable();
            
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                    | SecurityProtocolType.Tls11
                                    | SecurityProtocolType.Tls12;
            //test
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("test@egss.ae", "T@egss25"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("test@egss.ae"),
                Subject = "Test Email",
                Body = "This is a test email sent using SMTP with Gmail in .NET 8.0",
                IsBodyHtml = true,
            };
            mailMessage.To.Add("sushmita@focussoftnet.ae");
            try
            {
                smtpClient.Send(mailMessage);
                response["status"] = 1 ;
                response["message"] = "success";
            }
            catch (Exception e)
            {
                response["status"] = 0;
                response["message"] = e.Message;
            }
            
            return response;
            }
        #endregion
        #region Email
        /// <summary>
        /// Send Email
        /// </summary>

        /// <param name="To">To sloginuserName [string array]</param>
        /// <param name="Cc">Cc sloginuserName [string array]</param>
        /// <param name="Bcc">BCc sloginuserName [string array]</param>
        /// <param name="Subject">Subject</param>
        /// <param name="Body">Body</param>
        /// <param name="Attachment">Attachment give physical path [string array]</param>
        /// <param name="SMPT_Host">SMPT Host, set "" to get value from Preferences </param>
        /// <param name="SMPT_Port">SMPT Port, set 0 to get value from Preferences </param>
        /// <returns>Return status,message as hashtable</returns>
        public async Task<Hashtable> SendEmail(string From,string Password, string[] To, string[] Cc, string[] Bcc, string Subject, string Body, string[] Attachment, string SMPT_Host, int SMPT_Port, string CompId = "")
        {
            
            Hashtable   response=new Hashtable();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                    | SecurityProtocolType.Tls11
                                    | SecurityProtocolType.Tls12;
              
                ////test
                //using (var message = new MailMessage())
                //{
                //    message.To.Add(new MailAddress("sushmita@focussoftnet.ae", "sushmita"));
                //    message.From = new MailAddress("test@egss.ae", "T@egss25");
                //    message.Subject = "My subject";
                //    message.Body = "My message";
                //    message.IsBodyHtml = false; // change to true if body msg is in html

                //    using (var client = new SmtpClient("smtp.office365.com"))
                //    {
                //        client.UseDefaultCredentials = false;
                //        client.Port = 465;
                //        client.Credentials = new NetworkCredential("test@egss.ae", "T@egss25");
                //        client.EnableSsl = true;

                //        try
                //        {
                //            await client.SendMailAsync(message); // Email sent
                //        }
                //        catch (Exception e)
                //        {
                //            response["status"] = 0;
                //            response["message"] = e.Message;
                //        }
                //    }
                //}
                //    //end
                    if (SMPT_Port == 0)
                {
                    SMPT_Port = 587;
                }
                if (SMPT_Host == "")
                {
                    SMPT_Host = GetScalarByQuery("Select sValue From cCore_PreferenceText_0 Where iCategory = 13 and iFieldId = 0", CompId);
                }

                if (SMPT_Host == "")
                {
                    response["status"] = 0;
                    response["Message"] = "SMTP Address not defined";

                    
                    return response;
                }
                ////GetCompany details
                //string sCompanyName = GetScalarByQuery("select sCompanyName from Focus8ERP.dbo.mcore_company where iCompanyId=" + CompId, CompId);
                //Hashtable obj = GetCompanyDetails(sCompanyName);
                //string From = (string)obj["FromEmailId"];
                //string Password =(string) obj["FromPassword"];
                if (From == "")
                {
                     From = GetScalarByQuery("Select sValue From cCore_PreferenceText_0 Where iCategory = 13 and iFieldId = 1", CompId);
                }
                if (Password == "")
                {
                    Password = GetScalarByQuery("Select sValue From cCore_PreferenceText_0 Where iCategory = 13 and iFieldId = 2", CompId);
                }



                // '''''''''''''''''''''''''''''''''''''''''Sending Mail''''''''''''''''''''''''''''''''
                // sendMail()
                using (var mail = new MailMessage())
                {
                    


                    mail.From = new MailAddress(From);

                    if (To.Length == 0)
                    {

                        response["status"] = 0;
                        response["Message"] = "To Email ID not defined";
                        return response;

                    }

                    foreach (string s in To)
                    {
                        if (s.Trim() != "")
                        {
                            string sEmail = GetScalarByQuery("select sEmail from mSec_Users where sLoginName='" + s + "'", CompId);
                            mail.To.Add(new MailAddress(sEmail));
                        }
                    }

                    foreach (string s in Cc)
                    {
                        if (s.Trim() != "")
                        {
                            string sEmail = GetScalarByQuery("select sEmail from mSec_Users where sLoginName='" + s + "'", CompId);
                            mail.CC.Add(new MailAddress(sEmail));
                        }
                    }

                    foreach (string s in Bcc)
                    {
                        if (s.Trim() != "")
                        {
                            string sEmail = GetScalarByQuery("select sEmail from mSec_Users where sLoginName='" + s + "'", CompId);
                            mail.Bcc.Add(new MailAddress(sEmail));
                        }
                    }

                    // ''''''''''''''''''''''''''''''Email Subject''''''''''''''''''''''''''''''''''''''''
                    if ((Subject == ""))
                    {
                        mail.Subject = " ";
                    }
                    else
                    {
                        mail.Subject = Subject;
                    }

                    // ''''''''''''''''''''''''''''''Email Body''''''''''''''''''''''''''''''''''''''''

                    if (Body == "")
                    {
                        mail.Body = " ";
                    }
                    else
                    {
                        mail.Body = Body;
                    }

                    // ''''''''''''''''''''''''''''''Email Attachment'''''''''''''''''''''''''
                    foreach (string s in Attachment)
                    {
                        if (s != "")
                        {
                            mail.Attachments.Add(new Attachment(s));
                        }
                    }
                    mail.IsBodyHtml = true;
              

                        using (var client = new SmtpClient("smtp.office365.com"))
                        {
                            client.UseDefaultCredentials = false;
                            client.Port = SMPT_Port;
                            client.Credentials = new NetworkCredential("test@egss.ae", "T@egss25");
                            client.EnableSsl = true;

                            try
                            {
                                await client.SendMailAsync(mail); // Email sent
                            response["status"] = 1;
                            response["message"] = "Email sent Successfully";
                        }
                            catch (Exception e)
                            {
                                response["status"] = 0;
                                response["message"] = e.Message;
                            }
                        }
                    
                    //using (var mailClient = new SmtpClient("smtp.office365.com"))
                    //{
                    //    //  mailClient.Host = SMPT_Host;
                    //    mailClient.Port = SMPT_Port;
                    //    mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //    mailClient.UseDefaultCredentials = false;
                    //    mailClient.Credentials = new NetworkCredential(From, Password);
                    //    mailClient.EnableSsl = true;
                    //    mailClient.TargetName = "STARTTLS/smtp.office365.com";
                    //    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    //    await mailClient.SendMailAsync(mail); // Email sent
                    //    response["status"] = 1;
                    //    response["message"] = "Email sent Successfully";
                    //    return response;
                    //}
                }
            }
            catch (Exception ex)
            {
                response["status"] = 0;
                response["message"]= ex.Message;                                
                
            }
            return response;
        }
        #endregion

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


        public string GetScalarByQuery(string strQry, string CompId)
        {
            string result = "";
          
             string ConnStr=GetConnectionString(CompId);
            

            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            try
            {
                SqlCommand sqlCommand = new SqlCommand(strQry, sqlConnection);
                sqlConnection.Open();
                result = Convert.ToString(sqlCommand.ExecuteScalar());
            }
            catch (Exception err)
            {
               // ErrLog(err, "DevLib.GetScalarByQuery()");
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }

            return result;
        }

        public DataTable GetDataTableByQuery(string strQry, string CompId = "")
        {
        
            DataTable dataTable = new DataTable();
            string ConnStr=GetConnectionString(CompId);
            
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            try
            {
                SqlCommand sqlCommand = new SqlCommand(strQry, sqlConnection);
                int commandTimeout = 6000;
                sqlCommand.CommandTimeout = commandTimeout;
                sqlConnection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
               
                sqlDataAdapter.Fill(dataTable);
            
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

        #region F8API
        public class Focus8API
        {
            public static loginRes Login(string username, string password, string companycode, string sUrl)
            {

                bool status = false;
                loginRes lstResult = new loginRes();
               
                try
                {
                    List<Hashtable> datas = new List<Hashtable>();
                    Hashtable data = new Hashtable { { "Username", username }, { "password", password }, { "CompanyCode", companycode } };
                    datas.Add(data);
                    Hashtable login = new Hashtable { { "data", datas }, { "result", "1" }, { "message", "" } };
                    string url = sUrl + "/Login";
                   

                    string json = JsonConvert.SerializeObject(login);
                    using (var client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        client.Headers.Add("Content-Type", "application/json");
                        client.Timeout = 5 * 60 * 1000;
                        var response = client.UploadString(url, json);                        
                        lstResult = JsonConvert.DeserializeObject<loginRes>(response);
                         
                    }

                }
                catch (Exception ex)
                {
                    throw new CustomException("F8API.Login", ex);

                }

                return lstResult;
            }
            public static loginRes validateSession(string companycode,string sessionId, string sUrl)
            {

                bool status = false;
                loginRes lstResult = new loginRes();

                try
                {
                    List<Hashtable> datas = new List<Hashtable>();
                    Hashtable data = new Hashtable {  { "CompanyCode", companycode } };
                    datas.Add(data);
                    Hashtable login = new Hashtable { { "data", datas }, { "result", "1" }, { "message", "" } };
                    string url = sUrl + "/IsValidSession";


                    string json = JsonConvert.SerializeObject(login);
                    using (var client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        client.Headers.Add("Content-Type", "application/json");
                        client.Headers.Add("fSessionId", sessionId);
                        client.Timeout = 5 * 60 * 1000;
                        var response = client.UploadString(url, json);
                        lstResult = JsonConvert.DeserializeObject<loginRes>(response);

                    }

                }
                catch (Exception ex)
                {
                    throw new CustomException("F8API.Login", ex);

                }

                return lstResult;
            }

            public static loginRes GetLogOut(string AccessToken, string sUrl)
            {
                loginRes result = new loginRes();
                
                try
                {
                    string url = sUrl + "/Logout";
                    using (WebClient client = new WebClient())
                    {
                        client.Timeout = 5 * 60 * 1000;
                        client.Headers.Add("fSessionId", AccessToken);
                        client.Headers.Add("Content-Type", "application/json");
                        string ret = client.DownloadString(url);
                        result = JsonConvert.DeserializeObject<loginRes>(ret);
                      
                        
                    }
                }
                catch (Exception ex)
                {
                    throw new CustomException("F8API.Logout", ex);
                }
                return result;
            }
            public static string Post(string url, string data, string sessionId, ref string err)
            {                
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        webClient.Headers.Add("fSessionId", sessionId);
                        webClient.Headers.Add("Content-Type", "application/json");
                        webClient.Timeout = 300000;
                        string text = webClient.UploadString(url, data);
                        //devLibWeb.EventLog("response:" + Convert.ToString(text), "xdevlibApi");
                        return text;
                    };                   
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                    return null;
                }
            }

            //public static string DeleteVoucher(string VoucherName, string VoucherNo, string sessionId, ref string err, string baseUrl = "")
            //{
               
            //    try
            //    {
            //        string text = "";
               
            //        text = ((!string.IsNullOrEmpty(baseUrl)) ? (baseUrl + "/Transactions/" + VoucherName + "/" + VoucherNo) : (devLibWeb.urlVocDelete + VoucherName + "/" + VoucherNo));

            //        using (WebClientDel webClientDel = new WebClientDel())
            //        {
            //            webClientDel.Headers.Add("fSessionId", sessionId);
            //            webClientDel.Headers.Add("Content-Type", "application/json");
            //            string text2 = webClientDel.UploadString(text, "DELETE", "");
            //            F8API.PostResponse postResponse = JsonConvert.DeserializeObject<APIResponse.PostResponse>(text2);
            //            return text2;
            //        }
                   
            //    }
            //    catch (Exception ex)
            //    {
            //        err = ex.Message;
                  
            //        return null;
            //    }
            //}        
         

            public static string GetApi(string url, string sessionId, ref string err)
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        webClient.Headers.Add("fSessionId", sessionId);
                        webClient.Timeout = 300000;
                        return webClient.DownloadString(url);
                    }
                
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                    return null;
                }
            }

            //public static F8API.loginRes Login(string username, string password, string companycode, string sUrl)
            //{
            //    bool flag = false;
            //    F8API.loginRes result = new F8API.loginRes();
               
            //    try
            //    {
            //        List<Hashtable> list = new List<Hashtable>();
            //        Hashtable item = new Hashtable
            //{
            //    { "Username", username },
            //    { "password", password },
            //    { "CompanyCode", companycode }
            //};
            //        list.Add(item);
            //        Hashtable value = new Hashtable
            //{
            //    { "data", list },
            //    { "result", "1" },
            //    { "message", "" }
            //};
            //        string text = sUrl + "/Login";
                   
            //        string data = JsonConvert.SerializeObject(value);
            //        using (WebClient webClient = new WebClient())
            //        {
            //            webClient.Encoding = Encoding.UTF8;
            //            webClient.Headers.Add("Content-Type", "application/json");
            //            webClient.Timeout = 300000;
            //            string text2 = webClient.UploadString(text, data);
                       
            //            result = JsonConvert.DeserializeObject<loginRes>(text2);
            //            return result;
            //        }
                   
            //    }
            //    catch (Exception err)
            //    {
            //       // devLibWeb.ErrLog(err, "ClsAuth--Login()");
            //        flag = false;
            //    }

            //    return result;
            //}

            //public static F8API.loginRes GetLogOut(string AccessToken, string sUrl)
            //{
            //    loginRes loginRes2 = new loginRes();
              
            //    try
            //    {
            //        string address = sUrl + "/Logout";
            //        using (WebClient webClient = new WebClient())
            //        {
            //            webClient.Timeout = 300000;
            //            webClient.Headers.Add("fSessionId", AccessToken);
            //            webClient.Headers.Add("Content-Type", "application/json");
            //            string value = webClient.DownloadString(address);
            //            loginRes2 = JsonConvert.DeserializeObject<loginRes>(value);
            //            //devLibWeb.EventLog(" [GetLogOut] - Response: " + loginRes2);
            //            return loginRes2;
            //        }
                
            //    }
            //    catch (Exception ex)
            //    {
            //        //devLibWeb.ErrLog("ClsAuth--GetLogOut", ex.Message);
            //    }

            //    return loginRes2;
            //}

            public class WebClientDel : System.Net.WebClient
            {
                protected override WebRequest GetWebRequest(Uri uri)
                {
                    return base.GetWebRequest(uri);
                }
            }
            public class WebClient : System.Net.WebClient
            {
                public int Timeout { get; set; }

                protected override WebRequest GetWebRequest(Uri uri)
                {
                    WebRequest webRequest = base.GetWebRequest(uri);
                    webRequest.Timeout = Timeout;
                    ((HttpWebRequest)webRequest).ReadWriteTimeout = Timeout;
                    return webRequest;
                }
            }
        }
        #endregion
    }
}

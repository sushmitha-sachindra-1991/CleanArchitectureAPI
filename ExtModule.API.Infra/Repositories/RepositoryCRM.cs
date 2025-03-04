using ExtModule.API.Core;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ExtModule.API.Application.Interfaces;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace ExtModule.API.Infrastructure.Repositories
{
    public class RepositoryCRM : ICRMRepository
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

                        cmd.Parameters.AddWithValue("@" + s.Key, s.Value);
                    }
                }

                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                //ErrLog(ex, "DevLib.GetDataTableByStoredProcedure(" + spName + ")");
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
                //ErrLog(ex, "DevLib.GetDataTableByStoredProcedure(" + spName + ")");
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
                retval = Convert.ToString(sqlCommand.ExecuteScalar());
            }
            catch (Exception err)
            {
                //ErrLog(err, "ExtModule.Api.GetScalarByStoredProcedure(" + spName + ")");
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            return await Task.FromResult(retval);

        }
        public async Task<string> GetScalarByFunc(string FunctionName, string CompId)
        {
            var result = "";
            try
            {
                string text = "";
                text = "select dbo." + FunctionName;
                result = GetScalarByQuery(text, CompId);
            }
            catch (Exception err)
            {
                //ErrLog(err, "DevLib.GetTableNameOfTag()");
            }

            return await Task.FromResult(result);
        }
        public async Task<DataTable> GetMasterData(int iMasterTypeId, string[] Columns, string Condition, string CompId, string[] OrderColoumns)
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
                //ErrLog(ex, "DevLib.GetDataTableByStoredProcedure(" + spName + ")");
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
            catch (Exception err)
            {
                //ErrLog(err, "ExtModule.Api.GetScalarByStoredProcedure(" + spName + ")");
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            return await Task.FromResult(retval);
        }
        public async Task<Hashtable> CreateAndBulkInsertToTable(string TableName, List<Hashtable> lstParams, Hashtable coloumns, string CompId)
        {
            Hashtable retval = new Hashtable();
            string ConnStr = GetConnectionString(CompId);
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            sqlConnection.Open();
            SqlCommand sqlCommand = null;
            try
            {
                string query = "";

                //create table
                query = "Create Table " + TableName + " (";
                foreach (DictionaryEntry param in coloumns)
                {
                    query = query + param.Key.ToString() + " " + param.Value.ToString() + " , ";
                }
                query = query.TrimEnd(')');
                query = query + ")";
                sqlCommand = new SqlCommand(query, sqlConnection);

                sqlCommand.CommandType = CommandType.Text;

                int flag = (sqlCommand.ExecuteNonQuery());
                if (flag > 0)
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
                            query = @"insert into " + TableName;
                            string valueList = string.Join(", ", values.Select(item => $"'{item}'"));
                            query = query + "(" + string.Join(',', coulumnNames) + ") values (" + valueList + ");SELECT SCOPE_IDENTITY();";
                            sqlCommand = new SqlCommand(query, sqlConnection);

                            sqlCommand.CommandType = CommandType.Text;

                            retval["rows"] = Convert.ToString(sqlCommand.ExecuteScalar());
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
        public async Task<string> InsertToTable(string TableName, Hashtable Params, string CompId)
        {
            string retval = "";
            string ConnStr = GetConnectionString(CompId);
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            try
            {
                string query = "insert into " + TableName;
                string[] coulumnName = new string[Params.Count];
                string[] values = new string[Params.Count];
                if (Params != null)
                {
                    int i = 0;
                    foreach (DictionaryEntry Param in Params)
                    {
                        coulumnName[i] = Param.Key.ToString();
                        values[i] = Param.Value.ToString();

                    }
                }
                query = query + "(" + string.Join(',', coulumnName) + ") values (" + string.Join(',', values) + ")";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                sqlCommand.CommandType = CommandType.Text;
                sqlConnection.Open();
                retval = Convert.ToString(sqlCommand.ExecuteNonQuery());
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
                if (!string.IsNullOrEmpty(TableName))
                {

                    //delete
                    query = @"delete  " + TableName;

                    query = query + " where " + condition;
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
        public async Task<string> UpdateTable(string TableName, Hashtable Params, string condition, string CompId)
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

                    query = query + valueList + " where 1=1 and " + condition;
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
        public async Task<string> BulkInsertByStoredProcedure(string spName, List<Hashtable> Params, string CompId)
        {

            int rows = 0;
            string ConnStr = GetConnectionString(CompId);
            SqlCommand sqlCommand = null;
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            try
            {
                foreach (Hashtable s in Params)
                {
                    sqlCommand = new SqlCommand(spName, sqlConnection);
                    if (Params != null)
                    {
                        foreach (DictionaryEntry Param in s)
                        {
                            sqlCommand.Parameters.AddWithValue("@" + Param.Key, Param.Value);
                        }
                    }

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    rows = rows + (sqlCommand.ExecuteNonQuery());
                }

            }
            catch (Exception err)
            {
                //ErrLog(err, "ExtModule.Api.GetScalarByStoredProcedure(" + spName + ")");
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            return await Task.FromResult(rows.ToString());
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
        public async Task<DataTable> GetDataTableByView(string ViewName, string[] Columns, string Condition, string[] OrderColoumns, string CompId)
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
        public async Task<Hashtable> SendEmail(string From,string Password,string[] To, string[] Cc, string[] Bcc, string Subject, string Body, string[] Attachment, string SMPT_Host, int SMPT_Port, string CompId = "")
        {

            Hashtable response = new Hashtable();
            try
            {
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
                //GetCompany details
                //string sCompanyName = GetScalarByQuery("select sCompanyName from Focus8ERP.dbo.mcore_company where iCompanyId=" + CompId, CompId);
                //Hashtable obj = GetCompanyDetails(sCompanyName);
                //string From = (string)obj["FromEmailId"];
                //string Password = (string)obj["FromPassword"];
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

                MailMessage mail = new MailMessage();


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


                using (SmtpClient mailClient = new SmtpClient())
                {
                    mailClient.Host = SMPT_Host;
                    mailClient.Port = SMPT_Port;
                    mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailClient.UseDefaultCredentials = false;
                    mailClient.Credentials = new NetworkCredential(From, Password);
                    mailClient.EnableSsl = true;
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    mailClient.Send(mail);
                    response["status"] = 1;
                    response["message"] = "Email sent Successfully";
                    return response;
                }

            }
            catch (Exception ex)
            {
                response["status"] = 0;
                response["message"] = ex.Message;

            }
            return response;
        }
        #endregion
        #region Email Test
        public async Task<Hashtable> SendEmailTest(string CompId = "")
        {

            Hashtable response = new Hashtable();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                | SecurityProtocolType.Tls11
                                | SecurityProtocolType.Tls12;
            //test
            using (var message = new MailMessage())
            {
                message.To.Add(new MailAddress("sushmi46@gmail.com", "sushmita"));
                message.From = new MailAddress("infofocusextdevfocus@gmail.com", "info_focus_ext_dev");
                message.Subject = "My subject";
                message.Body = "My message";
                message.IsBodyHtml = false; // change to true if body msg is in html

                using (var client = new SmtpClient("smtp.gmail.com"))
                {
                    client.UseDefaultCredentials = false;
                    client.Port = 465;
                    client.Credentials = new NetworkCredential("infofocusextdevfocus@gmail.com", "focus@1234");
                    client.EnableSsl = true;

                    try
                    {
                        await client.SendMailAsync(message); // Email sent
                    }
                    catch (Exception e)
                    {
                        response["status"] = 0;
                        response["message"] = e.Message;
                    }
                }
            }
            return response;
        }
        #endregion
        #region Common functions
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
        public string GetConnectionString(string CompId)
        {
            string constring = "";
            try
            {
                CRM.ServerDetail objSer = GetCompanyServerDetailsById(CompId, "config", "ServerConfig");
                constring = "server=" + objSer.ServerName + ";Database=" + objSer.DatabaseName + " ;Integrated Security=false;User ID=" + objSer.UserID + ";password=" + objSer.password;
            }
            catch (Exception ex)
            {

            }
            return constring;
        }
        public string GetScalarByQuery(string strQry, string CompId)
        {
            string result = "";

            string ConnStr = GetConnectionString(CompId);


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
        public DataTable GetDataTableByQuery(string strQry, string CompId = "")
        {

            DataTable dataTable = new DataTable();
            string ConnStr = GetConnectionString(CompId);

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
        public CRM.ServerDetail GetCompanyServerDetailsById(string compId, string fileName, string rootNode)
        {
            CRM.ServerDetail serverDetail = new CRM.ServerDetail();
            try
            {
                if (!string.IsNullOrEmpty(compId))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                    xmlDocument.Load(directoryName + "\\XMLFiles\\" + fileName + ".xml");
                    foreach (XmlNode item in xmlDocument.SelectNodes(rootNode))
                    {
                        foreach (XmlNode item2 in item.SelectNodes("Company"))
                        {
                            string innerText = item2.SelectSingleNode("CompId").InnerText;
                            if (innerText == compId)
                            {
                                serverDetail.ServerName = item2.SelectSingleNode("ServerName").InnerText;
                                serverDetail.password = item2.SelectSingleNode("Password").InnerText;
                                serverDetail.DatabaseName = item2.SelectSingleNode("DatabaseName").InnerText;
                                serverDetail.UserID = item2.SelectSingleNode("UserID").InnerText;
                                return serverDetail;
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                // ErrLog(err, "GetCompanyServerDetails ");
            }

            return serverDetail;
        }
        #endregion
    }
}

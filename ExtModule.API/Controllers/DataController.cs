using Microsoft.AspNetCore.Mvc;
using ExtModule.API.Application.Factory;
using ExtModule.API.Core;
using System.Data;
using ExtModule.API.Model.DataResponse;
using ExtModule.API.Model.DataRequest;
using System.Collections;
using ExtModule.API.Core.ERP;
using ExtModule.API.Logging;
using log4net;
using log4net.Config;
using System.Reflection;


namespace ExtModule.API.Controllers
{
    [ApiController]    
    public class DataController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public DataController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
       
        }
        #region Test
        [HttpGet]
        [Route("api/Data")]
        public string[] Get()
        {
            string filename = "test";
             string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
            Logger.Instance.LogInfo(filename, "entered");
            return Summaries;
        }
        #endregion

        #region GetDataTableByStoredProcedure
        [HttpPost]
        [Route("api/{type}/Data/GetDataBySp")]
        public async Task<IActionResult> GetDataTableByStoredProcedure(DbCallStoredProcedureInput obj, string type)
        {
            var objRes = new APIResponse<DataTable>();
          string fileName= type + "_"+obj.CompId;
            string conString = "";
            if (obj != null)
            {
                try
                {                    
                    var repository = _repositoryFactory.CreateRepository(type);
                    DataTable dt = await repository.GetDataTableByStoredProcedure(obj.SPName, obj.Param, obj.CompId);

                    objRes.data = dt;
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogError(fileName,ex.InnerException.Message,ex);
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return Ok(objRes);

        }

        #endregion
        #region GetDataTableByView
        [HttpPost]
        [Route("api/{type}/Data/GetDataByView")]
        public async Task<IActionResult> GetDataTableByView(DbCallViewInput obj, string type)
        {
            var objRes = new APIResponse<DataTable>();
            string fileName = type + "_" + obj.CompId;
            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    DataTable dt = await repository.GetDataTableByView(obj.ViewName,obj.Columns,obj.Condition,obj.Ordercolumn,  obj.CompId);

                    objRes.data = dt;
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogError(fileName, ex.InnerException.Message, ex);
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return Ok(objRes);

        }

        #endregion
        #region GetDataTableByStoredProcedure
        [HttpPost]
        [Route("api/{type}/Data/GetDataBySpPageBreak")]
        public async Task<IActionResult> GetDataBySpPageBreak(DbCallStoredProcedureInputByPage obj, string type)
        {
            var objRes = new APIResponse<DataTable>();
            string fileName = type + "_" + obj.CompId;
            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    DataTable dt = await repository.GetDataTableByStoredProcedure(obj.SPName, obj.Param, obj.CompId);

                    objRes.data = dt;
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogError(fileName, ex.Message, ex);
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return Ok(objRes);

        }

        #endregion
        #region GetScalarBySP
        [HttpPost]
        [Route("api/{type}/Data/GetScalarBySP")]
        public async Task<APIResponse<string>> GetScalarBySP(DbCallStoredProcedureInput obj, string type)
        {
            var objRes = new APIResponse<string>();

            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    string val = await repository.GetScalarByStoredProcedure(obj.SPName, obj.Param, obj.CompId);

                    objRes.data = val;
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return objRes;

        }

        #endregion

        #region GetMasterData
        [HttpPost]
        [Route("api/{type}/Data/GetMasterData")]
        public async Task<IActionResult> GetMasterData(DbCallMasterInput obj, string type)
        {
            var objRes = new APIResponse<DataTable>();

            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    DataTable dt = await repository.GetMasterData(obj.MasterTypeId,obj.Columns,obj.Condition, obj.CompId,obj.Ordercolumn);

                    objRes.data = dt;
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return Ok(objRes);

        }

        #endregion

        #region InsertBySP
        [HttpPost]
        [Route("api/{type}/Data/InsertBySP")]
        public async Task<APIResponse<int>> InsertBySP(DbCallStoredProcedureInput obj, string type)
        {
            var objRes = new APIResponse<int>();

            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    string rows = await (repository.InsertByStoredProcedure(obj.SPName, obj.Param, obj.CompId));

                    objRes.data = int.Parse(rows);
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return objRes;

        }

        #endregion

        #region AddData
        [HttpPost]
        [Route("api/{type}/Data/AddData")]
        public async Task<APIResponse<int>> AddData(DbCallAddToTableInput obj, string type)
        {
            var objRes = new APIResponse<int>();

            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    string rows = await (repository.InsertToTable(obj.sTableName, obj.Param, obj.CompId));

                    objRes.data = int.Parse(rows);
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return objRes;

        }

        #endregion
        #region UpdateData
        [HttpPost]
        [Route("api/{type}/Data/UpdateData")]
        public async Task<APIResponse<int>> UpdateData(DbCallUpdateToTableInput obj, string type)
        {
            var objRes = new APIResponse<int>();

            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    string rows = await (repository.UpdateTable (obj.sTableName, obj.Param,obj.Condition, obj.CompId));

                    objRes.data = int.Parse(rows);
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return objRes;

        }

        #endregion
        #region DeleteData
        [HttpPost]
        [Route("api/{type}/Data/DeleteData")]
        public async Task<APIResponse<int>> DeleteData(DbCallDeleteTableInput obj, string type)
        {
            var objRes = new APIResponse<int>();

            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    string rows = await (repository.DeleteRowFromTable(obj.sTableName, obj.Condition, obj.CompId));

                    objRes.data = int.Parse(rows);
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return objRes;

        }

        #endregion
        #region BulkInsertBySP
        [HttpPost]
        [Route("api/{type}/Data/BulkInsertBySP")]
        public async Task<APIResponse<string>> BulkInsertBySP(DbCallBulkInputBySp obj, string type)
        {
            var objRes = new APIResponse<string>();

            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    string rows = await (repository.BulkInsertByStoredProcedure(obj.SPName, obj.Params, obj.CompId));

                    objRes.data = rows;
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return objRes;

        }

        #endregion
        #region BulkInsertByTable
        [HttpPost]
        [Route("api/{type}/Data/BulkInsertByTable")]
        public async Task<APIResponse<string>> BulkInsertByTable(DbCallBulkInputByTable obj, string type)
        {
            var objRes = new APIResponse<string>();

            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    string rows = await (repository.BulkInsertToTable (obj.TableName, obj.Params, obj.CompId));

                    objRes.data = rows;
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return objRes;

        }

        #endregion
        #region CreateAndBulkImportTable
        [HttpPost]
        [Route("api/{type}/Data/CreateAndBulkImportTable")]
        public async Task<APIResponse<string>> CreateAndBulkImportTable(DbCallBulkInputByTable obj, string type)
        {
            var objRes = new APIResponse<string>();

            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    Hashtable res = await (repository.CreateAndBulkInsertToTable(obj.TableName, obj.Params,obj.Coloumns, obj.CompId));

                    objRes.data = res["rows"].ToString();
                    objRes.status = 1;
                    objRes.sMessage = res["message"].ToString();
                }
                catch (Exception ex)
                {
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return objRes;

        }

        #endregion

        #region GetScalar
        [HttpPost]
        [Route("api/{type}/Data/GetScalar")]
        public async Task<IActionResult> GetScalar(DbCallStoredProcedureInput obj, string type)
        {
            var objRes = new APIResponse<string>();
            string fileName = type + "_" + obj.CompId;
            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    string res = await repository.GetScalarByStoredProcedure(obj.SPName, obj.Param, obj.CompId);

                    objRes.data = res;
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogError(fileName, ex.Message, ex);
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return Ok(objRes);

        }

        #endregion
        #region GetScalar
        [HttpPost]
        [Route("api/{type}/Data/DateToInt")]
        public async Task<IActionResult> DateToInt(DbCallDateFormatInput obj, string type)
        {
            var objRes = new APIResponse<string>();
            string fileName = type + "_" + obj.CompId;
            string conString = "";
            if (obj != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    string res = await repository.DateToInt(obj.date, obj.CompId);

                    objRes.data = res;
                    objRes.status = 1;
                    objRes.sMessage = "success";
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogError(fileName, ex.Message, ex);
                    objRes.status = 0;
                    objRes.sMessage = ex.Message;
                }
            }
            return Ok(objRes);

        }

        #endregion

    }
}

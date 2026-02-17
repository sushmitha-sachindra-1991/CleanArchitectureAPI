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
using Microsoft.AspNetCore.Authorization;
using ExtModule.API.Application.Interfaces;
using AutoMapper;



namespace ExtModule.API.Controllers
{
    [ApiController]    
    public class DataController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IERPRepository _repositoryERP;
        private readonly IMapper _mapper;
        public DataController(IRepositoryFactory repositoryFactory,IERPRepository eRPRepository,IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _repositoryERP = eRPRepository;
            _mapper=mapper;
       
        }
       

        #region GetDataTableByStoredProcedure
        /// <summary>
        /// Executes the specified stored procedure and returns the result as a DataTable.
        /// </summary>
        /// <param name="obj">The input parameters required to execute the stored procedure.</param>
        /// <param name="type">
        /// Identifies the functional module or data domain for the request.  
        /// This value determines which repository or data source is used internally.  
        /// For example, when the value is "ERP", the request is routed through the ERP-specific repository
        /// to execute the stored procedure against the ERP database.
        /// </param>

        /// <returns>An IActionResult containing the stored procedure execution result.</returns>
        [HttpPost]

        [Authorize]
        [Route("api/{type}/Data/GetDataBySp")]
        public async Task<IActionResult> GetDataTableByStoredProcedure(DbCallStoredProcedureInputDTO objDTO, string type)
        {
            var objRes = new APIResponse<DataTable>();
          string fileName= type + "_"+ objDTO.CompId;
            string conString = "";
            if (objDTO != null)
            {
                try
                {                    
                    var repository = _repositoryFactory.CreateRepository(type);
                    DbCallStoredProcedureInput obj=_mapper.Map<DbCallStoredProcedureInput>(objDTO);
                    DataTable dt = await repository.GetDataTableByStoredProcedureAsync(obj);

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

        #region GetDataTableListByStoredProcedure
        /// <summary>
        /// Executes the specified stored procedure and returns the result as a Dataset.
        /// </summary>
        /// <param name="obj">The input parameters required to execute the stored procedure.</param>
        /// <param name="type">
        /// Identifies the functional module or data domain for the request.  
        /// This value determines which repository or data source is used internally.  
        /// For example, when the value is "ERP", the request is routed through the ERP-specific repository
        /// to execute the stored procedure against the ERP database.
        /// </param>
        /// <returns>An IActionResult containing the stored procedure execution result.</returns>

        [HttpPost]
        [Authorize]
        [Route("api/{type}/Data/GetDataTableListBySp")]
        public async Task<IActionResult> GetDataTableListBySp(DbCallStoredProcedureInput objDTO, string type)
        {
            var objRes = new APIResponse<List<DataTable>>();
            string fileName = type + "_" + objDTO.CompId;
            string conString = "";
            if (objDTO != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    DbCallStoredProcedureInput obj = _mapper.Map<DbCallStoredProcedureInput>(objDTO);
                    List<DataTable> dt = await repository.GetMultipleResultSetsBySPAsync(obj);

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
        #region GetDataTableByView
        /// <summary>
        /// Executes the specified view and returns the result as a datatable.
        /// </summary>
        /// <param name="obj">The input parameters required to execute the view.</param>
        /// <param name="type">
        /// Identifies the functional module or data domain for the request.  
        /// This value determines which repository or data source is used internally.  
        /// For example, when the value is "ERP", the request is routed through the ERP-specific repository
        /// to execute the stored procedure against the ERP database.
        /// </param>
        /// <returns>An IActionResult containing the view execution result.</returns>
        [HttpPost]
        [Authorize]
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
                    DataTable dt = await repository.GetDataTableByViewAsync(obj.ViewName,obj.Columns,obj.Condition,obj.Ordercolumn,  obj.CompId);

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
        #region GetDataBySpPageBreak
        /// <summary>
        /// Executes the specified stored procedure and returns a paged result set
        /// based on the provided page number and page size.
        /// </summary>
        /// <param name="obj">
        /// The input payload containing the stored procedure name, parameters,
        /// page number, and page size used to retrieve a specific segment of data.
        /// </param>
        /// <param name="type">
        /// Identifies the functional module or data domain for the request.
        /// This value determines which repository or data source is used internally.
        /// For example, when set to "ERP", the stored procedure is executed against
        /// the ERP database through the ERP-specific repository.
        /// </param>
        /// <returns>
        /// An IActionResult containing the paged data returned by the stored procedure.
        /// </returns>
        [HttpPost]
        [Authorize]
        [Route("api/{type}/Data/GetDataBySpPageBreak")]
        public async Task<IActionResult> GetDataBySpPageBreak(DbCallStoredProcedureInputByPageDTO objDTO, string type)
        {
            var objRes = new APIResponse<DataTable>();
            string fileName = type + "_" + objDTO.CompId;
            string conString = "";
            if (objDTO != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    DbCallStoredProcedureInput obj = new DbCallStoredProcedureInput
                    {
                        Param = objDTO.Param,
                        CompId = objDTO.CompId,
                        SPName = objDTO.SPName,
                    };
                    DataTable dt = await repository.GetDataTableByStoredProcedureAsync(obj);

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
     
        #region GetDataTableByView
        /// <summary>
        /// Executes the specified stored procedure and returns the result as a string value.
        /// </summary>
        /// <param name="obj">The input parameters required to execute the stored procedure.</param>
        /// <param name="type">
        /// Identifies the functional module or data domain for the request.  
        /// This value determines which repository or data source is used internally.  
        /// For example, when the value is "ERP", the request is routed through the ERP-specific repository
        /// to execute the stored procedure against the ERP database.
        /// </param>
        /// <returns>An IActionResult containing the stored procedure execution result.</returns>
        [HttpPost]
        [Authorize]
        [Route("api/{type}/Data/GetScalarBySP")]
        public async Task<APIResponse<string>> GetScalarBySP(DbCallStoredProcedureInputDTO objDTO, string type)
        {
            var objRes = new APIResponse<string>();

            string conString = "";
            if (objDTO != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    DbCallStoredProcedureInput obj = _mapper.Map<DbCallStoredProcedureInput>(objDTO);
                    string val = await repository.GetScalarByStoredProcedureAsync(obj);

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
        /// <summary>
        /// Returns a datatable of master data for the master type id passed.
        /// </summary>
        /// <param name="obj">The input parameters required to retrieve the master data.</param>
        /// <param name="type">
        /// Identifies the functional module or data domain for the request.  
        /// This value determines which repository or data source is used internally.  
        /// For example, when the value is "ERP", the request is routed through the ERP-specific repository
        /// to execute the stored procedure against the ERP database.
        /// </param>
        /// <returns>An IActionResult containing the result.</returns>
        [HttpPost]
        [Authorize]
        [Route("api/{type}/Data/GetMasterData")]
        public async Task<IActionResult> GetMasterData(DbCallMasterInputDTO objDTO, string type)
        {
            var objRes = new APIResponse<DataTable>();

            string conString = "";
            if (objDTO != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    DbCallMasterInput obj=_mapper.Map< DbCallMasterInput>(objDTO);
                    DataTable dt = await repository.GetMasterDataAsync(obj);

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


        #region AddData
        /// <summary>
        /// Adds the single row to table based on the input passed
        /// </summary>
        /// <param name="obj">Includes the tablename,compid,and collection of column name and value</param>
        ///<param name="type">
        /// Identifies the functional module or data domain for the request.  
        /// This value determines which repository or data source is used internally.  
        /// For example, when the value is "ERP", the request is routed through the ERP-specific repository
        /// to execute the stored procedure against the ERP database.
        /// </param>
        /// <returns>
        /// An <see cref="APIResponse{T}"/> containing:
        /// - <c>status</c>: the success or failure code of the insert operation  
        /// - <c>sMessage</c>: a descriptive message about the result  
        /// - <c>data</c>: an integer representing the ID or affected row count returned by the insert  
        /// </returns>

        [HttpPost]
        [Authorize]
        [Route("api/{type}/Data/AddData")]
        public async Task<APIResponse<int>> AddData(DbCallAddToTableInputDTO objDTO, string type)
        {
            var objRes = new APIResponse<int>();

            string conString = "";
            if (objDTO != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    DbCallAddToTableInput obj = _mapper.Map<DbCallAddToTableInput>(objDTO);
                    string rows = await (repository.InsertToTableAsync(obj));

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
        /// <summary>
        /// Updates an existing record in the specified module or data domain
        /// using the provided table name, parameters, and update condition.
        /// </summary>
        /// <param name="obj">
        /// The input payload containing the table name, company identifier,
        /// key–value parameters to update, and the condition used to identify
        /// the target record(s).  
        /// - <c>sTableName</c>: The name of the table to update  
        /// - <c>CompId</c>: The company or tenant identifier  
        /// - <c>Param</c>: A hashtable of column names and their new values  
        /// - <c>Condition</c>: The WHERE clause or filter determining which rows to update  
        /// </param>
        /// <param name="type">
        /// Identifies the functional module or data domain for the request.  
        /// This value determines which repository or data source is used internally.  
        /// For example, when set to <c>"ERP"</c>, the update operation is executed
        /// against the ERP database through the ERP-specific repository.
        /// </param>
        /// <returns>
        /// An <see cref="APIResponse{T}"/> containing:  
        /// - <c>status</c>: the success or failure code of the update operation  
        /// - <c>sMessage</c>: a descriptive message about the result  
        /// - <c>data</c>: an integer representing the number of affected rows  
        /// </returns>
        [HttpPost]
        [Authorize]
        [Route("api/{type}/Data/UpdateData")]
        public async Task<APIResponse<int>> UpdateData(DbCallUpdateToTableInputDTO objDTO, string type)
        {
            var objRes = new APIResponse<int>();

            string conString = "";
            if (objDTO != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    DbCallUpdateToTableInput obj=_mapper.Map<DbCallUpdateToTableInput>(objDTO);
                    string rows = await (repository.UpdateTableAsync (obj));

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
        /// <summary>
        /// Delete an existing record in the specified module or data domain
        /// using the provided table name, parameters, and update condition.
        /// </summary>
        /// <param name="obj">
        /// The input payload containing the table name, company identifier,
        /// to delete, and the condition used to identify
        /// the target record(s).  
        /// - <c>sTableName</c>: The name of the table to update  
        /// - <c>CompId</c>: The company or tenant identifier  
        /// - <c>Condition</c>: The WHERE clause or filter determining which rows to update  
        /// </param>
        /// <param name="type">
        /// Identifies the functional module or data domain for the request.  
        /// This value determines which repository or data source is used internally.  
        /// For example, when set to <c>"ERP"</c>, the update operation is executed
        /// against the ERP database through the ERP-specific repository.
        /// </param>
        /// <returns>
        /// An <see cref="APIResponse{T}"/> containing:  
        /// - <c>status</c>: the success or failure code of the update operation  
        /// - <c>sMessage</c>: a descriptive message about the result  
        /// - <c>data</c>: an integer representing the number of affected rows  
        /// </returns>
        [HttpPost]
        [Authorize]
        [Route("api/{type}/Data/DeleteData")]
        public async Task<APIResponse<int>> DeleteData(DbCallDeleteTableInputDTO objDTO, string type)
        {
            var objRes = new APIResponse<int>();

            string conString = "";
            if (objDTO != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    DbCallDeleteTableInput obj=_mapper.Map<DbCallDeleteTableInput>(objDTO);
                    string rows = await (repository.DeleteRowFromTableAsync(obj));

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

        #region BulkInsertByTable
        /// <summary>
        /// Adds the bulk row to table based on the input passed
        /// </summary>
        /// <param name="obj">Includes the tablename,compid,and collection of column name and value</param>
        ///<param name="type">
        /// Identifies the functional module or data domain for the request.  
        /// This value determines which repository or data source is used internally.  
        /// For example, when the value is "ERP", the request is routed through the ERP-specific repository
        /// to execute the stored procedure against the ERP database.
        /// </param>
        /// <returns>
        /// An <see cref="APIResponse{T}"/> containing:
        /// - <c>status</c>: the success or failure code of the insert operation  
        /// - <c>sMessage</c>: a descriptive message about the result  
        /// - <c>data</c>: an integer representing the ID or affected row count returned by the insert  
        /// </returns>
        [HttpPost]
        [Authorize]
        [Route("api/{type}/Data/BulkInsertByTable")]
        public async Task<APIResponse<string>> BulkInsertByTable(DbCallBulkInputByTableDTO objDTO, string type)
        {
            var objRes = new APIResponse<string>();

            string conString = "";
            if (objDTO != null)
            {
                try
                {
                    var repository = _repositoryFactory.CreateRepository(type);
                    DbCallBulkInputByTable obj = _mapper.Map<DbCallBulkInputByTable>(objDTO);
                    string rows = await (repository.BulkInsertToTableAsync (obj));

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
      
    }
}

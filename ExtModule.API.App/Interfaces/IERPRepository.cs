using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtModule.API.Core;
using ExtModule.API.Core.ERP;
using static ExtModule.API.Core.ERP.F8API;

namespace ExtModule.API.Application.Interfaces
{
    public interface IERPRepository : IRepository
    {
        Task<FocusPostResponse<F8API.PostResponse>> InterCompanyPostingByStoredProcedureVoucherWise(string spName, Hashtable Param, string SessionId, string CompId);
        Task<FocusPostResponse<F8API.PostResponse>> PostToJVByStoredProcedureVoucherWise(string spName, Hashtable Param, string SessionId, string CompId,string baseFocusAPIUrl);
        Task<FocusPostResponse<F8API.PostResponse>> DeleteVoucher(string VoucherName, string VoucherNo, string sessionId, string baseFocusAPIUrl = "");
        Task<loginRes> Login(string username, string password, string companycode, string sUrl);
        Task<loginRes> validateFocusSessionId(string sesessionId, string companycode, string sUrl);
        Task<loginRes> LogOut(string sessionId, string sUrl);
        Task<Hashtable> GetItemImage(string compId, int productId);

    }
}

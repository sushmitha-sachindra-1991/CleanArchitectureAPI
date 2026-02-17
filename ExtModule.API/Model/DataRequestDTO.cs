
using System.Collections;
using System.Text.Json.Serialization;

namespace ExtModule.API.Model.DataRequest
{
    
    public class DbCallStoredProcedureInputDTO
    {
        public string SPName { get; set; }
        public string CompId { get; set; }              
        public Hashtable Param { get; set; }
    }
    public class DbCallViewInputDTO
    {
        public string ViewName { get; set; }
        public string[] Columns { get; set; }//coulm seperated by comma 
        public string Condition { get; set; }
        public string CompId { get; set; }
        public string[]? Ordercolumn { get; set; } = default;
    }
    public class DbCallScalarFunctionInputDTO
    {
        public string FuncName { get; set; }
       
        public string CompId { get; set; }
       
    }
    public class DbCallTableFunctionInputDTO
    {
        public string FuncName { get; set; }
        public string[] Columns { get; set; }//coulm seperated by comma
        public string? Condition { get; set; } = default;
        public string CompId { get; set; }

    }
    public class DbCallAddToTableInputDTO
    {
        public string sTableName { get; set; }
        public string CompId { get; set; }
        public Hashtable Param { get; set; }
    }
    public class DbCallUpdateToTableInputDTO
    {
        public string sTableName { get; set; }
        public string CompId { get; set; }
        public Hashtable Param { get; set; }
        public string Condition { get; set; }
    }
    public class DbCallDeleteTableInputDTO
    {
        public string sTableName { get; set; }
        public string CompId { get; set; }
       
        public string Condition { get; set; }
    }
    public class DbCallStoredProcedureInputByPageDTO
    {
        public string SPName { get; set; }
        public string CompId { get; set; }
        public Hashtable Param { get; set; }
        public  int PageNumber {  get; set; }
        public int PageSize {  get; set; }
    }
    public class DbCallDateFormatInputDTO
    {
        public string date { get; set; }
        public string CompId { get; set; }
      
    }
    /// <summary>
    /// Represents the input required to retrieve or manage item images
    /// associated with a specific product .
    /// </summary>
    public class DbCallItemImage
    {
        public int iProductId { get; set; }
        public string compId { get; set; }

    }
    public class DbCallMasterInputDTO
    {
        public int MasterTypeId { get; set; }
        public string[] Columns { get; set; }//coulm seperated by comma
        public string Condition { get; set; }
        public string CompId { get; set; }
        public string[]? Ordercolumn { get; set; }=default ;

    }
    public class DbCallBulkInputBySp
    {
        public string SPName { get; set; }
        public string CompId { get; set; }
        public List<Hashtable> Params { get; set; }
    }
    public class DbCallBulkInputByTableDTO
    {
        public string? TableName { get; set; } = default;
        public string CompId { get; set; }
        public Hashtable? Coloumns { get; set; } = default;
        public List<Hashtable> Params { get; set; }
    }
    public class FocusAPIInputBySP
    {
        public string SPName { get; set; }
        public string CompId { get; set; }
        public string SessionId { get; set; }
        public Hashtable Param { get; set; }
    }
    public class DeleteVoucher
    {
        public string iVoucherType { get; set; }
        public string sVoucherNo { get; set; }
        public string SessionId { get; set; }
        public string CompId { get; set; }

    }
    public class SendEmailInput
    {
        public string sFrom { get; set; }
        public string sPassword { get; set; }
        public string[] sTo { get; set; }
        public string[] sCC { get; set; }
        public string[] sBCC { get; set; }
        public string sSubject { get; set; }
        public string sBody { get; set; }
        public string[] sAttachments { get; set; }
        public string SMPT_Host { get; set; }
        public int SMPT_Port { get; set; }
        public string CompId { get; set; }
    }
    public class SendEmailInputTest
    {
   
        public string CompId { get; set; }
    }
}

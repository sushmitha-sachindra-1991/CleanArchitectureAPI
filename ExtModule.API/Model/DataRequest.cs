
using System.Collections;
using System.Text.Json.Serialization;

namespace ExtModule.API.Model.DataRequest
{
    
    public class DbCallStoredProcedureInput
    {
        public string SPName { get; set; }
        public string CompId { get; set; }              
        public Hashtable Param { get; set; }
    }
    public class DbCallViewInput
    {
        public string ViewName { get; set; }
        public string[] Columns { get; set; }//coulm seperated by comma 
        public string Condition { get; set; }
        public string CompId { get; set; }
        public string[]? Ordercolumn { get; set; } = default;
    }
    public class DbCallAddToTableInput
    {
        public string sTableName { get; set; }
        public string CompId { get; set; }
        public Hashtable Param { get; set; }
    }
    public class DbCallUpdateToTableInput
    {
        public string sTableName { get; set; }
        public string CompId { get; set; }
        public Hashtable Param { get; set; }
        public string Condition { get; set; }
    }
    public class DbCallDeleteTableInput
    {
        public string sTableName { get; set; }
        public string CompId { get; set; }
       
        public string Condition { get; set; }
    }
    public class DbCallStoredProcedureInputByPage
    {
        public string SPName { get; set; }
        public string CompId { get; set; }
        public Hashtable Param { get; set; }
        public  int PageNumber {  get; set; }
        public int PageSize {  get; set; }
    }
    public class DbCallDateFormatInput
    {
        public string date { get; set; }
        public string CompId { get; set; }
      
    }
    public class DbCallMasterInput
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
    public class DbCallBulkInputByTable
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
}

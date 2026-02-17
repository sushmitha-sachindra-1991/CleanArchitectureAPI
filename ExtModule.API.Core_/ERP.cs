using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExtModule.API.Core.ERP
{
    public static class F8API
    {


    
        public class LoginRes
        {
            public List<Data> Data { get; set; }

            public int Result { get; set; }

            public string Message { get; set; }

            public string Url { get; set; }
        }
        public class PostResponse
        {
            public string Url { get; set; }

            public List<Hashtable> Data { get; set; }

            public int Result { get; set; }

            public string Message { get; set; }
        }
        public class Data
        {
            public int AltLanguageId { get; set; }

            public int EmployeeId { get; set; }

            public string FSessionId { get; set; }
        }
        public class PostingData
        {
            public List<Hashtable> Data { get; set; }

            public PostingData()
            {
                Data = new List<Hashtable>();
            }
        }
    }
    public class FocusPostResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T F8APIPost { get; set; }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtlMiriDemo_V1.VerifyIdRes
    {
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Root
        {
        public string MiriAccountNumberBase31 { get; set; }
        public string MiriAccountNumberBase21 { get; set; }
        public string MiriAccountNumberBase10 { get; set; }
        public string MiriNumber { get; set; }
        public string TransactionDate { get; set; }
        public string UserProfileNumber { get; set; }
        public ResponseData ResponseData { get; set; }
        public string AccountFirstName { get; set; }
        public string AccountLastName { get; set; }
        public string AccountMiddleName { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        }

    public class ResponseData
        {
         
        public int ResponseData1 { get; set; }
        public int ResponseData2 { get; set; }
        }
    }
    

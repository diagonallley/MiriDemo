using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtlMiriDemo_V1.TokenValidationRes
    {
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Root
        {
        public string MiriAccountNumberBase31;
        public string MiriAccountNumberBase21;
        public string MiriAccountNumberBase10;
        public string MiriNumber;
        public string TransactionDate;
        public string UserProfileNumber;
        public string StatusCode;
        public string StatusMessage;
        }
    }

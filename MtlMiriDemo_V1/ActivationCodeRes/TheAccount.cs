using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtlMiriDemo_V1.ActivationCodeRes { 
    public class TheAccount
        {
        public int Issuer { get; set; }
        public int ProfileId { get; set; }
        public string MiriProfileNumber { get; set; }
        public string MiriAccountNumber { get; set; }
        public int MiriAccountNumberB10 { get; set; }
        public string MiriAccountNumberB21 { get; set; }
        public string NameOnCard { get; set; }
        public string AccountActive { get; set; }
        public string ActivationStatus { get; set; }
        public DateTime AccountIssueDate { get; set; }
        public DateTime AccountExpireDate { get; set; }
        public int OnePeriodSeconds { get; set; }
        public string FraudAlert { get; set; }
        public int FraudAttempts { get; set; }
        public string FieldOneTwoText { get; set; }
        public string AccountActivationCode { get; set; }
        public int SecurityLevel { get; set; }
        public string DeviceId { get; set; }
        public int DisplayUserName { get; set; }
        public int DisplayCompanyName { get; set; }
        public int DisplayUserPhoto { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime AccountCreatedDate { get; set; }
        public DateTime AccountActiveDate { get; set; }
        public string AccountCvv { get; set; }
        }
    }

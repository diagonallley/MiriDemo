using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtlMiriDemo_V1.AccountRes
    {
    public class TheAccountView
        {
        public int Issuer { get; set; }
        public int ProfileId { get; set; }
        public string MiriProfileNumber { get; set; }
        public string MiriAccountNumber { get; set; }
        public string MiriAccountNumberB10 { get; set; }
        public string MiriAccountNumberB21 { get; set; }
        public string NameOnCard { get; set; }
        public string AccountActive { get; set; }
        public string ActivationStatus { get; set; }
        public string AccountIssueDate { get; set; }
        public string AccountExpireDate { get; set; }
        public string OnePeriodSeconds { get; set; }
        public string FraudAlert { get; set; }
        public string FraudAttempts { get; set; }
        public string FieldOneTwoText { get; set; }
        public string AccountActivationCode { get; set; }
        public string SecurityLevel { get; set; }
        public string DeviceId { get; set; }
        public string DisplayUserName { get; set; }
        public string DisplayCompanyName { get; set; }
        public string DisplayUserPhoto { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string AccountCreatedDate { get; set; }
        public string AccountActiveDate { get; set; }
        public string AccountCvv { get; set; }
        }
    }

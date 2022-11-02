using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Newtonsoft;
using System.Data.SqlClient;
using MTLMiriLib;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Specialized;
using static MTLMiriLib.MiriHelper;
using System.Text;
using System.Net.Http;
using System.Configuration;

namespace MtlMiriDemo_V1.Controllers
    {
    public class LoginController : Controller
        {
        // GET: Login
        public ActionResult Index()
            {
            return View();
            }

        public ActionResult SignOut()
            {
            Session.Abandon();
            Session.Clear();
            Response.Cookies.Clear();
            Session.RemoveAll();
            Session["userid"] = null;
            return RedirectToAction("Index", "Login");

            }
        // [OutputCache(NoStore = true, Duration = 0)]
        private bool ValidateAdminUser(string userid)
            {
            //userid = "12345";
            try {
                string empid = "";
                string name = "";
                string designation = "";
                string email = "";
                string mobile = "";
                string usertype = "";
               // SqlHelper sql = new SqlHelper();
               demoClass.SqlHelper sql = new demoClass.SqlHelper();
                // DataSet ds = sql.GetDatasetByCommand("get_employee");
                sql.AddParameterToSQLCommand("@userid", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@userid", userid);


              //  DataSet ds = new demoClass().GetDatasetByCommand("Users_Validate");
                DataSet ds = sql.GetDatasetByCommand("Users_Validate");

                int tables = ds.Tables.Count;
                Thread.Sleep(10000);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 1)
                    {
                    userid = ds.Tables[0].Rows[0]["userid"].ToString().Trim();
                    name = ds.Tables[0].Rows[0]["name"].ToString().Trim();
                    designation = ds.Tables[0].Rows[0]["designation"].ToString().Trim();
                    email = ds.Tables[0].Rows[0]["email"].ToString().Trim();
                    mobile = ds.Tables[0].Rows[0]["mobile"].ToString().Trim();
                    usertype = ds.Tables[0].Rows[0]["usertype"].ToString().Trim();
                    Session["name"] = name;
                    Session["designation"] = designation;
                    Session["email"] = email;
                    Session["mobile"] = mobile;
                    Session["userid"] = userid;
                    Session["usertype"] = usertype;
                    return true;
                    }
                else
                    {
                    return false;
                    }
                }
            catch (Exception ex)
                {

                ExceptionLogging.SendExcepToDB(ex);
                return false;
                }

            }
        public string ValidateAdminMiriID(string id)
            {
            try
                {
                System.Threading.Thread.Sleep(1000);

                string validationurl = ConfigHelper.GetValidationDomain();

                //MiriHelper miri = new MiriHelper("https://mmauth.com");
                MiriHelper miri = new MiriHelper(validationurl);
                id = id.Replace("-", "");
                id = id.Replace(" ", "");

                MiriIdTransactionResult res = new demoClass().MiriIdTransactionRest(id);





                LogApiResponse(res.ResponseData1, res.AccountFirstName, res.AccountLastName, res.MiriAccountNumberB31, res.StatusCode, res.StatusMessage);

                if (res != null && res.StatusCode == "0")
                    {
                    string accountnumber = res.MiriAccountNumberB31;
                    string responsedata1 = res.ResponseData1;
                    if (responsedata1 == null)
                        {
                        responsedata1 = "12345";
                        }
                    Session["userid"] = responsedata1.ToString();

                    if (ValidateAdminUser(responsedata1.ToString()))
                        {
                        return "000:Valid User";
                        }
                    /*  if (true)
                          {
                          return "000:Valid User";
                          }*/
                    else
                        {
                        return "001:INVALID:User not found";
                        }
                    }

                else if (res != null && res.StatusCode == "536")

                    {
                    return "004:Fraud Alert";
                    }

                else
                    {
                    return "002:INVALID:Invalid MIRI ID";
                    }
                }
            catch (Exception ex)
                {

                ExceptionLogging.SendExcepToDB(ex);
                return "003:INVALID:Database Connection Issue" + ex.Message;


                }

            }

        public void LogApiResponse(string Userid, string FirstName, string LastName, string AccountNumber, string StatusCode, string StatusMessage)
            {
            try
                {
                SqlHelper sql = new SqlHelper();
                sql.AddParameterToSQLCommand("@UserID", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@UserID", Userid);
                sql.AddParameterToSQLCommand("@UserName", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@UserName", FirstName);
                sql.AddParameterToSQLCommand("@UserLastName", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@UserLastName", LastName);
                sql.AddParameterToSQLCommand("@MiriAccountNumber", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@MiriAccountNumber", AccountNumber);
                sql.AddParameterToSQLCommand("@StatusCode", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@StatusCode", StatusCode);
                sql.AddParameterToSQLCommand("@StatusMessage", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@StatusMessage", StatusMessage);
                int r = sql.GetExecuteNonQueryByCommand("LogApiLogin");
                }
            catch (Exception ex)
                {
                ExceptionLogging.SendExcepToDB(ex);
                }
            }
        }

    public class demoClass
        {

        public Root RegisterUser(dynamic req)
            {
            string applianceurl = "https://qtebt.manipaltechnologies.com/";
            //string uriString = applianceurl + "/MIRISYSTEMS/Services/MiriUserManagementServices/UserManagementService.svc/rest/RegisterUser";

            string uriString = applianceurl + "MIRISYSTEMS/MIS/Account/Create";

       // https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Account/RegisterUser
            RestHelper restHelper = new RestHelper();
            restHelper.applianceurl = applianceurl;
            string value = JsonConvert.SerializeObject(req);

            string value2 = restHelper.AuthPost(new Uri(uriString), value);

            var Account= JsonConvert.DeserializeObject<Root>(value2);
            return Account;

          




            }

        public static dynamic GetActivationCode(Root root)

            {
            //Get Activation Code
            string applianceurl = "https://qtebt.manipaltechnologies.com/";

            string activation = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Account/Create/ActivationRecord";
            RestHelper restact = new RestHelper();
            restact.applianceurl = applianceurl;

            var accReq = new
                {
                mode = 0,
                key = 0,
                req = new
                    {
                    MiriAccountNumber = root.resp.TheAccountView.MiriAccountNumber
                    }
                };

            var ActivationReqString = JsonConvert.SerializeObject(accReq);
            string ActivationCodeRes = restact.AuthPost(new Uri(activation), ActivationReqString);
            var Account = JsonConvert.DeserializeObject(ActivationCodeRes);

            return Account;



            }

        public static dynamic CreateProfileAccountGetActivationCode(string auth, RegisterRequest regOb)
            {

            string Profileurl = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Profile/Create";
            string AccountUrl = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Account/Create";
            ProfileReq.Root ProfileReq = new ProfileReq.Root();

            ProfileReq.key = "0";
            ProfileReq.mode = "0";



            ProfileReq.Req request = new ProfileReq.Req();

            ProfileReq.TheProfile theProfile = new ProfileReq.TheProfile();
            /* theProfile.Issuer = 14;
             theProfile.ProfileExpireDate = "07/05/2023";*/


            theProfile.Issuer = Convert.ToInt32(regOb.IssuerNumber);
            theProfile.ProfileExpireDate = regOb.ExpiryDate;
           
            theProfile.LookUpData1 = regOb.LookUpDataOne;
            theProfile.LookUpData2 = regOb.LookUpDataTwo;
            theProfile.FirstName= regOb.FirstName;
            theProfile.LastName = regOb.LastName;
            theProfile.ResponseData1 = regOb.ResponseDataOne;
            theProfile.ResponseData2 = regOb.LastName;

            request.TheProfile = theProfile;

            ProfileReq.req = request;

            var jsonString = JsonConvert.SerializeObject(ProfileReq);



            var root = JsonConvert.DeserializeObject<Root>(jsonString);
            //Console.WriteLine(jsonString);


            string Profile = Request.CreatePostReq(Profileurl, jsonString);


            ProfileRes.Root Profile_ = JsonConvert.DeserializeObject<ProfileRes.Root>(Profile);

            AccountReq.TheAccount account = new AccountReq.TheAccount();

            account.Issuer = Convert.ToInt32(regOb.IssuerNumber);
            account.ProfileId = Profile_.resp.TheProfile.ProfileId;
            account.AccountCvv = regOb.Cvv;
            account.FieldOneTwoText = $"[{regOb.FirstName.ToLower()}]{regOb.LastName.ToLower()}";
            AccountReq.Req areq = new AccountReq.Req();
            areq.theAccount = account;


            AccountReq.Root AccountRequest = new AccountReq.Root();
            AccountRequest.key = "0";
            AccountRequest.mode = "0";


            AccountRequest.req = areq;

            string accountRequestString = JsonConvert.SerializeObject(AccountRequest);








            string jsonStringAccountReq = JsonConvert.SerializeObject(accountRequestString);
            string AccountResponse = Request.CreatePostReq(AccountUrl, accountRequestString);
            AccountRes.Root Account = JsonConvert.DeserializeObject<AccountRes.Root>(AccountResponse);


            AccountActivation.Root AccountActivation = new AccountActivation.Root();
            AccountActivation.key = "0";
            AccountActivation.mode = "0";
            AccountActivation.Req AccountActivationReq = new AccountActivation.Req();
            AccountActivationReq.MiriAccountNumber = Account.resp.TheAccountView.MiriAccountNumber;
            AccountActivation.req = AccountActivationReq;

            string AccountActivationJsonString = JsonConvert.SerializeObject(AccountActivation);
            //string AccountUrl = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Account/Create";
            string ActivationURL = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Account/Create/ActivationRecord";
            string ActivationCode = Request.CreatePostReq(ActivationURL, AccountActivationJsonString);
            ActivationCodeRes.Root activationCodeResponse = JsonConvert.DeserializeObject<ActivationCodeRes.Root>(ActivationCode);

            return activationCodeResponse;
            /*Console.WriteLine(activationCodeResponse.resp.TheAccount.AccountActivationCode);
            Console.WriteLine("  ...");*/
            }
        public static dynamic CreateProfileAccountGetActivationCode(string auth)
            {

            string Profileurl = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Profile/Create";
            string AccountUrl = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Account/Create";
            ProfileReq.Root ProfileReq = new ProfileReq.Root();

            ProfileReq.key = "0";
            ProfileReq.mode = "0";



            ProfileReq.Req request = new ProfileReq.Req();

            ProfileReq.TheProfile theProfile = new ProfileReq.TheProfile();

            theProfile.Issuer = 14;
            theProfile.ProfileExpireDate = "07/05/2023";
            /*  theProfile.AccountEmail = "thishtat@gmail.com";
              theProfile.PhoneNumber = "1111223456";

              theProfile.LookUpData1 = "111123444";
              theProfile.LookUpData2 = "129103";*/

            request.TheProfile = theProfile;

            ProfileReq.req = request;

            var jsonString = JsonConvert.SerializeObject(ProfileReq);



            var root = JsonConvert.DeserializeObject<Root>(jsonString);
            //Console.WriteLine(jsonString);


            string Profile = Request.CreatePostReq(Profileurl, jsonString);

            ProfileRes.Root Profile_ = JsonConvert.DeserializeObject<ProfileRes.Root>(Profile);

            AccountReq.TheAccount account = new AccountReq.TheAccount();
            account.Issuer = 14;
            account.ProfileId = Profile_.resp.TheProfile.ProfileId;

            AccountReq.Req areq = new AccountReq.Req();
            areq.theAccount = account;


            AccountReq.Root AccountRequest = new AccountReq.Root();
            AccountRequest.key = "0";
            AccountRequest.mode = "0";


            AccountRequest.req = areq;

            string accountRequestString = JsonConvert.SerializeObject(AccountRequest);








            string jsonStringAccountReq = JsonConvert.SerializeObject(accountRequestString);
            string AccountResponse = Request.CreatePostReq(AccountUrl, accountRequestString);
            AccountRes.Root Account = JsonConvert.DeserializeObject<AccountRes.Root>(AccountResponse);


            AccountActivation.Root AccountActivation = new AccountActivation.Root();
            AccountActivation.key = "0";
            AccountActivation.mode = "0";
            AccountActivation.Req AccountActivationReq = new AccountActivation.Req();
            AccountActivationReq.MiriAccountNumber = Account.resp.TheAccountView.MiriAccountNumber;
            AccountActivation.req = AccountActivationReq;

            string AccountActivationJsonString = JsonConvert.SerializeObject(AccountActivation);
            //string AccountUrl = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Account/Create";
            string ActivationURL = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Account/Create/ActivationRecord";
            string ActivationCode = Request.CreatePostReq(ActivationURL, AccountActivationJsonString);
            ActivationCodeRes.Root activationCodeResponse = JsonConvert.DeserializeObject<ActivationCodeRes.Root>(ActivationCode);

            return activationCodeResponse;
            /*Console.WriteLine(activationCodeResponse.resp.TheAccount.AccountActivationCode);
            Console.WriteLine("  ...");*/
            }
        public class SqlHelper
            {
            public enum ExpectedType
                {
                StringType,
                NumberType,
                DateType,
                BooleanType,
                ImageType
                }

            public static string ConnectionString;

            private SqlConnection objSqlConnection;

            private SqlCommand objSqlCommand;

            private int CommandTimeout = 30;

            public static string LocalSqlIP;

            public static string LocalSqlDb;

            public static string constr;

            private static string SyncDbIP;

            public static string SyncDbName;

            public SqlHelper()
                {
                try
                    {
                    ConnectionString = ConfigHelper.GetConnectionString();
                    objSqlConnection = new SqlConnection(ConnectionString);
                    objSqlCommand = new SqlCommand();
                    objSqlCommand.Parameters.Clear();
                    objSqlCommand.CommandTimeout = CommandTimeout;
                    objSqlCommand.Connection = objSqlConnection;
                    }
                catch (Exception ex)
                    {
                    throw new Exception("Error initializing data class." + Environment.NewLine + ex.Message);
                    }
                }

            public void Dispose()
                {
                try
                    {
                    if (objSqlConnection != null)
                        {
                        if (objSqlConnection.State != 0)
                            {
                            objSqlConnection.Close();
                            }

                        objSqlConnection.Dispose();
                        }

                    if (objSqlCommand != null)
                        {
                        objSqlCommand.Dispose();
                        }
                    }
                catch (Exception ex)
                    {
                    throw new Exception("Error disposing data class." + Environment.NewLine + ex.Message);
                    }
                }

            public void OpenConnection()
                {
                if (objSqlConnection.State != ConnectionState.Open)
                    {
                    objSqlConnection.Open();
                    }
                }

            public void ClearParameters()
                {
                objSqlCommand.Parameters.Clear();
                }

            public void CloseConnection()
                {
                if (objSqlConnection.State != 0)
                    {
                    objSqlConnection.Close();
                    }
                }

            public object GetExecuteScalarByCommand(string Command)
                {
                object obj = 0;
                try
                    {
                    objSqlCommand.CommandText = Command;
                    objSqlCommand.CommandTimeout = CommandTimeout;
                    objSqlCommand.CommandType = CommandType.StoredProcedure;
                    OpenConnection();
                    objSqlCommand.Connection = objSqlConnection;
                    obj = objSqlCommand.ExecuteScalar();
                    CloseConnection();
                    }
                catch (Exception ex)
                    {
                    CloseConnection();
                    throw ex;
                    }

                return obj;
                }

            public int GetExecuteNonQueryByCommand(string Command)
                {
                try
                    {
                    int num = 0;
                    objSqlCommand.CommandText = Command;
                    objSqlCommand.CommandTimeout = CommandTimeout;
                    objSqlCommand.CommandType = CommandType.StoredProcedure;
                    OpenConnection();
                    objSqlCommand.Connection = objSqlConnection;
                    num = objSqlCommand.ExecuteNonQuery();
                    CloseConnection();
                    return num;
                    }
                catch (Exception ex)
                    {
                    CloseConnection();
                    throw ex;
                    }
                }

            public int GetExecuteNonQueryBySQL(string query)
                {
                int num = 0;
                try
                    {
                    objSqlCommand.CommandText = query;
                    objSqlCommand.CommandTimeout = CommandTimeout;
                    objSqlCommand.CommandType = CommandType.Text;
                    OpenConnection();
                    objSqlCommand.Connection = objSqlConnection;
                    num = objSqlCommand.ExecuteNonQuery();
                    CloseConnection();
                    return num;
                    }
                catch (Exception ex)
                    {
                    CloseConnection();
                    throw ex;
                    }
                }

            public DataSet GetDatasetByCommand(string Command)
                {
                try
                    {
                    objSqlCommand.CommandText = Command;
                    objSqlCommand.CommandTimeout = CommandTimeout;
                    objSqlCommand.CommandType = CommandType.StoredProcedure;
                    objSqlConnection.Open();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(objSqlCommand);
                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);
                    return dataSet;
                    }
                catch (Exception ex)
                    {
                    throw ex;
                    }
                finally
                    {
                    CloseConnection();
                    }
                }

            public string GetDataStored(string Command)
                {
                /*  objSqlCommand.CommandText = Command;
                  objSqlCommand.CommandTimeout = CommandTimeout;
                  objSqlCommand.CommandType = CommandType.StoredProcedure;
                  objSqlConnection.Open();
                  objSqlCommand.Parameters.AddWithValue("@hash", "010002B");*/
                SqlConnection sqlConnection = new SqlConnection();
                SqlCommand command = new SqlCommand();
                sqlConnection.ConnectionString = "Data source=.;Database=demo;Trusted_Connection=true;MultipleActiveResultSets=true;";
                command.Connection = sqlConnection;
                command.CommandTimeout = CommandTimeout;
                command.CommandText = Command;
                command.CommandType=System.Data.CommandType.StoredProcedure;
                sqlConnection.Open();
                command.Parameters.AddWithValue("@hash", "010002B");
                SqlDataReader reader=command.ExecuteReader();
                string str=string.Empty;
                while (reader.Read())
                    {
                    str = reader[0].ToString();
                    }
                return str;
                }
            public DataSet GetDatasetBySQL(string query)
                {
                try
                    {
                    objSqlCommand.CommandText = query;
                    objSqlCommand.CommandTimeout = CommandTimeout;
                    objSqlCommand.CommandType = CommandType.Text;
                    objSqlConnection.Open();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(objSqlCommand);
                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);
                    return dataSet;
                    }
                catch (Exception ex)
                    {
                    throw ex;
                    }
                finally
                    {
                    CloseConnection();
                    }
                }

            public SqlDataReader GetReaderBySQL(string strSQL)
                {
                objSqlConnection.Open();
                try
                    {
                    SqlCommand sqlCommand = new SqlCommand(strSQL, objSqlConnection);
                    return sqlCommand.ExecuteReader();
                    }
                catch (Exception ex)
                    {
                    CloseConnection();
                    throw ex;
                    }
                }

            public SqlDataReader GetReaderByCmd(string Command)
                {
                SqlDataReader sqlDataReader = null;
                try
                    {
                    objSqlCommand.CommandText = Command;
                    objSqlCommand.CommandType = CommandType.StoredProcedure;
                    objSqlCommand.CommandTimeout = CommandTimeout;
                    objSqlConnection.Open();
                    objSqlCommand.Connection = objSqlConnection;
                    return objSqlCommand.ExecuteReader();
                    }
                catch (Exception ex)
                    {
                    CloseConnection();
                    throw ex;
                    }
                }

            public void AddParameterToSQLCommand(string ParameterName, SqlDbType ParameterType)
                {
                try
                    {
                    SqlParameter value = new SqlParameter(ParameterName, ParameterType);
                    objSqlCommand.Parameters.Add(value);
                    }
                catch (Exception ex)
                    {
                    throw ex;
                    }
                }

            public void AddParameterToSQLCommand(string ParameterName, SqlDbType ParameterType, int ParameterSize)
                {
                try
                    {
                    objSqlCommand.Parameters.Add(new SqlParameter(ParameterName, ParameterType, ParameterSize));
                    }
                catch (Exception ex)
                    {
                    throw ex;
                    }
                }

            public object GetSQLCommandParameterValue(string ParameterName)
                {
                try
                    {
                    return objSqlCommand.Parameters[ParameterName].Value;
                    }
                catch (Exception ex)
                    {
                    throw ex;
                    }
                }

            public void SetSQLCommandParameterValue(string ParameterName, object Value)
                {
                try
                    {
                    objSqlCommand.Parameters[ParameterName].Value = Value;
                    }
                catch (Exception ex)
                    {
                    throw ex;
                    }
                }
            }

        
        public MiriCardTransactionRestResponse MiriCardTransactionRest(string nameoncard, string cardnumber, string month, string year, string cvv, string transactionid = "123", string merchantid = "merchant123")
            {
            transactionid = "112";
            merchantid = "122";
            string uriString = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/SERVICES/MIServiceDecoder/Verify/MiriCard/"+transactionid + "/" + month + "/" + year + "/" + nameoncard + "/" + cardnumber + "/" + merchantid;
            //https://qtebt.manipaltechnologies.com/MIRISYSTEMS/Services/MIServiceDecoder/Verify/MiriCard/111/08/24/EHKFjxlq/4567895547519447/123
            //[Route("Verify/MiriCard/{TransactionID}/{CardMonth}/{CardYear}/{NameOnCard}/{NumberToVerify}/{MerchantID}")]

            //MIRISYSTEMS/Services/MIServiceDecoder/Verify/MiriCard
            //string uriString = applianceurl + "/MIRISYSTEMS/Services/MiriDecoderServices/MiriDecoderService.svc/Rest/MiriCardTransaction/" + transactionid + "/" + month + "/" + year + "/" + nameoncard + "/" + cardnumber + "/" + merchantid;

            RestHelper restHelper = new RestHelper();
            string value = restHelper.Get(new Uri(uriString));

            MiriCardTransactionRestResponse miriCardTransactionRestResponse = JsonConvert.DeserializeObject<MiriCardTransactionRestResponse>(value);
            /*miriCardTransactionRestResponse.UserFields;*/

            MiriCardTransactionRestResponse miriCardTransactionRestResponse2 = new MiriCardTransactionRestResponse();
            miriCardTransactionRestResponse2.StatusCode = miriCardTransactionRestResponse.StatusCode;
            miriCardTransactionRestResponse2.StatusMessage = miriCardTransactionRestResponse.StatusMessage;
            miriCardTransactionRestResponse2.TransactionDate = miriCardTransactionRestResponse.TransactionDate;
            miriCardTransactionRestResponse2.TransactionNumber = miriCardTransactionRestResponse.TransactionNumber;
            miriCardTransactionRestResponse2.MiriAccountNumberB31 = miriCardTransactionRestResponse.MiriAccountNumberB31;
            miriCardTransactionRestResponse2.UserProfileNumber = miriCardTransactionRestResponse.UserProfileNumber;
            miriCardTransactionRestResponse2.NameOnCard = miriCardTransactionRestResponse.NameOnCard;
            dynamic userFields = miriCardTransactionRestResponse.UserFields;
            if (userFields != null)
                {
                miriCardTransactionRestResponse2.ResponseData2 = userFields[0].Value;
                miriCardTransactionRestResponse2.ResponseData3 = userFields[1].Value;
                }

            if (miriCardTransactionRestResponse.ResponseData != null)
                {
                miriCardTransactionRestResponse2.ResponseData1 = miriCardTransactionRestResponse.ResponseData.ResponseData1;
                miriCardTransactionRestResponse2.ResponseData2 = miriCardTransactionRestResponse.ResponseData.ResponseData2;
                miriCardTransactionRestResponse2.ResponseData3 = miriCardTransactionRestResponse.ResponseData.ResponseData3;
                }

            return miriCardTransactionRestResponse2;
            }
        public static dynamic CreateRequest(string url = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/Services/MIServiceDecoder/Verify/MiriId/")
            {
            //url = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/Services/MIServiceDecoder/Verify/MiriId/";


            var wb = new WebClient();
            var data = new NameValueCollection();
            //string url = "www.example.com"
            data["NumberToVerify"] = "010002B367487961";
//            data["password"] = "myPassword";

            var response = wb.UploadValues(url, "POST", data);

            //Thread.Sleep(5000);
           
            try
                {

              return response;
                }
            catch (WebException e)
                {
                return e.Message;
                }
            }

    


        public MiriIdTransactionResult MiriIdTransactionRest(string MiriId)
            {
            try
                {
                //string applianceurl = "https://mmcpayment.com/";
                string applianceurl = "https://qtebt.manipaltechnologies.com/";

                
                //String url = applianceurl + @"MIRISYSTEMS/Services/MIServiceDecoder/Verify/MiriId/" + MiriId;

                //String url = applianceurl + @"/MIRISYSTEMS/Services/MiriDecoderServices/MiriDecoderService.svc/Rest/MiriIDTransaction/" + MiriId;

                /*var response=CreateRequest(url);

              
                RestHelper helper = new RestHelper();
                helper.applianceurl = applianceurl;



                string responses = helper.Get(new Uri(url));
*/
                String url = applianceurl + @"MIRISYSTEMS/Services/MIServiceDecoder/Verify/MiriId/";


                //string response = CreateGetReq(url, MiriId);
                string response = CreateGetRequest(url, MiriId);


                VerifyIdRes.Root verifyRes = JsonConvert.DeserializeObject<VerifyIdRes.Root>(response);
                string userProfileNumber=verifyRes.UserProfileNumber;

                string GetResponseDataUrl = applianceurl + @"MIRISYSTEMS/Services/MIServiceDecoder/Profile/Get/";
                #region Send Req

                #endregion



                MiriIdTransactionRestResponse obj = JsonConvert.DeserializeObject<MiriIdTransactionRestResponse>(response);

                /*   MiriIdTransactionRestResponse obj= new MiriIdTransactionRestResponse();
                   obj.StatusMessage = "Success";
                   obj.StatusCode = "0";
                   obj.AccountMiddleName = "";
                  */
               /* obj.ResponseData1 = "dummy";
                
                obj.ResponseData2 = "dummy2";*/
                //010002B437990612

                MiriIdTransactionResult res = new MiriIdTransactionResult();
                res.StatusCode = obj.StatusCode;
                res.StatusMessage = obj.StatusMessage;

                if (res.StatusCode.Trim() == "0")

                    {
                    res.AccountFirstName = obj.AccountFirstName;
                    res.AccountLastName = obj.AccountLastName;
                    res.MiriAccountNumberB31 = obj.MiriAccountNumberBase31;
                    //res.MiriAccountNumberB31 = obj.MiriAccountNumberBase31;
                    // res.ResponseData1 = obj.ResponseData1;
                    res.ResponseData1 = obj.ResponseData1;
                    res.ResponseData2 = obj.ResponseData2;
                  /*  res.ResponseData1 = "12345";
                    res.ResponseData2 = "54321";*/
                    //  res.ResponseData = obj.ResponseData;
                    dynamic userfields = obj.UserFields;
                    if (userfields != null)
                        {
                        res.ResponseData2 = userfields[0].Value;
                        res.ResponseData3 = userfields[1].Value;
                        }

                    // ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(obj.ResponseData);


                    if (obj.ResponseData != null)
                        {
                        res.ResponseData1 = obj.ResponseData.ResponseData1;
                        res.ResponseData2 = obj.ResponseData.ResponseData2;
                        res.ResponseData3 = obj.ResponseData.ResponseData3;
                        }

                    //   res.ResponseData2 = obj.UserFields.ResponseData2;
                    //   res.ResponseData3 = obj.UserFields.ResponseData3;
                    }



                return res;
                }
            catch (Exception ex)
                {
                return null;
                }

            }
        
        public static dynamic CreateGetReq(string url, string MiriId)
            {
            string html = string.Empty;
            //string miriId = "010002B429387412";
            //string url = @"https://qtebt.manipaltechnologies.com/MIRISYSTEMS/Services/MIServiceDecoder/Verify/MiriId/" + miriId;
            url = url + MiriId;
            try
                {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

               /* System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;*/

                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                    {
                    html = reader.ReadToEnd();
                    }
                return html;
                }
            catch (Exception e)
                {
                return e.Message;
                }

            }

        public static dynamic CreateGetRequest(string url, string MiriId)
            {

            url = url + MiriId;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = WebRequest.Create(url);
            request.Method = "GET";

            try {

                var webResponse = request.GetResponse();
                var webStream = webResponse.GetResponseStream();

                var reader = new StreamReader(webStream);
                var data = reader.ReadToEnd();
                return data;

                }
            catch (Exception e) { return e.Message; }

            }


        public DataSet GetDatasetByCommand(string Command)
            {
            try
                {
                SqlConnection ObjSqlConnection = new SqlConnection();
                SqlCommand objSqlCommand = new SqlCommand();
                /* ObjSqlConnection.ConnectionString = "Data Source = tcp:172.17.3.12; Initial Catalog = MIRISYSTEMS_V304; Persist Security Info = True; User ID = mirisystems3010; Password = Manipal@3010;";*/
                //ObjSqlConnection.ConnectionString = "Data source=.;Database=demo;Trusted_Connection=true;MultipleActiveResultSets=true;";
                ObjSqlConnection.ConnectionString = ConfigurationManager.AppSettings["CONSTR"];


                objSqlCommand.Connection = ObjSqlConnection;
                objSqlCommand.CommandText = Command;
               /* objSqlCommand.CommandTimeout = CommandTimeout;*/
                objSqlCommand.CommandType = CommandType.StoredProcedure;
                ObjSqlConnection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(objSqlCommand);
                DataSet dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet);
                return dataSet;
                }
            catch (Exception ex)
                {
                throw ex;
                }
            finally
                {
                //CloseConnection();

                }
            }


        public MiriIdTransactionResult MiriIdTransactionRest_(string MiriId)
            {
            try
                {
                string applianceurl = "";
                string uriString = "https://qtebt.manipaltechnologies.com" + "/MIRISYSTEMS/Services/MIServiceDecoder/Verify/MiriId" + MiriId;
                RestHelper restHelper = new RestHelper();
                restHelper.applianceurl = applianceurl;
                string value = restHelper.Get(new Uri(uriString));
                MiriIdTransactionRestResponse miriIdTransactionRestResponse = JsonConvert.DeserializeObject<MiriIdTransactionRestResponse>(value);
                MiriIdTransactionResult miriIdTransactionResult = new MiriIdTransactionResult();
                miriIdTransactionResult.StatusCode = miriIdTransactionRestResponse.StatusCode;
                miriIdTransactionResult.StatusMessage = miriIdTransactionRestResponse.StatusMessage;
                if (miriIdTransactionResult.StatusCode.Trim() == "0")
                    {
                    miriIdTransactionResult.AccountFirstName = miriIdTransactionRestResponse.AccountFirstName;
                    miriIdTransactionResult.AccountLastName = miriIdTransactionRestResponse.AccountLastName;
                    miriIdTransactionResult.MiriAccountNumberB31 = miriIdTransactionRestResponse.MiriAccountNumberB31;
                    miriIdTransactionResult.ResponseData1 = miriIdTransactionRestResponse.ResponseData1;
                    dynamic userFields = miriIdTransactionRestResponse.UserFields;
                    if (userFields != null)
                        {
                        miriIdTransactionResult.ResponseData2 = userFields[0].Value;
                        miriIdTransactionResult.ResponseData3 = userFields[1].Value;
                        }

                    if (miriIdTransactionRestResponse.ResponseData != null)
                        {
                        miriIdTransactionResult.ResponseData1 = miriIdTransactionRestResponse.ResponseData.ResponseData1;
                        miriIdTransactionResult.ResponseData2 = miriIdTransactionRestResponse.ResponseData.ResponseData2;
                        miriIdTransactionResult.ResponseData3 = miriIdTransactionRestResponse.ResponseData.ResponseData3;
                        }
                    }

                return miriIdTransactionResult;
                }
            catch (Exception)
                {
                return null;
                }
            }


      

        }

    public class MiriIdTransactionResult
        {
        public string StatusCode;
        public string StatusMessage;
        public string AccountBin;
        public string AccountFirstName;
        public string AccountLastName;
        public string AccountMiddleName;
        public string MiriAccountNumberB31;
        public string MiriAccountNumberBase31;
        public string TransactionDate;
        public string TransactionNumber;
        public string UserProfileNumber;
        public string ResponseData1;
        public string ResponseData2;
        public string ResponseData3;

       /* public static explicit operator MiriIdTransactionResult(MiriIdTransactionResult v)
            {
            throw new NotImplementedException();
            }*/
        }

    public class MiriIdTransactionRestResponse
        {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string AccountBin { get; set; }
        public string AccountDisplayMonth { get; set; }
        public string AccountDisplayYear { get; set; }
        public string AccountFirstName { get; set; }
        public string AccountLastName { get; set; }
        public string AccountMiddleName { get; set; }
        public string AccountSwapData { get; set; }
        public string MerchantID { get; set; }
        public string MiriAccountNumberB10 { get; set; }
        public string MiriAccountNumberB21 { get; set; }
        public string MiriAccountNumberB31 { get; set; }

        public string MiriAccountNumberBase21 { get; set; }

        public string MiriAccountNumberBase10 { get; set; }


        public string MiriAccountNumberBase31 { get; set; }
        public string MiriDynamicNumber { get; set; }
        public string NameOnCard { get; set; }
        public string ResponseData1 { get; set; }

        public string ResponseData2 { get; set; }

        public string ResponseData3 { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionNumber { get; set; }
        public object UserFields { get; set; }

        public ResponseData ResponseData { get; set; }
        public string UserProfileNumber { get; set; }
        }


    public class ResponseData
        {
        public string ResponseData1;
        public string ResponseData2;
        public string ResponseData3;
        }
    }

public class RestHelper
    {
    public string applianceurl = "https://mmauth.com";

    public string Post(Uri url, string value)
        {
        WebRequest webRequest = WebRequest.Create(url);
        byte[] bytes = Encoding.ASCII.GetBytes(value);
        webRequest.ContentType = "application/json";
        webRequest.Method = "POST";

        try
            {
            using (Stream stream = webRequest.GetRequestStream())
                {
                stream.Write(bytes, 0, bytes.Length);
                }

            HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
            return new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd();
            }
        catch (WebException)
            {
            return null;
            }
        }

    public dynamic PostAccountCreate(Uri url, string value)
        {
        WebRequest webRequest = WebRequest.Create(url);
        byte[] bytes = Encoding.ASCII.GetBytes(value);
        webRequest.ContentType = "application/json";
        webRequest.Method = "POST";
        webRequest.Headers.Add("Authorization", "276958340159160438727923510846062345");
        try
            {
            using (Stream stream = webRequest.GetRequestStream())
                {
                stream.Write(bytes, 0, bytes.Length);
                }

            HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
            return new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd();
            }
        catch (WebException)
            {
            return null;
            }
        }

    public string AuthPost(Uri url, string value)
        {
        WebRequest webRequest = WebRequest.Create(url);
        byte[] bytes = Encoding.ASCII.GetBytes(value);
        webRequest.ContentType = "application/json";
        webRequest.Method = "POST";
        webRequest.Headers.Add("Authorization", "850426971328046917536784290513064195");
        try
            {
            using (Stream stream = webRequest.GetRequestStream())
                {
                stream.Write(bytes, 0, bytes.Length);
                }

            HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
            return new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd();
            }
        catch (WebException)
            {
            return null;
            }
        }
    public string Get(Uri url)
        {
        WebRequest webRequest = WebRequest.Create(url);
        webRequest.ContentType = "application/json";
        webRequest.Method = "GET";
        try
            {
            HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
            return new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd();
            }
        catch (WebException)
            {
            return null;
            }
        }
    }

/*public class LogInTicketReq
    {
    public int Mode { get; set; }
    public int Key { get; set; }

    }*/
public class RegisterHelper
    {
    public RegisterResponse RegisterUser(RegisterRequest req)
        {
        string applianceurl = "https://mmcpayment.com/";

        //string uriString = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Account/RegisterUser";

        string uriString = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Account/Create/Activate";

        //string uriString = applianceurl + "/MIRISYSTEMS/Services/MiriUserManagementServices/UserManagementService.svc/rest/RegisterUser";
        RestHelper restHelper = new RestHelper();
        //restHelper.applianceurl = applianceurl;
        restHelper.applianceurl = uriString;
        string value = JsonConvert.SerializeObject(req);

        string value2 = restHelper.AuthPost(new Uri(uriString), value);
        //string value2 = restHelper.Post(new Uri(uriString), value);
        return JsonConvert.DeserializeObject<RegisterResponse>(value2);
        }

    public string RegisterAndActivateUser()
        {
      
      

        string ProfileUrl = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Profile/Create";

        var ProfileCreate = new
            {
            mode = "0",
            key = "0",
            req = new
                {
                TheProfile = new
                    {
                    Issuer = 14,
                    ProfileExpireDate = "07/05/2023",
                    FirstName = "qqa",
                    LastName = "rty",
                    Address1 = "EBT",
                    Address2 = "EBT 2",
                    City = "Mumbai",
                    State = "Mahrashtra",
                    Country = "India",
                    ZipCode = "400093",
                    AccountEmail = "abc.xyz@mtldemo.com",
                    PhoneNumber = "8954657852",
                    LookUpData1 = "33333",
                    LookUpData2 = "44444",
                    ResponseData1 = "12233"
                    }
                }
            };

        string profileReq = JsonConvert.SerializeObject(ProfileCreate);



        var request = WebRequest.Create(ProfileUrl);
        request.Method = "POST";

        
        
        byte[] byteArray = Encoding.UTF8.GetBytes(profileReq);

        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteArray.Length;

        var reqStream = request.GetRequestStream();
        reqStream.Write(byteArray, 0, byteArray.Length);

         var response = request.GetResponse();
        Console.WriteLine(((HttpWebResponse)response).StatusDescription);

         var respStream = response.GetResponseStream();

         var reader = new StreamReader(respStream);
        string data = reader.ReadToEnd();
        Console.WriteLine(data);

       









        string str = "";
        return str;
        }


    public static dynamic CreateProfile(Object profileCreate)
        {
        string ProfileUrl = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/MIS/Profile/Create";

        if (profileCreate == null)
            {
            profileCreate = new
                {
                mode = "0",
                key = "0",
                req = new
                    {
                    TheProfile = new
                        {
                        Issuer = 14,
                        ProfileExpireDate = "07/05/2023",
                        FirstName = "qqa",
                        LastName = "rty",
                        Address1 = "EBT",
                        Address2 = "EBT 2",
                        City = "Mumbai",
                        State = "Mahrashtra",
                        Country = "India",
                        ZipCode = "400093",
                        AccountEmail = "abc.x1yz@mtldemo.com",
                        PhoneNumber = "1235467852",
                        LookUpData1 = "33213",
                        LookUpData2 = "42244",
                        ResponseData1 = "12233"
                        }
                    }
                };
            }
        

        string profileReq = JsonConvert.SerializeObject(profileCreate);


        WebRequest webRequest = WebRequest.Create(ProfileUrl);
        byte[] bytes = Encoding.ASCII.GetBytes(profileReq);
        webRequest.ContentType = "application/json";
        webRequest.Method = "POST";
        webRequest.Headers.Add("Authorization", "276958340159160438727923510846062345");

        try
            {
            using (Stream stream = webRequest.GetRequestStream())
                {
                stream.Write(bytes, 0, bytes.Length);
                }

            HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
            return new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd();
            }
        catch (WebException)
            {
            return null;
            }













        }

   // public static dynamic CreateAccountAndActivate(Object CreateObject  )




    }

public class Resp
    {
    public TheAccountView TheAccountView { get; set; }
    public string StatusCode { get; set; }
    public string StatusMessage { get; set; }
    }

public class Root
    {
    public string mode { get; set; }
    public string key { get; set; }
    public Resp resp { get; set; }
    }

public class TheAccountView
    {
    public int Issuer { get; set; }
    public int ProfileId { get; set; }
    public string MiriProfileNumber { get; set; }
    public string MiriAccountNumber { get; set; }
    public string AccountCvv { get; set; }
    public string AccountActive { get; set; }
    public string ActivationStatus { get; set; }
    public DateTime AccountIssueDate { get; set; }
    public DateTime AccountExpireDate { get; set; }
    public string FraudAlert { get; set; }
    public int FraudAttempts { get; set; }
    public int SecurityLevel { get; set; }
    public string FieldOneTwoText { get; set; }
    public string AccountActivationCode { get; set; }
    public string DeviceId { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime AccountCreatedDate { get; set; }
    public DateTime AccountActiveDate { get; set; }
    public int DisplayUserName { get; set; }
    public int DisplayCompanyName { get; set; }
    public int DisplayUserPhoto { get; set; }
    public int MiriAccountNumberB10 { get; set; }
    public string MiriAccountNumberB21 { get; set; }
    public string NameOnCard { get; set; }
    public int OnePeriodSeconds { get; set; }
    }

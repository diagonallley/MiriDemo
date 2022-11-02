using MTLMiriLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;
using static MTLMiriLib.MiriHelper;


namespace MtlMiriDemo_V1.Controllers
{
    public class AllKartPaymentController : Controller
    {
        // GET: AllKartPayment
        public ActionResult Index()
        {
            if (Session["MerchantId"]!=null)
            {
                Session["Amt"] = "69955.00";
            }
            else
            {
                return RedirectToAction("Index", "AllKart");
            }

            return View();
        }

        [HttpPost]
        public string GenerateRandomNo()
        {
            int _min = 100;
            int _max = 999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max).ToString();
        }
        private string Transaction(string CardType, string CardBrand)
        {
            try
            {
                demoClass.SqlHelper sql = new demoClass.SqlHelper();
            sql.AddParameterToSQLCommand("@cardtype", SqlDbType.VarChar);
            sql.SetSQLCommandParameterValue("@cardtype", CardType);
            sql.AddParameterToSQLCommand("@brandtype", SqlDbType.VarChar);
            sql.SetSQLCommandParameterValue("@brandtype", CardBrand);
            SqlDataReader r = sql.GetReaderByCmd("generate_transactionid");
            if (r.Read())
            {
                string trn = r[0].ToString();
                return trn;
            }
            return "";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendExcepToDB(ex);
                return "";
            }
        }




        private MemoryStream GenerateMyQCCode(string QCText)
        {
            try
            {

                var QCwriter = new BarcodeWriter();
                QCwriter.Format = BarcodeFormat.QR_CODE;

                QCwriter.Options = new EncodingOptions
                {
                    Height = 200,
                    Width = 200
                };

                QCwriter.Options.Margin = 1;

                var result = QCwriter.Write(QCText);
                //  string path = Server.MapPath("~/images/MyQRImage.jpg");
                var barcodeBitmap = new Bitmap(result);
                Image image;
                using (MemoryStream memory = new MemoryStream())
                {

                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    //  fs.Write(bytes, 0, bytes.Length);

                    image = Image.FromStream(memory);
                    return memory;
                }

                // lblQRText.Text =   
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendExcepToDB(ex);
                return null;
            }
        }

        public FileContentResult ReceiptQRCode(string id)
        {
            //Session["TransactionID"] = "TRN" + GenerateRandomNo(10000, 99999);
            //Session["TransactionDate"] = DateTime.Now.ToString();
            //Session["Amt"] = GenerateRandomNo(1000, 8000);
            //Session["merchantid"] = merchant[Convert.ToInt32(GenerateRandomNo(0, 5))];
            string tranasctionid = Session["TransactionID"].ToString();
            string transactiondate = Session["TransactionDate"].ToString();
            string transactionamt = Session["Amt"].ToString();
            string cardno = Session["CardNo"].ToString();
            //    string cardholdername = Session["test_fieldone"].ToString() + " " + Session["test_fieldtwo"].ToString();
            string merchantid = Session["Merchantid"].ToString();
            MemoryStream memoryStream = GenerateMyQCCode(tranasctionid + "," + transactiondate.ToLower() + "," + transactionamt.ToLower() + "," + cardno.ToLower() + "," + merchantid);
            byte[] fileContents = memoryStream.ToArray();
            return new FileContentResult(fileContents, "image/jpg");
        }
        public string GenerateRandomNo(int min, int max)
        {
            int _min = min;
            int _max = max;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max).ToString();
        }
        [HttpPost]

        public ActionResult CardValidation(string firstname, string lastname, string cardNumber, string month, string year, string cvv, string cardtypevalue, string CardBrand = "M")
        {
            try
            {
                string domain = ConfigHelper.GetDomain();
                MiriHelper miri = new MiriHelper(domain);
                MiriCardTransactionRestResponse req = new MiriCardTransactionRestResponse();
                //string Issuer = "";
                string merchant = "ALLKART";//Session["MerchantId"].ToString();
                string nameoncard = firstname + lastname;
                // year = year.Substring(2, 2);
                string Sessionid = Session.SessionID;
                string IpAddress = Request.UserHostAddress;
                string Browser = Request.UserAgent;
                // string CardType = Session["CardType"].ToString();
                // string CardBrand = Session["CardBrand"].ToString();
                Session["CardType"] = cardtypevalue;
                string TransactionId = Transaction(cardtypevalue, CardBrand);
                Session["TransactionID"] = TransactionId;

                Session["TransactionDate"] = DateTime.Now.ToString();
                string Amt = "69,955"; //Session["Amt"].ToString();


                string masked = cardNumber.Substring(0, 4) + "-XXXX-XXXX-" + cardNumber.Substring(15, 4);
                Session["CardNo"] = masked;

                
                //req = miri.MiriCardTransactionRest(nameoncard, cardNumber, month, year, cvv, TransactionId, merchant);

                req = new demoClass().MiriCardTransactionRest(nameoncard, cardNumber, month, year, cvv, TransactionId, merchant);
                SqlHelper sql = new SqlHelper();
                if (req.StatusCode == "0")
                {
                    string NameOnCard = req.NameOnCard;
                    string TransactionNumber = req.TransactionNumber;
                    string UserProfileNumber = req.UserProfileNumber;

                    sql.AddParameterToSQLCommand("@statuscode", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@statuscode", req.StatusCode);
                    sql.AddParameterToSQLCommand("@miri_account_no", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@miri_account_no", req.MiriAccountNumberB10);
                    sql.AddParameterToSQLCommand("@profilenumber", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@profilenumber", req.UserProfileNumber);
                    sql.AddParameterToSQLCommand("@nameoncard", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@nameoncard", req.NameOnCard);
                    sql.AddParameterToSQLCommand("@mirinumber", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@mirinumber", req.MiriDynamicNumber);
                    sql.AddParameterToSQLCommand("@statusmessage", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@statusmessage", req.StatusMessage);
                    sql.AddParameterToSQLCommand("@transaction_date", SqlDbType.DateTime);
                    sql.SetSQLCommandParameterValue("@transaction_date", req.TransactionDate);
                    sql.AddParameterToSQLCommand("@transactionid", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@transactionid", TransactionId);
                    sql.AddParameterToSQLCommand("@ip", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@ip", IpAddress);
                    sql.AddParameterToSQLCommand("@sessionid", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@sessionid", Sessionid);
                    sql.AddParameterToSQLCommand("@user_agent", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@user_agent", Browser);
                    //sql.AddParameterToSQLCommand("@company", SqlDbType.VarChar);
                    //sql.SetSQLCommandParameterValue("@company", year);
                    sql.AddParameterToSQLCommand("@merchantid", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@merchantid", merchant);
                    sql.AddParameterToSQLCommand("@amount", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@amount", Amt);
                    //sql.AddParameterToSQLCommand("@designation", SqlDbType.VarChar);
                    //sql.SetSQLCommandParameterValue("@designation", cvv);
                    int r = sql.GetExecuteNonQueryByCommand("insert_log_miri_transaction");
                }
                else
                {
                    sql.AddParameterToSQLCommand("@transactionid", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@transactionid", TransactionId);
                    sql.AddParameterToSQLCommand("@statuscode", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@statuscode", req.StatusCode);
                    sql.AddParameterToSQLCommand("@statusmessage", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@statusmessage", req.StatusMessage);
                    sql.AddParameterToSQLCommand("@transaction_date", SqlDbType.DateTime);
                    sql.SetSQLCommandParameterValue("@transaction_date", req.TransactionDate);
                    sql.AddParameterToSQLCommand("@ip", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@ip", IpAddress);
                    sql.AddParameterToSQLCommand("@sessionid", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@sessionid", Sessionid);
                    sql.AddParameterToSQLCommand("@user_agent", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@user_agent", Browser);
                    //sql.AddParameterToSQLCommand("@company", SqlDbType.VarChar);
                    //sql.SetSQLCommandParameterValue("@company", year);
                    sql.AddParameterToSQLCommand("@merchantid", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@merchantid", merchant);
                    int r = sql.GetExecuteNonQueryByCommand("insert_log_miri_transaction");
                    return Content("108,FAIL," + req.StatusMessage);
                }
                return Content("005,Success," + Session["TransactionID"] + "," + Session["TransactionDate"] + "," + Session["MerchantId"] + "," + Session["CardNo"] + "," + Session["CardType"] + "," + Session["Amt"], "text /html");
            }

            catch (Exception ex)
            {
                ExceptionLogging.SendExcepToDB(ex);
                return Content("109,FAIL,ERROR," + ex.Message);
            }
        }
    }
}
using MTLMiriLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;
using static MTLMiriLib.MiriHelper;

namespace MtlMiriDemo_V1.Controllers
{
    public class MiriTokenController : Controller
    {
        // GET: MiriToken
      
        string MiritokenIssuer = ConfigHelper.GetTokenIssuer();
        string sms = System.Configuration.ConfigurationManager.AppSettings["SMS"];
        string mail = System.Configuration.ConfigurationManager.AppSettings["mail"];
        string Filepath = System.Configuration.ConfigurationManager.AppSettings["myFilePath"];
        public ActionResult Index()
        {
            if (Session["userid"] != null)
            {
                if (!string.IsNullOrEmpty(Session["userid"] as string))
                {
                    ViewBag.name = Session["name"];
                    ViewBag.designation = Session["designation"];
                    ViewBag.email = Session["email"];
                    ViewBag.mobile = Session["mobile"];
                    ViewBag.userid = Session["userid"];
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        public JsonResult Detail(string id = "")
        {
            SqlHelper sql = new SqlHelper();
            UserData data = new UserData();
            try
            {
                sql.AddParameterToSQLCommand("@hash", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@hash", id);

                DataSet ds = sql.GetDatasetByCommand("get_user_data");
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    data.status = "valid";
                    //   data.empid = ds.Tables[0].Rows[0]["empid"] != null ? ds.Tables[0].Rows[0]["empid"].ToString().Trim() : "";
                    data.firstname = ds.Tables[0].Rows[0]["firstname"] != null ? ds.Tables[0].Rows[0]["firstname"].ToString().Trim() : "";
                    data.lastname = ds.Tables[0].Rows[0]["lastname"] != null ? ds.Tables[0].Rows[0]["lastname"].ToString().Trim() : "";
                    data.image = ds.Tables[0].Rows[0]["image"] != null ? ds.Tables[0].Rows[0]["image"].ToString().Trim() : "";
                    data.mobile = ds.Tables[0].Rows[0]["mobile"] != null ? ds.Tables[0].Rows[0]["mobile"].ToString().Trim() : "";
                    data.email = ds.Tables[0].Rows[0]["email"] != null ? ds.Tables[0].Rows[0]["email"].ToString().Trim() : "";
                }
                else
                {
                    data.status = "invalid";

                }
            }
            catch(Exception ex)
            {
                ExceptionLogging.SendExcepToDB(ex);
                data.status = "invalid";
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            //   return data;
            // return data.ToString();
            return Json(data, JsonRequestBehavior.AllowGet);
            // return JsonConvert.SerializeObject(data);
            // return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateStatus(string hash, string status)
        {

            Status data = new Status();
            try
            {
                SqlHelper sql = new SqlHelper();
                //  DataSet ds=   sql.GetDatasetByCommand("Select fullname,miri_account_no from miri_account where " +
                //      "username='" + this.UserName + "' and password ='"+ password + "'");

                sql.AddParameterToSQLCommand("@hash", SqlDbType.VarChar);
                sql.AddParameterToSQLCommand("@status", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@hash", hash);
                sql.SetSQLCommandParameterValue("@status", status);


                int n = sql.GetExecuteNonQueryByCommand("update_activation_status");

                if (n == 1)
                {
                    data.statuscode = "SUCCESS";
                    //   data.empid = ds.Tables[0].Rows[0]["empid"] != null ? ds.Tables[0].Rows[0]["empid"].ToString().Trim() : "";
                    data.statusmessage = "Activation status updated.";
                }
                else
                {
                    data.statuscode = "FAIL";
                    data.statusmessage = "Unable to update activation status. Record not found.";

                }
                //   return data;
                // return data.ToString();
                return Json(data, JsonRequestBehavior.AllowGet);
                // return JsonConvert.SerializeObject(data);
                // return Json(data, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                data.statuscode = "ERROR";
                data.statusmessage = "Failed to update activation status. " + ex.Message;
               
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetActivationStatus(string id)
        {

            Status data = new Status();
            try
            {
                SqlHelper sql = new SqlHelper();
                //  DataSet ds=   sql.GetDatasetByCommand("Select fullname,miri_account_no from miri_account where " +
                //      "username='" + this.UserName + "' and password ='"+ password + "'");

                sql.AddParameterToSQLCommand("@hash", SqlDbType.VarChar);
                //  sql.AddParameterToSQLCommand("@status", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@hash", id);
                //    sql.SetSQLCommandParameterValue("@status", status);


                object n = sql.GetExecuteScalarByCommand("get_activation_status");
                int status = Convert.ToInt32(n);
                if (status >= 0)
                {

                    data.statuscode = "SUCCESS";
                    //   data.empid = ds.Tables[0].Rows[0]["empid"] != null ? ds.Tables[0].Rows[0]["empid"].ToString().Trim() : "";
                    data.statusmessage = "ACTIVATION_STATE_" + status.ToString();
                }
                else
                {
                    data.statuscode = "FAIL";
                    data.statusmessage = "Unable to get activation status. Record not found.";

                }
                //   return data;
                // return data.ToString();
                return Json(data, JsonRequestBehavior.AllowGet);
                // return JsonConvert.SerializeObject(data);
                // return Json(data, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                data.statuscode = "ERROR";
                data.statusmessage = "Failed to get activation status. " + ex.Message;
                ExceptionLogging.SendExcepToDB(ex);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }


        [OutputCache(Duration = 0)]
        public ActionResult UserImage(string id)
        {

            string filename = Session["miriaccountnumber"].ToString();
            //  string path = Path.Combine(Server.MapPath("~/UploadedFiles"), filename + ".jpg");

            //string path = @"D:/UploadedFiles/" + filename + ".jpg";
            string path = Filepath + filename + ".jpg";
            return base.File(path, "image/jpeg");
        }


        //[OutputCache(Duration = 0)]
        //public ActionResult EmpImage(string id)
        //{

        //    string filename = Session["userid"].ToString();
        //    //  string path = Path.Combine(Server.MapPath("~/UploadedFiles"), filename + ".jpg");

        //    string path =  @"D:/UploadedFiles/" + filename + ".jpg";
        //    return base.File(path, "image/jpeg");
        //}

        private bool CheckUserExists(string email, string productcode)
        {

            SqlHelper sql = new SqlHelper();
            try
            {
                sql.AddParameterToSQLCommand("@email", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@email", email);
                sql.AddParameterToSQLCommand("@product", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@product", productcode);
                object r = sql.GetExecuteScalarByCommand("CheckUserExists");
                if (r is int && Convert.ToInt32(r) > 0)
                {
                    return true;
                }
                else return false;
            }
            catch(Exception ex)
            {
                ExceptionLogging.SendExcepToDB(ex);
                return false;
            }

        }
        public string GenerateRandomNo()
        {
            int _min = 100;
            int _max = 999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max).ToString();
        }
        public ActionResult RegisterUser(string firstname, string lastname, string email, string mobile, string _phoneCode, string camimage, HttpPostedFileBase file, string productcode = "2", string designation = "", string company = "")
        {
            if (Session["userid"] == null) { RedirectToAction("Index", "Product"); }

            Session["FirstName"] = firstname;
            Session["LastName"] = lastname;
            Session["Email1"] = email;
            Session["Mobile1"] = mobile;
            Session["phoneCode"] = _phoneCode;
            int maxlength = 2 * 1024 * 1024;
            if (file == null && string.IsNullOrEmpty(camimage)) { return Content("104:FAIL:" + "PHOTO_MANDATORY"); }
            if (file != null && file.ContentLength > maxlength) { return Content("105:FAIL:" + "SIZE_LIMIT_2MB"); }
            try
            {
                string domain = ConfigHelper.GetDomain();
                MiriHelper miri = new MiriHelper(domain);
                RegisterRequest req = new RegisterRequest();
                
                if (CheckUserExists(email, productcode))
                {

                    return Content("106:FAIL:" + "User alredy exist");
                }

                //req.IssuerNumber = MiritokenIssuer;
                req.IssuerNumber = "15";
                req.IssuerNumber = ConfigurationManager.AppSettings["TOKEN_ISSUER"];
                req.FirstName = firstname;
                req.LastName = lastname;
                req.ActivationFieldOne = firstname.ToLower();
                req.ActivationFieldTwo = lastname.ToLower();
                req.LookUpDataOne = CreateMD5(email + "_" + DateTime.Now.ToString());
                req.ExpiryDate = DateTime.Now.AddMonths(12).ToString("MM/dd/yyyy").Replace("-", "/");
                req.AccountIssueDate = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/");
               // RegisterResponse res = miri.RegisterUser(req);
               // string userprofilenumber = Active.UserProfileId;

                req.ResponseDataOne = firstname.ToLower() + lastname.ToLower();


                string auth = "";

                ActivationCodeRes.Root ActiveCode = demoClass.CreateProfileAccountGetActivationCode(auth, req);
                string userprofilenumber = ActiveCode.resp.TheAccount.ProfileId.ToString();

                string userProfileNumber = ActiveCode.resp.TheAccount.ProfileId.ToString()
;
                
                if (ActiveCode.resp.StatusCode == "0")
                {
                    string activationcode = ActiveCode.resp.TheAccount.AccountActivationCode;
                    string miriaccountnumber = ActiveCode.resp.TheAccount.MiriAccountNumber;
                    Session["activationcode"] = activationcode;
                    Session["miriaccountnumber"] = miriaccountnumber;
                    Session["fieldone"] = firstname;
                    Session["fieldtwo"] = lastname;


                    SqlHelper sql = new SqlHelper();

                    string mobilenumber = _phoneCode + "-" + mobile.ToString();
                    sql.AddParameterToSQLCommand("@firstname", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@firstname", firstname);

                    sql.AddParameterToSQLCommand("@lastname", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@lastname", lastname);

                    sql.AddParameterToSQLCommand("@email", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@email", email);

                    sql.AddParameterToSQLCommand("@image", SqlDbType.VarChar);

                    sql.AddParameterToSQLCommand("@activationcode", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@activationcode", activationcode);

                    sql.AddParameterToSQLCommand("@miriidentifier", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@miriidentifier", miriaccountnumber);

                    sql.AddParameterToSQLCommand("@mobile", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@mobile", mobilenumber);

                    sql.AddParameterToSQLCommand("@company", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@company", company);

                    sql.AddParameterToSQLCommand("@designation", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@designation", designation);

                    sql.AddParameterToSQLCommand("@product", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@product", productcode);

                    sql.AddParameterToSQLCommand("@miriprofilenumber", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@miriprofilenumber", userprofilenumber);

                    sql.AddParameterToSQLCommand("@createdby", SqlDbType.Int);
                    sql.SetSQLCommandParameterValue("@createdby", Session["userid"]);

                    try
                    {
                        //string path = Path.Combine(Server.MapPath("~/UploadedFiles"), Path.GetFileName(file.FileName));


                        // string path = Path.Combine(Server.MapPath("~/UploadedFiles"),miriaccountnumber+".jpg" );


                        //string path =  @"D:/UploadedFiles/" + miriaccountnumber + ".jpg";
                        string path = Filepath + miriaccountnumber + ".jpg";

                        string image = "";

                        if (camimage.Length > 100)
                        {

                            string imgstr = camimage.Substring(camimage.IndexOf(',') + 1);

                            var bytess = Convert.FromBase64String(imgstr);
                            using (var imageFile = new FileStream(path, FileMode.Create))
                            {
                                imageFile.Write(bytess, 0, bytess.Length);
                                imageFile.Flush();
                            }
                            image = imgstr;
                        }

                        else if (file != null)
                        {

                            file.SaveAs(path);
                            Image img = Image.FromFile(path);
                            image = ImageToBase64(img);
                        }

                        //file.SaveAs(path);
                        //Image img = Image.FromFile(path);

                        //// Image img = Image.FromFile(Path.Combine(Server.MapPath("~/UploadedFiles"), miriaccountnumber + ".jpg"));
                        //string image = ImageToBase64(img);
                        sql.SetSQLCommandParameterValue("@image", image);
                    }

                    catch (Exception ex)
                    {
                        ExceptionLogging.SendExcepToDB(ex);
                        return Content("106:FAIL:IMAGE_UPLOAD_ERROR : " + ex.Message, "text/html");
                    }
                    int r = sql.GetExecuteNonQueryByCommand("registration_insert");

                    if (r > 0)
                    {
                        // Session["activationcode"] = activationcode;
                        String html = "<html><body>";
                        html += "<div id=\"activationcode\">" + activationcode + "</div><br/>" + firstname + "<br/>" + lastname + "<br/>" + "<img src='" + Url.Action("QRCode", "Test") + "/" + Session["miriaccountnumber"].ToString() + "' />";
                        html += "<img id='userimage' src='" + Url.Action("UserImage", "Test") + "/" + Session["miriaccountnumber"].ToString() + "' />";
                        html += "</body></html>";
                        //string androidlink = "";
                        //string ioslink = "";
                        //string productname = "";
                        //productname = "MiriToken"; androidlink = "https://bit.ly/3bIs9Fo"; ioslink = "https://apple.co/2rgRiok";
                        //if (sms == "enable")
                        //{
                        //    if (_phoneCode == "+91")
                        //    {
                        //        sendsms(firstname, " ", activationcode.ToUpper(), mobile, androidlink, ioslink, productname, _phoneCode);
                        //    }
                        //}

                        //if (mail == "enable")
                        //{

                        //    //SendMail(email, "noreply.ebt@manipalgroup.info", activationcode, firstname, androidlink, "https://mmcpayment.com/mes/files/MiriIdAndroid100px.png", ioslink, "https://mmcpayment.com/mes/files/MiriIdApple100px.png", productname, "Nrp$8et3");

                        //    string body = this.PopulateBody(firstname, activationcode, androidlink, "https://mmcpayment.com/mes/files/MiriIdAndroid100px.png", ioslink, "https://mmcpayment.com/mes/files/MiriIdApple100px.png", productname);
                        //    SendMail(email, "noreply.ebt@manipalgroup.info", "Nrp$8et3", body);


                        //}

                        return Content("005:Success:" + activationcode, "text/html");
                    }



                    //return activationcode+Environment.NewLine+firstname+Environment.NewLine+lastname;
                    string failhtml = "<html><body>";
                    failhtml += "FAIL:REGISTER";
                    return Content("107:FAIL:REGISTERSQL", "text/html");
                }

                else
                {
                    return Content("108:FAIL:" + ActiveCode.resp.StatusMessage);
                }

            }


            catch (Exception ex)
            {
                ExceptionLogging.SendExcepToDB(ex);
                return Content("108:FAIL:REGISTER:" + ex.Message);

            }


        }

        public void PopulateBody()
        {
            // string body = string.Empty;
            if (mail == "enable")
            {
                string firstname = Session["FirstName"].ToString();
                string lastname = Session["LastName"].ToString();
                string activationcode = Session["activationcode"].ToString();
                string androidlink = "https://bit.ly/3bIs9Fo";
                // string androidQR;
                string ioslink = "https://apple.co/2rgRiok";
                //string iosQR;
                string Product = "Miri Token";

                string acode = string.Join(",", activationcode, firstname, lastname);
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Email/index.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{Customer First Name}", firstname);
                body = body.Replace("{4256 8745 1452 2356}", activationcode);
                body = body.Replace("{123456}", acode);
                body = body.Replace("{androlink}", androidlink);
                body = body.Replace("{qrandroid}", "https://mmcpayment.com/mes/files/AndroidMiritokenQr.png");
                body = body.Replace("{applelink}", ioslink);
                body = body.Replace("{qrapple}", "https://mmcpayment.com/mes/files/AppleMiritokenQr.png");
                body = body.Replace("{desktoplink}", "https://mmcpayment.com/TestMES/MiriToken/AppLink/miritoken_pc");
                body = body.Replace("{qrdesktop}", "https://mmcpayment.com/mes/files/DesktopMiriToken.png");
                body = body.Replace("{MiriID/MiriToken}", Product);
               // body = body.Replace("{img}", "https://mmcpayment.com/TestMES/Email/miri-token.jpg");
                //return body;
                SendMail(body);
            }
        }

        public void SendMail(string strMailBody)
        {
            string emailTo = Session["Email1"].ToString();
            string Emailfrom = "noreply.ebt@manipalgroup.info";
            string Networkpassword = "Nrp$8et3";
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(Emailfrom);
                message.To.Add(new MailAddress(emailTo));
                message.Subject = "Welcome to Miri Experience Suite";
                message.IsBodyHtml = true;
                message.Body = strMailBody;
                //smtp.Port = 587;
                smtp.Port = 25;
                smtp.Host = "manipalgroup.info";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("noreply.ebt", Networkpassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                // log.WriteToFile(ex.Message);
                ExceptionLogging.SendExcepToDB(ex);
            }
        }
        public void sendsms()
        {
            try
            {
                if (sms == "enable")
                {
                    string countrycode = Session["phoneCode"].ToString();

                    if (countrycode == "+91")
                    {
                        string name = Session["FirstName"].ToString();
                        string lastname = Session["LastName"].ToString();
                        string activationcode = Session["activationcode"].ToString(); ;
                        string mobile = Session["Mobile1"].ToString();
                        string androidlink = "https://bit.ly/3bIs9Fo";
                        string ioslink = "https://apple.co/2rgRiok";
                        string Product = "Miri Token";
                        // use the API URL here
                        name = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
                        string message = "Dear " + name + "," + Environment.NewLine;
                        message += "Thank you for registering to experience our " + Product + ".";
                        message += " Please download the Mobile App, activate it with the code " + activationcode + "." + Environment.NewLine;
                        message += "links for downloading are, Android: " + androidlink + " " + " and iOS: " + ioslink + "." + Environment.NewLine;
                        message += "Regards" + Environment.NewLine + "MTL EBT Team";

                        string auth = "302149AGyJjJzqcR15dc013c6";
                        auth = "302149AcZR85gFx3Py5dc4fa01";
                        string DT_ID = "1007836701686344352";
                        string strUrl = "https://api.msg91.com/api/sendhttp.php?mobiles=" + mobile + "&authkey=" + auth + "&route=" + 4 + "&sender=" + "MTLEBT" + "&message=" + " " + message + " " + "&country=" + countrycode + "&DLT_TE_ID=" + DT_ID;
                        // Create a request object
                        WebRequest request = HttpWebRequest.Create(strUrl);
                        // Get the response back
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream s = (Stream)response.GetResponseStream();
                        StreamReader readStream = new StreamReader(s);
                        string dataString = readStream.ReadToEnd();
                        response.Close();
                        s.Close();
                        readStream.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                ExceptionLogging.SendExcepToDB(ex);
            }
        }

        public JsonResult RestQr()
        {
            try
            {
                string activationcode = Session["activationcode"].ToString();
                string miriaccountnumber = Session["miriaccountnumber"].ToString();
                string domain = ConfigHelper.GetDomain();
                MiriHelper miri1 = new MiriHelper(domain);
                var re1 = miri1.ResetUser(activationcode, miriaccountnumber);

                if (re1.StatusCode == "0")
                {
                    Session["activationcode"] = re1.ActivationCode;
                    // QRCode(re1.ActivationCode);
                    //return Json(re1, JsonRequestBehavior.AllowGet);
                    saveQR(re1.ActivationCode, Session["miriaccountnumber"].ToString());
                }
                return Json(re1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // return Content("108:Fail:Reset Qr" + ex.Message);
                ExceptionLogging.SendExcepToDB(ex);
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
            finally
            {
            }
        }

        public int De_Reg()
        {
            try
            {
                string activationcode = Session["activationcode"].ToString();
                string miriaccountnumber = Session["miriaccountnumber"].ToString();
                string domain = ConfigHelper.GetDomain();
                MiriHelper miri1 = new MiriHelper(domain);
                var re1 = miri1.DeRegisterUser(activationcode, miriaccountnumber);
                if (re1.StatusCode == "0")
                {
                    SqlHelper sql = new SqlHelper();
                    sql.AddParameterToSQLCommand("@activationcode", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@activationcode", activationcode);
                    sql.AddParameterToSQLCommand("@miriidentifier", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@miriidentifier", miriaccountnumber);
                    sql.AddParameterToSQLCommand("@Status", SqlDbType.VarChar);
                    sql.SetSQLCommandParameterValue("@Status", "U");
                    int r = sql.GetExecuteNonQueryByCommand("proc_deregister");
                }
                int scode = Convert.ToInt32(re1.StatusCode);

                return scode;
            }
            catch (Exception ex)
            {
                ErrorLogFile LogError1 = new ErrorLogFile();
                LogError1.LogError(ex);
                ExceptionLogging.SendExcepToDB(ex);
                return 1;

            }
            finally
            {
            }
        }

        //public JsonResult De_Reg()
        //{
        //    try
        //    {
        //        string activationcode = Session["activationcode"].ToString();
        //        string miriaccountnumber = Session["miriaccountnumber"].ToString();
        //        MiriHelper miri1 = new MiriHelper();
        //        var re1 = miri1.DeRegisterUser(activationcode, miriaccountnumber);
        //        if (re1.StatusCode == "0")
        //        {
        //            SqlHelper sql = new SqlHelper();
        //            sql.AddParameterToSQLCommand("@activationcode", SqlDbType.VarChar);
        //            sql.SetSQLCommandParameterValue("@activationcode", activationcode);
        //            sql.AddParameterToSQLCommand("@miriidentifier", SqlDbType.VarChar);
        //            sql.SetSQLCommandParameterValue("@miriidentifier", miriaccountnumber);
        //            sql.AddParameterToSQLCommand("@Status", SqlDbType.VarChar);
        //            sql.SetSQLCommandParameterValue("@Status", "U");
        //            int r = sql.GetExecuteNonQueryByCommand("proc_deregister");
        //        }
        //        return Json(re1, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        // return Content("108:Fail:Reset Qr" + ex.Message);
        //        ErrorLogFile LogError1 = new ErrorLogFile();
        //        LogError1.LogError(ex);
        //        return Json(ex, JsonRequestBehavior.AllowGet);
        //    }
        //    finally
        //    {
        //    }
        //}

        public void saveQR(string activation, string miriaccountno)
        {
            SqlHelper sql = new SqlHelper();
            try
            {
                sql.AddParameterToSQLCommand("@activationcode", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@activationcode", activation);
                sql.AddParameterToSQLCommand("@MiriProfileNumber", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@MiriProfileNumber", miriaccountno);
                sql.AddParameterToSQLCommand("@Status", SqlDbType.VarChar);
                sql.SetSQLCommandParameterValue("@Status", "I");
                int r = sql.GetExecuteNonQueryByCommand("Proc_RestQr");
            }
            catch(Exception ex)
            {
                ExceptionLogging.SendExcepToDB(ex);
            }
        }


       
        //public void SendMail(string emailTo, string Emailfrom, string activationcode, string fname, string androidlink, string androidQR, string ioslink, string iosQR, string Product, string Networkpassword)
        //{
        //    try
        //    {
        //        MailMessage message = new MailMessage();
        //        SmtpClient smtp = new SmtpClient();
        //        message.From = new MailAddress(Emailfrom);
        //        message.To.Add(new MailAddress(emailTo));
        //        message.Subject = "Welcome to Miri Experience Suite";
        //        message.IsBodyHtml = true;
        //        string strMailBody = "";
        //        strMailBody = "Dear " + fname + " ,<br /> <br />";
        //        strMailBody += "<div>Thank you for registering to experience our <b>" + Product + "</b></div><br />";
        //        strMailBody += "<div>You're just a few clicks away from starting your Miri Experience. Please follow the steps below and you'll be live in no time!</div><br /> ";
        //        strMailBody += "<div><b>Step 1)</b> Please download " + Product + " Mobile App by clicking below icons and scan the QR </div>";
        //        strMailBody += "<table><tr><td><div><p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;•<ion-icon name='logo - android'></ion-icon> </p></div><div><p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;	<img src='" + androidQR + "' Style='height:75px;width:75px' /></p></div></td><td><div><p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
        //        strMailBody += "•	<img src='https://mmcpayment.com/mes/files/AndroidMiriIDQR.png' style='width: 50px; height: 50px;'> <a href='" + androidlink + "'></a></img></p></div><div><p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;	<img src='" + iosQR + "' Style='height:75px;width:75px' /></p></div></td></tr></table>";
        //        strMailBody += "<div><b>Step 2)</b> Activate your mobile App using below Activation code <b>" + activationcode + "</b> scan QR <img src='https://mmauth.com/MTLAttendance/generateqr/AppleMiriidQR.png' + activationcode +/></div>";
        //        strMailBody += "<div><b>Step 3)</b> Use the App & Generate your MiriID</div> <br /> <br />";
        //        strMailBody += "<div>Regards, </div>";
        //        strMailBody += "<div><b>Team Manipal</b></div>";
        //        message.Body = strMailBody;
        //        smtp.Port = 587;
        //        //smtp.Port = 25;
        //        smtp.Host = "manipalgroup.info";
        //        smtp.EnableSsl = true;
        //        smtp.UseDefaultCredentials = false;
        //        smtp.Credentials = new NetworkCredential(Emailfrom, Networkpassword);
        //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        smtp.Send(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        // log.WriteToFile(ex.Message);
        //    }
        //}

        public ActionResult AppLink(string id)
        {
            string applink = "";

            if (id == "miritoken_android")
            {
                applink = @"https://bit.ly/3bIs9Fo";
            }


            else if (id == "miritoken_ios")
            {
                applink = @"https://apple.co/2rgRiok";
            }

            else if (id == "MiriToken Desktop")
            {
                applink = Server.MapPath("~/files/MIRITokenAPP/MiriTokenApp.application");
            }


            return File(applink, "application/application", id + ".application");

        }
        public FileContentResult AppQRCode(string id)
        {
            string applink = "";
            if (id == "miritoken_android")
            {
                applink = @"https://bit.ly/3bIs9Fo";

            }
            else if (id == "miritoken_ios")
            {
                applink = @"https://apple.co/2rgRiok";
            }

            else if (id == "miritoken_pc")
            {
                //applink = Server.MapPath("~/files/MIRIIDAPP/MiriIDApp.application");
            }
            MemoryStream memoryStream = GenerateMyQCCode(applink);
            byte[] fileContents = memoryStream.ToArray();
            // return new FileContentResult(fileContents, "image/jpg");
            return new FileContentResult(fileContents, "images/jpg");
        }

        private MemoryStream GenerateMyQCCode(string QCText)
        {
            try
            {

                var QCwriter = new BarcodeWriter();
                QCwriter.Format = BarcodeFormat.QR_CODE;

                QCwriter.Options = new EncodingOptions
                {
                    Height = 150,
                    Width = 150
                };

                QCwriter.Options.Margin = 1;

                var result = QCwriter.Write(QCText);
                //  string path = Server.MapPath("~/~/Content/images/MyQRImage.jpg");
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


        public string ImageToBase64(Image image)
        {

            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }


        [OutputCache(Duration = 0)]
        public FileContentResult QRCode(string id)
        {
            string activationcode = Session["activationcode"].ToString();
            string fieldone = Session["fieldone"].ToString();
            string fieldtwo = Session["fieldtwo"].ToString();
            fieldone.ToLower();
            fieldtwo.ToLower();

            //  fieldone = fieldone.Substring(0, 1).ToUpper() + fieldone.Substring(1);
            //  fieldtwo = fieldtwo.Substring(0, 1).ToUpper() + fieldtwo.Substring(1);
            MemoryStream memoryStream = GenerateMyQCCode(activationcode + "," + fieldone.ToLower() + "," + fieldtwo.ToLower());
            byte[] fileContents = memoryStream.ToArray();
            return new FileContentResult(fileContents, "image/jpg");
        }

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public string LoginToken(string userid, string token)
        {
            string val1 = token.Replace("-", string.Empty);
            if (ValidateDemoTokenUser(userid))
            {
                string identifier = Session["miriaccountnumber"].ToString();
                return ValidateMiriToken(identifier, val1);
            }


            else
            {
                return "001:INVALID:INVALID_USER";
            }
        }

        public string ValidateMiriToken(string identifier, string token)
        {
            string domain = ConfigHelper.GetDomain();
            MiriHelper miri = new MiriHelper(domain);
            //MiriHelper.MiriTokenChallengeResponseResult res = miri.MiriTokenTransactionRest(identifier, token);
            //string url = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/SERVICES/MIServiceDecoder/Verify/MiriToken";
            string url = "";
            string MiriTokenValidationRes = MtlMiriDemo_V1.Request.CreateGetReq(url,token,identifier);

            TokenValidationRes.Root res = JsonConvert.DeserializeObject<TokenValidationRes.Root>(MiriTokenValidationRes);


            //URI	https://{DomainURL}/MIRISYSTEMS/Services/MIServiceDecoder/Verify/MiriToken/{alt-key}/{miri-token}
            if (res != null && res.StatusCode == "0")
            {               
                return "000:VALID" + ":" + Session["user_firstname"] + ":" + Session["user_lastname"] + ":" + Session["user_email"] + ":" + Session["user_mobile"] + ":" + Session["user_designation"] + ":" + Session["user_company"];
            }

            else if (res != null && res.StatusCode == "536")
            {
                return "004:FRAUD_ALERT";
            }
            else
            {
                return "001:INVALID:INVALID_USER";
            }
        }

        private bool ValidateDemoTokenUser(string userid)
        {
            if (Session["userid"] == null) { RedirectToAction("Login", "DemoUser"); }
            string firstname = "";
            string lastname = "";
            string email = "";
            string image = "";
            string mobile = "";
            string miriidentifier = "";
            string designation = "";
            string company = "";
            SqlHelper sql = new SqlHelper();
            // DataSet ds = sql.GetDatasetByCommand("get_employee");
            sql.AddParameterToSQLCommand("@hash", SqlDbType.VarChar);
            sql.SetSQLCommandParameterValue("@hash", userid);
            DataSet ds = sql.GetDatasetByCommand("get_token_user_data");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 1)
            {
                Session["user_firstname"] = firstname = ds.Tables[0].Rows[0]["firstname"].ToString().Trim();
                Session["user_lastname"] = lastname = ds.Tables[0].Rows[0]["lastname"].ToString().Trim();
                Session["user_image"] = image = ds.Tables[0].Rows[0]["image"].ToString().Trim();
                Session["user_email"] = email = ds.Tables[0].Rows[0]["email"].ToString().Trim();
                Session["user_mobile"] = mobile = ds.Tables[0].Rows[0]["mobile"].ToString().Trim();
                Session["user_designation"] = designation = ds.Tables[0].Rows[0]["designation"].ToString().Trim();
                Session["user_company"] = company = ds.Tables[0].Rows[0]["company"].ToString().Trim();
                Session["miriaccountnumber"] = Session["user_miriidentifier"] = miriidentifier = ds.Tables[0].Rows[0]["miriidentifier"].ToString().Trim();

                return true;
            }
            else

            {
                return false;
            }
        }
    }
}
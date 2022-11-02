using MTLMiriLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MtlMiriDemo_V1.Controllers
{
    public class DemoUserController : Controller
    {
        string Filepath = System.Configuration.ConfigurationManager.AppSettings["myFilePath"];
        // GET: DemoUser
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Detail(string id = "")
        {
            SqlHelper sql = new SqlHelper();
            //  DataSet ds=   sql.GetDatasetByCommand("Select fullname,miri_account_no from miri_account where " +
            //      "username='" + this.UserName + "' and password ='"+ password + "'");
            sql.AddParameterToSQLCommand("@hash", SqlDbType.VarChar);
            //  sql.AddParameterToSQLCommand("@password", SqlDbType.VarChar);
            sql.SetSQLCommandParameterValue("@hash", id);
            // sql.SetSQLCommandParameterValue("@empid", );
            UserData data = new UserData();
            DataSet ds = sql.GetDatasetByCommand("get_user_detail");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                data.status = "valid";
                //   data.empid = ds.Tables[0].Rows[0]["empid"] != null ? ds.Tables[0].Rows[0]["empid"].ToString().Trim() : "";
                data.firstname = ds.Tables[0].Rows[0]["firstname"] != null ? ds.Tables[0].Rows[0]["firstname"].ToString().Trim() : "";
                data.lastname = ds.Tables[0].Rows[0]["lastname"] != null ? ds.Tables[0].Rows[0]["lastname"].ToString().Trim() : "";
               // data.image = ds.Tables[0].Rows[0]["image"] != null ? ds.Tables[0].Rows[0]["image"].ToString().Trim() : "";
                data.mobile = ds.Tables[0].Rows[0]["mobile"] != null ? ds.Tables[0].Rows[0]["mobile"].ToString().Trim() : "";
                data.email = ds.Tables[0].Rows[0]["email"] != null ? ds.Tables[0].Rows[0]["email"].ToString().Trim() : "";

                string identifier = ds.Tables[0].Rows[0]["miriidentifier"] != null ? ds.Tables[0].Rows[0]["miriidentifier"].ToString().Trim() : "";

                data.image = GetImage(identifier);
            }
            else
            {
                data.status = "invalid";

            }
            //   return data;
            // return data.ToString();
            return Json(data, JsonRequestBehavior.AllowGet);
            // return JsonConvert.SerializeObject(data);
            // return Json(data, JsonRequestBehavior.AllowGet);
        }



        private string GetImage(string miriaccountnumber)
        {
            try
            {
                String imgstring = "";
                string path = Filepath + miriaccountnumber + ".jpg";

                Image img = Image.FromFile(path);
                imgstring = ImageToBase64(img);
                return imgstring;
            }

            catch(Exception ex)
            {
                return "";
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



    }
}
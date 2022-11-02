using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTLMiriLib;
namespace MtlMiriDemo_V1.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Activity()
        {
            string filepath = "";
            try
            {
                

                if (Session["userid"] == null) { return Content("NOT AUTHENTICATED!!!"); }
                if (Session["usertype"] == null || Session["usertype"].ToString() != "A") { return Content("NOT AUTHORIZED!!!"); }
                String s = "Login Id,User Name,Login Date, First Name, Last Name, Email, Designation,Mobile,Product,Company,RegistrationDate" + Environment.NewLine;
                LogHelper l = new LogHelper();
                List<LogHelper.ActivityLog> list = l.GetActivityLog("");
                foreach (LogHelper.ActivityLog log in list)
                {


                    s += log.LoginId + "," + log.UserName + "," + log.LoginDate + "," + log.FirstName + "," + log.LastName + "," + log.Email + "," + log.Designation + "," + log.Mobile + "," + log.Product + "," + log.Company + "," + log.RegistrationDate + Environment.NewLine;

                }

                 filepath = "D:\\Reports\\" + "Activity_Report" + "_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".csv";

                WriteToFile(s, filepath);
                return File(filepath, "text/csv", "Activity_Report.csv");
            }
            catch(Exception ex)
            {
                
                ExceptionLogging.SendExcepToDB(ex);
                return File(filepath, "text/csv", "Activity_Report.csv");
            }
        }

        public void WriteToFile(string Message, string filepath)
        {
            try
            {
                string path = "D:\\Reports";
                // string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
             
                if (!System.IO.File.Exists(filepath))
                {
                    // Create a file to write to.   
                    using (StreamWriter sw = System.IO.File.CreateText(filepath))
                    {
                        sw.WriteLine(Message);
                    }
                }
                //else
                //{
                //    using (StreamWriter sw = System.IO.File.AppendText(filepath))
                //    {
                //        sw.WriteLine(Message);
                //    }
                //}
            }

            catch (Exception ex)
            {
                ExceptionLogging.SendExcepToDB(ex);
            }

        }







    }
}
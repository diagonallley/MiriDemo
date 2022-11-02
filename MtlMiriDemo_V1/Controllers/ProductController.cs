using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MtlMiriDemo_V1.Controllers
{
    public class ProductController : Controller
    {
        string Filepath = System.Configuration.ConfigurationManager.AppSettings["myFilePath"];
        // GET: Product
        [OutputCache(NoStore = true, Duration = 0)]
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
            else {
                return RedirectToAction("Index", "Login");
            }
           
            return View();
        }


        [OutputCache(Duration = 0)]
        public ActionResult EmpImage(string id)
        {
            string filename = Session["userid"].ToString();
            //  string path = Path.Combine(Server.MapPath("~/UploadedFiles"), filename + ".jpg");
            string path = Filepath + filename + ".jpg";
            return base.File(path, "image/jpeg");
        }
    }
}
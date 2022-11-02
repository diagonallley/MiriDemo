using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MtlMiriDemo_V1.Controllers
{
    public class AllKartController : Controller
    {
        // GET: AllKart
        public ActionResult Index()
        {
            Session["MerchantId"] = "ALLKART";
            return View();
        }
    }
}
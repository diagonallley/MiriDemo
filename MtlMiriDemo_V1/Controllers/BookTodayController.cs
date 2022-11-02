using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MtlMiriDemo_V1.Controllers
{
    public class BookTodayController : Controller
    {
        // GET: BookToday
        public ActionResult Index()
        {
           
                Session["MerchantId"] = "BookToday";
            return View();
        }
    }
}
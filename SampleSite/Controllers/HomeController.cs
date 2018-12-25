using System;
using System.Web.Mvc;

namespace AspNetSampleSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}

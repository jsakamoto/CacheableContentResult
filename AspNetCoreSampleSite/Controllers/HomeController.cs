using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSampleSite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

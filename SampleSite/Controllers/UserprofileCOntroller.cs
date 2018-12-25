using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Toolbelt.Web;

namespace AspNetSampleSite.Controllers
{
    public class UserprofileController : Controller
    {
        public ActionResult Index(int id)
        {
            ViewBag.UserId = id;
            return View();
        }

        [HttpGet, OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Picture(int id)
        {
            var imagePath = HttpContext.Server.MapPath($"~/App_Data/user/{id}/picture.png");
            var lastWriteTime = System.IO.File.GetLastWriteTimeUtc(imagePath);

            return new CacheableContentResult(
                contentType: "image/png",
                lastModified: lastWriteTime,
                getContent: () =>
                {
                    return System.IO.File.ReadAllBytes(imagePath);
                }
            );
        }
    }
}
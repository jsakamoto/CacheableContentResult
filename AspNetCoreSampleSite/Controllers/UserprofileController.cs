using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Toolbelt.Web;

namespace AspNetCoreSampleSite.Controllers
{
    public class UserprofileController : Controller
    {
        private IHostingEnvironment HostingEnvironment { get; }

        public UserprofileController(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index(int id)
        {
            ViewBag.UserId = id;
            return View();
        }

        [HttpGet]
        public ActionResult Picture(int id)
        {
            var imagePath = Path.Combine(HostingEnvironment.ContentRootPath, "App_Data", "user", id.ToString(), "picture.png");
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
using System;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Toolbelt.Web;

namespace SampleSite.Controllers
{
    using IO = System.IO;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CustomImage()
        {
            var imagePath = HttpContext.Server.MapPath("~/Content/Adelie.png");
            var imageBin = IO.File.ReadAllBytes(imagePath);
            var etag = Convert.ToBase64String(MD5.Create().ComputeHash(imageBin));

            return new CacheableContentResult(
                    cacheability: HttpCacheability.ServerAndPrivate,
                    lastModified: IO.File.GetLastWriteTime(imagePath),
                    etag:etag,
                    contentType: "image/png",
                    getContent: () => imageBin
                );
        }
    }
}

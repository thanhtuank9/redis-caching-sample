using RedisCaching.Service.Services;
using System.Diagnostics;
using System.Web.Mvc;

namespace RedisCachingSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly PropertyCaching _propertyCaching;
        public HomeController()
        {
            _propertyCaching = new PropertyCaching();
        }

        public ActionResult Index()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var data = _propertyCaching.GetPropertiesWithoutCaching();

            stopwatch.Stop();
            var totalMilliseconds = stopwatch.ElapsedMilliseconds;

            ViewBag.TotalMilliseconds = totalMilliseconds;

            return View(data);
        }

        [Route("redis-caching-1")]
        public ActionResult Caching1()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var data = _propertyCaching.GetCachingProperties1();

            stopwatch.Stop();
            var totalMilliseconds = stopwatch.ElapsedMilliseconds;

            ViewBag.TotalMilliseconds = totalMilliseconds;
            ViewBag.IsCaching = true;
            return View("Index", data);
        }

        [Route("redis-caching-2")]
        public ActionResult Caching2()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var data = _propertyCaching.GetCachingProperties2();

            stopwatch.Stop();
            var totalMilliseconds = stopwatch.ElapsedMilliseconds;

            ViewBag.TotalMilliseconds = totalMilliseconds;
            ViewBag.IsCaching = true;
            return View("Index", data);
        }

        public JsonResult ClearCache()
        {
            _propertyCaching.ClearCache();
            return Json(new {status = true, message = "Clear cache successfull"}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
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

        public ActionResult Caching()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var data = _propertyCaching.GetCachingProperties2();

            stopwatch.Stop();
            var totalMilliseconds = stopwatch.ElapsedMilliseconds;

            ViewBag.TotalMilliseconds = totalMilliseconds;

            return View("Index", data);
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
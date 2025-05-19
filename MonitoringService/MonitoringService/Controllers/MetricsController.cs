using Microsoft.AspNetCore.Mvc;

namespace MonitoringService.Controllers
{
    public class MetricsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

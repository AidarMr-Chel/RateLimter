using Microsoft.AspNetCore.Mvc;

namespace MonitoringService.Controllers;

public class LimitsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

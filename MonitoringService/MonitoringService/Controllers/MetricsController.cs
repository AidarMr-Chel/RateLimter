using Microsoft.AspNetCore.Mvc;
using MonitoringService.Services.Absrtacts;

namespace MonitoringService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly IMetricService _metricService;

    public MetricsController(IMetricService metricService)
    {
        _metricService = metricService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMetrics([FromQuery] int rangeSeconds = 60, [FromQuery] int maxCount = 1000)
    {
        var metrics = await _metricService.GetCurrentMetricsAsync(rangeSeconds);
        return Ok(metrics);
    }

}
using Microsoft.AspNetCore.Mvc;
using MonitoringService.Models.loging.modelsDto;
using MonitoringService.Services.Absrtacts;

namespace MonitoringService.Controllers;

[ApiController]
[Route("api/system-events")]
public class SystemEventLogController : ControllerBase
{
    private readonly ISystemEventLogService _service;

    public SystemEventLogController(ISystemEventLogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<SystemEventLogEntryDto>>> GetAll([FromQuery] int minutes = 60)
    {
        var events = await _service.GetRecentEventsAsync(minutes);
        return Ok(events);
    }

    [HttpGet("filter")]
    public async Task<ActionResult<List<SystemEventLogEntryDto>>> Filtered(
        [FromQuery] string? eventType,
        [FromQuery] string? path,
        [FromQuery] int minutes = 60)
    {
        var result = await _service.FilterEventsAsync(eventType, path, minutes);
        return Ok(result);
    }
}
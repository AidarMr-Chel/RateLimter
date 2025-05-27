using Microsoft.AspNetCore.Mvc;
using MonitoringService.Models;
using MonitoringService.Models.loging.modelsDto;
using MonitoringService.Services.Absrtacts;

namespace MonitoringService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ILogService _service;

    public LogsController(ILogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int take = 100)
    {
        var logs = await _service.GetLatestLogsAsync(take);
        return Ok(logs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var log = await _service.GetByIdAsync(id);
        if (log == null) return NotFound($"Log with ID {id} not found");
        return Ok(log);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> Filter([FromQuery] LogFilterDto filter)
    {
        var logs = await _service.GetFilteredLogsAsync(filter);
        return Ok(logs);
    }
}

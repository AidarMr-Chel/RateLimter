using Microsoft.AspNetCore.Mvc;
using MonitoringService.Models.policy.modelsDto;
using MonitoringService.Services.Absrtacts;

namespace MonitoringService.Controllers;

[ApiController]
[Route("api/limits")]
public class PolicyController : ControllerBase
{
    private readonly IPolicyService _service;

    public PolicyController(IPolicyService service)
    {
        _service = service;
    }

    [HttpGet("filters")]
    public async Task<IActionResult> GetAllFilters() =>
        Ok(await _service.GetAllFiltersAsync());

    [HttpGet("filters/{id}")]
    public async Task<IActionResult> GetFilter(string id)
    {
        var filter = await _service.GetFilterAsync(id);
        return filter == null ? NotFound() : Ok(filter);
    }


    [HttpGet("rules")]
    public async Task<IActionResult> GetAllRules() =>
        Ok(await _service.GetAllRulesAsync());

    [HttpGet("rules/{id}")]
    public async Task<IActionResult> GetRule(string id)
    {
        var rule = await _service.GetRuleAsync(id);
        return rule == null ? NotFound() : Ok(rule);
    }

    [HttpPost("rules")]
    public async Task<IActionResult> SaveRule([FromBody] RuleCreateDto ruleCreate)
    {
        var result = await _service.SaveRuleAsync(ruleCreate.Rule, ruleCreate.Filter);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("rules/{id}")]
    public async Task<IActionResult> DeleteRule(string id)
    {
        await _service.DeleteRuleAsync(id);
        return NoContent();
    }
}


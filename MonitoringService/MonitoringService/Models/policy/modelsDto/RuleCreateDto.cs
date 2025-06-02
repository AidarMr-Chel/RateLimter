using ApiGateway.RateLimiting.configPolicy.modelsDto;

namespace MonitoringService.Models.policy.modelsDto;

public class RuleCreateDto
{
    public RateLimitRuleDto Rule { get; set; } = default!;
    public FilterDto Filter { get; set; } = default!;
}

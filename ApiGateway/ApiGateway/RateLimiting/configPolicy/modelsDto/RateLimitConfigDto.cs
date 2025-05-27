namespace ApiGateway.RateLimiting.configPolicy.modelsDto;

public class RateLimitConfigDto
{
    public List<FilterDto> Filters { get; set; } = new();
    public List<RateLimitRuleDto> Rules { get; set; } = new();
}

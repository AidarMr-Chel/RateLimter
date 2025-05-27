using ApiGateway.RateLimiting.configPolicy.modelsDto;

namespace ApiGateway.RateLimiting.configPolicy.abstracts;

public interface IRateLimitRuleMatcher
{
    Task<(RateLimitRuleDto, FilterDto)?> FindMatchingRule(HttpContext context);
}

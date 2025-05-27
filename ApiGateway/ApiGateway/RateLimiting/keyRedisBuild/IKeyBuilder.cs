using ApiGateway.Logging.models;
using ApiGateway.RateLimiting.configPolicy.models;
using ApiGateway.RateLimiting.configPolicy.modelsDto;

namespace ApiGateway.RateLimiting.keyBuild;

public interface IKeyBuilder
{
    string BuildKey(RateLimitRuleDto rule, FilterDto filter, HttpContext context);
    string BuildRuleId(RateLimitRuleDto rule, FilterDto filter, HttpContext context);
    RuleDetails BuildRuleDetails(RateLimitRuleDto rule, FilterDto filter, HttpContext context);
}


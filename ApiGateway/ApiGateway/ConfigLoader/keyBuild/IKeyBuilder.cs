using ApiGateway.ConfigLoader.models;
using ApiGateway.Logging.models;

namespace ApiGateway.ConfigLoader.keyBuild;

public interface IKeyBuilder
{
    string BuildKey(RateLimitRule rule, HttpContext context);
    string BuildRuleId(RateLimitRule rule, HttpContext context);
    RuleDetails BuildRuleDetails(RateLimitRule rule, HttpContext context);
}

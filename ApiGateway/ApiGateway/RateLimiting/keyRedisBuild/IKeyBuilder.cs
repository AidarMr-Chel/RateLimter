using ApiGateway.Logging.models;
using ApiGateway.RateLimiting.configPolicy.models;
using ApiGateway.RateLimiting.configPolicy.modelsDto;

namespace ApiGateway.RateLimiting.keyBuild;

public interface IKeyBuilder
{
    string BuildKey(FilterDto filter);
    string BuildRedisHash(FilterDto filter);
    RuleDetails BuildRuleDetails(RateLimitRuleDto rule, FilterDto filter);
}


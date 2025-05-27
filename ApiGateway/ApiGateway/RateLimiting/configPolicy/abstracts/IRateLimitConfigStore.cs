using ApiGateway.RateLimiting.configPolicy.modelsDto;

namespace ApiGateway.RateLimiting.configPolicy.abstracts;

public interface IRateLimitConfigStore
{
    Task<IEnumerable<RateLimitRuleDto>> GetAllRulesAsync();
    Task<IEnumerable<FilterDto>> GetAllFiltersAsync();

}

using ApiGateway.RateLimiting.configPolicy.modelsDto;

namespace MonitoringService.Repositories.Abstracts;

public interface IPolicyRepository
{
    Task<IEnumerable<FilterDto>> GetAllFiltersAsync();
    Task<FilterDto?> GetFilterAsync(string id);
    Task<IEnumerable<RateLimitRuleDto>> GetAllRulesAsync();
    Task<RateLimitRuleDto?> GetRuleAsync(string id);
    Task SaveRuleAsync(RateLimitRuleDto rule, FilterDto filter);
    Task DeleteRuleAsync(string id);
}
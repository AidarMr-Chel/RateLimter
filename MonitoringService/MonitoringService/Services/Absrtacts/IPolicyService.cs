using ApiGateway.RateLimiting.configPolicy.modelsDto;

namespace MonitoringService.Services.Absrtacts;

public interface IPolicyService
{
    Task<IEnumerable<FilterDto>> GetAllFiltersAsync();
    Task<FilterDto?> GetFilterAsync(string id);
    Task SaveFilterAsync(FilterDto filter);
    Task DeleteFilterAsync(string id);

    Task<IEnumerable<RateLimitRuleDto>> GetAllRulesAsync();
    Task<RateLimitRuleDto?> GetRuleAsync(string id);
    Task SaveRuleAsync(RateLimitRuleDto rule);
    Task DeleteRuleAsync(string id);
}

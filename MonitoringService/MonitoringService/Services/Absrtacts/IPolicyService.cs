using ApiGateway.RateLimiting.configPolicy.modelsDto;
using MonitoringService.Models.policy.modelsDto;

namespace MonitoringService.Services.Absrtacts;

public interface IPolicyService
{
    Task<IEnumerable<FilterDto>> GetAllFiltersAsync();
    Task<FilterDto?> GetFilterAsync(string id);
    Task<IEnumerable<RateLimitRuleDto>> GetAllRulesAsync();
    Task<RateLimitRuleDto?> GetRuleAsync(string id);
    Task<ValidationResultDto> SaveRuleAsync(RateLimitRuleDto rule, FilterDto filter);
    Task DeleteRuleAsync(string id);
}

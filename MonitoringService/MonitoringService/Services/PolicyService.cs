using ApiGateway.RateLimiting.configPolicy.modelsDto;
using ApiGateway.RateLimiting.configPolicy.validation;
using MonitoringService.Repositories.Abstracts;
using MonitoringService.Services.Absrtacts;
using System.ComponentModel.DataAnnotations;

namespace MonitoringService.Services;

public class PolicyService : IPolicyService
{
    private readonly IPolicyRepository _repository;
    private readonly FilterValidator _filterValidator;
    private readonly RuleValidator _ruleValidator;

    public PolicyService(
        IPolicyRepository repository,
        FilterValidator filterValidator,
        RuleValidator ruleValidator)
    {
        _repository = repository;
        _filterValidator = filterValidator;
        _ruleValidator = ruleValidator;
    }

    public async Task<IEnumerable<FilterDto>> GetAllFiltersAsync() =>
        await _repository.GetAllFiltersAsync();

    public async Task<FilterDto?> GetFilterAsync(string id) =>
        await _repository.GetFilterAsync(id);

    public async Task SaveFilterAsync(FilterDto filter)
    {
        var result = await _filterValidator.ValidateAsync(filter);
        if (!result.IsValid)
            throw new ValidationException("Validation Exception");

        await _repository.SaveFilterAsync(filter);
    }

    public async Task DeleteFilterAsync(string id) =>
        await _repository.DeleteFilterAsync(id);

    public async Task<IEnumerable<RateLimitRuleDto>> GetAllRulesAsync() =>
        await _repository.GetAllRulesAsync();

    public async Task<RateLimitRuleDto?> GetRuleAsync(string id) =>
        await _repository.GetRuleAsync(id);

    public async Task SaveRuleAsync(RateLimitRuleDto rule)
    {
        var result = await _ruleValidator.ValidateAsync(rule);
        if (!result.IsValid)
            throw new ValidationException("Validation Exception");

        var filter = await _repository.GetFilterAsync(rule.FilterId);
        if (filter == null)
            throw new Exception($"FilterId {rule.FilterId} not found");

        await _repository.SaveRuleAsync(rule);
    }

    public async Task DeleteRuleAsync(string id) =>
        await _repository.DeleteRuleAsync(id);
}

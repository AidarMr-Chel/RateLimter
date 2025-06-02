using ApiGateway.RateLimiting.configPolicy.modelsDto;
using ApiGateway.RateLimiting.configPolicy.validation;
using MonitoringService.Models.policy.modelsDto;
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

    public async Task<IEnumerable<RateLimitRuleDto>> GetAllRulesAsync() =>
        await _repository.GetAllRulesAsync();

    public async Task<RateLimitRuleDto?> GetRuleAsync(string id) =>
        await _repository.GetRuleAsync(id);

    public async Task<ValidationResultDto> SaveRuleAsync(RateLimitRuleDto rule, FilterDto filter)
    {
        var filterResult = await _filterValidator.ValidateAsync(filter);
        var ruleResult = await _ruleValidator.ValidateAsync(rule);

        var errors = new List<string>();

        if (!filterResult.IsValid)
            errors.AddRange(filterResult.Errors.Select(e => $"[Filter] {e.ErrorMessage}"));

        if (!ruleResult.IsValid)
            errors.AddRange(ruleResult.Errors.Select(e => $"[Rule] {e.ErrorMessage}"));

        if (errors.Any())
            return new ValidationResultDto { IsSuccess = false, Errors = errors };

        await _repository.SaveRuleAsync(rule, filter);
        return new ValidationResultDto { IsSuccess = true };

    }

    public async Task DeleteRuleAsync(string id) =>
        await _repository.DeleteRuleAsync(id);
}

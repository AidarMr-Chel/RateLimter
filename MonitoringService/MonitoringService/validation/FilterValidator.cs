using ApiGateway.RateLimiting.configPolicy.modelsDto;
using FluentValidation;

namespace ApiGateway.RateLimiting.configPolicy.validation;

public class FilterValidator : AbstractValidator<FilterDto>
{
    public FilterValidator()
    {
        RuleFor(f => f.Id)
            .NotEmpty().WithMessage("Filter Id is required");

        RuleFor(f => f)
            .Must(HaveAtLeastOneFilter)
            .WithMessage("At least one filter condition (e.g., IP, Region, Path) must be specified");
    }

    private bool HaveAtLeastOneFilter(FilterDto filter)
    {
        return (filter.Ip?.Any() ?? false)
            || (filter.Region?.Any() ?? false)
            || (filter.Country?.Any() ?? false)
            || (filter.UserAgent?.Any() ?? false)
            || (filter.HttpMethod?.Any() ?? false)
            || (filter.Path?.Any() ?? false)
            || (filter.ApiKey?.Any() ?? false)
            || (filter.ClientId?.Any() ?? false)
            || (filter.UserId?.Any() ?? false)
            || (filter.DeviceType?.Any() ?? false);
    }
}

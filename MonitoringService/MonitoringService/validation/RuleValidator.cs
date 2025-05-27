using FluentValidation;
using ApiGateway.RateLimiting.configPolicy.modelsDto;

namespace ApiGateway.RateLimiting.configPolicy.validation;

public class RuleValidator : AbstractValidator<RateLimitRuleDto>
{
    public RuleValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty().WithMessage("Rule Id is required");

        RuleFor(r => r.FilterId)
            .NotEmpty().WithMessage("FilterId is required");

        RuleFor(r => r.Limit)
            .GreaterThan(0).WithMessage("Limit must be > 0");

        RuleFor(r => r.PeriodSeconds)
            .GreaterThan(0).WithMessage("PeriodSeconds must be > 0");

        RuleFor(r => r.StrategyName)
            .NotEmpty().WithMessage("StrategyName is required")
            .Must(BeValidStrategy)
            .WithMessage("Unsupported StrategyName");
    }

    private bool BeValidStrategy(string strategy)
    {
        return strategy is "FixedWindow" or "SlidingWindow" or "TokenBucket";
    }
}

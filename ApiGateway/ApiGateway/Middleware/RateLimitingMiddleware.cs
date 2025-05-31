using ApiGateway.Logging.abstracts;
using ApiGateway.RateLimiting.configPolicy.abstracts;
using ApiGateway.RateLimiting.core;
using ApiGateway.RateLimiting.core.abstracts;
using ApiGateway.RateLimiting.keyBuild;

namespace ApiGateway.Middleware;

public class RateLimitingMiddleware
{
    private readonly IRateLimitingStrategySelector _strategies;
    private readonly IRateLimitRuleMatcher _ruleMatcher;
    private readonly IKeyBuilder _keyBuilder;
    private readonly IMasterLogService _logService;

    public RateLimitingMiddleware(
        IRateLimitingStrategySelector strategies,
        IRateLimitRuleMatcher ruleMatcher,
        IKeyBuilder keyBuilder,
        IMasterLogService logService)
    {
        _strategies = strategies;
        _ruleMatcher = ruleMatcher;
        _keyBuilder = keyBuilder;
        _logService = logService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var policy = await _ruleMatcher.FindMatchingRule(context);

        if (policy is not (var rule, var filter))
        {
            await next(context);
            return;
        }

        var key = _keyBuilder.BuildKey(filter);

        var rateLimitContext = new RateLimitRequestContext
        {
            Key = key,
            Limit = rule.Limit,
            Period = rule.Period,
        };

        var allowed = await _strategies
            .GetStrategy(rule.StrategyName)
            .IsRequestAllowedAsync(rateLimitContext);

        if (!allowed) 
        { 
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            var redisKey = _keyBuilder.BuildRedisHash(filter);
            var ruleDetails = _keyBuilder.BuildRuleDetails(rule, filter);
            ruleDetails.RuleId = rule.Id;
            await _logService.LogAsync(
                context,
                StatusCodes.Status429TooManyRequests,
                "RateLimitExceeded",
                redisKey,
                ruleDetails
            );

            context.Items["RateLimitExceeded"] = true;
            return;
        }

        await next(context);
    }
}

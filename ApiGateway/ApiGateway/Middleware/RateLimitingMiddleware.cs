using ApiGateway.ConfigLoader;
using ApiGateway.ConfigLoader.keyBuild;
using ApiGateway.Logging;
using ApiGateway.Logging.abstracts;
using ApiGateway.RateLimiting.core;

namespace ApiGateway.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitingStrategySelector _strategies;
    private readonly IRateLimitConfigProvider _providerConfig;
    private readonly IKeyBuilder _keyBuilder;
    private readonly IMasterLogService _logService;

    public RateLimitingMiddleware(
        RequestDelegate next, 
        IRateLimitingStrategySelector strategies, 
        IRateLimitConfigProvider providerConfig,
        IKeyBuilder keyBuilder,
        IMasterLogService logService)
    {
        _next = next;
        _strategies = strategies;
        _providerConfig = providerConfig;
        _keyBuilder = keyBuilder;
        _logService = logService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var rule = await _providerConfig.GetRule(context);
        var key = _keyBuilder.BuildKey(rule, context);

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
            var ruleId = _keyBuilder.BuildRuleId(rule, context);
            var ruleDetails = _keyBuilder.BuildRuleDetails(rule, context);
            await _logService.LogAsync(
                context,
                StatusCodes.Status429TooManyRequests,
                "RateLimitExceeded",
                ruleId,
                ruleDetails
            );

            context.Items["RateLimitExceeded"] = true;
            return;
        }

        await _next(context);
    }
}

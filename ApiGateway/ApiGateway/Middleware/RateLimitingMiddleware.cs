using ApiGateway.ConfigLoader;
using ApiGateway.ConfigLoader.keyBuild;
using ApiGateway.RateLimiting.core;

namespace ApiGateway.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitingStrategySelector _strategies;
    private readonly IRateLimitConfigProvider _providerConfig;
    private readonly IKeyBuilder _keyBuilder;

    public RateLimitingMiddleware(
        RequestDelegate next, 
        IRateLimitingStrategySelector strategies, 
        IRateLimitConfigProvider providerConfig,
        IKeyBuilder keyBuilder)
    {
        _next = next;
        _strategies = strategies;
        _providerConfig = providerConfig;
        _keyBuilder = keyBuilder;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var rule = _providerConfig.GetRule(context);
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
            await context.Response.WriteAsync("Too Many Requests (rate limit exceeded)");
            return;
        }

        await _next(context);
    }

}

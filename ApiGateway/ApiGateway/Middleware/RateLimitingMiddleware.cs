using ApiGateway.ConfigLoader;
using ApiGateway.RateLimiting.core;

namespace ApiGateway.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitingStrategySelector _strategies;
    private readonly IRateLimitConfigProvider _providerConfig;

    public RateLimitingMiddleware(
        RequestDelegate next, 
        IRateLimitingStrategySelector strategies, 
        IRateLimitConfigProvider providerConfig)
    {
        _next = next;
        _strategies = strategies;
        _providerConfig = providerConfig;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var region = context.Request.Headers["X-Region"].ToString();
        var rule = _providerConfig.GetRule(region);

        var rateLimitContext = new RateLimitRequestContext
        {
            Key = ip,
            Limit = rule.Limit,
            Period = rule.Period,
        };

        var allowed = await _strategies
            .GetStrategy(rule.StrategyName)
            .IsRequestAllowedAsync(rateLimitContext);

        Console.WriteLine(rule.StrategyName);
        if (!allowed) 
        { 
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too Many Requests (rate limit exceeded)");
            return;
        }

        await _next(context);
    }

}

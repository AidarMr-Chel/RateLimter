using ApiGateway.ConfigLoader;
using ApiGateway.RateLimiting;

namespace ApiGateway.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitingStrategy _strategy;
    private readonly IRateLimitConfigProvider _providerConfig;

    public RateLimitingMiddleware(
        RequestDelegate next, 
        IRateLimitingStrategy strategy, 
        IRateLimitConfigProvider providerConfig)
    {
        _next = next;
        _strategy = strategy;
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
        Console.WriteLine(region);
        Console.WriteLine(rateLimitContext.Limit);
        Console.WriteLine(rateLimitContext.Period);

        var allowed = await _strategy.IsRequestAllowedAsync(rateLimitContext);
        if (!allowed) 
        { 
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too Many Requests (rate limit exceeded)");
            return;
        }

        await _next(context);
    }

}

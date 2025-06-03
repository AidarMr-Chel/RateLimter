using ApiGateway.Metrics;

namespace ApiGateway.Middleware;

public class RpsCountingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRpsAggregator _aggregator;

    public RpsCountingMiddleware(RequestDelegate next, IRpsAggregator aggregator)
    {
        _next = next;
        _aggregator = aggregator;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
        await _aggregator.IncrementAsync();
    }
}

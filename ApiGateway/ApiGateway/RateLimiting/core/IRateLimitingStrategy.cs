namespace ApiGateway.RateLimiting.core;

public interface IRateLimitingStrategy
{
    string Name { get; }
    Task<bool> IsRequestAllowedAsync(RateLimitRequestContext context);
}

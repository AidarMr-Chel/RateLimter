namespace ApiGateway.RateLimiting.core.abstracts;

public interface IRateLimitingStrategy
{
    string Name { get; }
    Task<bool> IsRequestAllowedAsync(RateLimitRequestContext context);
}

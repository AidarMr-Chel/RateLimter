namespace ApiGateway.RateLimiting;

public interface IRateLimitingStrategy
{
    Task<bool> IsRequestAllowedAsync(RateLimitRequestContext context);
}

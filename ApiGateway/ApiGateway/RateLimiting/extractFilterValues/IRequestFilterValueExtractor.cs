namespace ApiGateway.RateLimiting.extractFilterValues;

public interface IRequestFilterValueExtractor
{
    string? Extract(string key, HttpContext context);
}

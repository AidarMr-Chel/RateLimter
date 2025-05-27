namespace ApiGateway.RateLimiting.extractFilterValues;

public class DefaultRequestFilterValueExtractor : IRequestFilterValueExtractor
{
    public string? Extract(string key, HttpContext context)
    {
        return key.ToLowerInvariant() switch
        {
            "ip" => context.Connection.RemoteIpAddress?.ToString(),
            "region" => context.Request.Headers["X-Region"].FirstOrDefault(),
            "country" => context.Request.Headers["X-Country"].FirstOrDefault(),
            "userid" => context.User.Identity?.IsAuthenticated == true
                          ? context.User.Identity.Name
                          : null,
            "apikey" => context.Request.Headers["X-Api-Key"].FirstOrDefault(),
            "clientid" => context.Request.Headers["X-Client-Id"].FirstOrDefault(),
            "useragent" => context.Request.Headers["User-Agent"].FirstOrDefault(),
            "path" => context.Request.Path.ToString(),
            "httpmethod" => context.Request.Method,
            "devicetype" => context.Request.Headers["X-Device-Type"].FirstOrDefault(),
            _ => null
        };
    }

}

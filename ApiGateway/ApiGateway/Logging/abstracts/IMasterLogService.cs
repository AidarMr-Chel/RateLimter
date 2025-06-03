using ApiGateway.Logging.models;

namespace ApiGateway.Logging.abstracts;

public interface IMasterLogService
{
    Task LogAsync(HttpContext context, int statusCode, string reason, string? redisHash = null, RuleDetails? rule = null, int? durationMs = null);
    Task LogInternalEventAsync(string type, string message, string? path = null, string? instance = null, DateTime? timestamp = null);

}


using ApiGateway.Logging.models;

namespace ApiGateway.Logging.abstracts;

public interface IMasterLogService
{
    Task LogAsync(HttpContext context, int statusCode, string reason, string? ruleId = null, RuleDetails? rule = null, int? durationMs = null);
}


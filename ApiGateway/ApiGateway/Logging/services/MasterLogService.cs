using ApiGateway.Logging.abstracts;
using ApiGateway.Logging.models;

namespace ApiGateway.Logging.services;

public class MasterLogService : IMasterLogService
{
    private readonly ILogWriter _writer;

    public MasterLogService(ILogWriter writer)
    {
        _writer = writer;
    }

    public async Task LogAsync(HttpContext context, int statusCode, string reason, string? ruleId = null, RuleDetails? rule = null, int? durationMs = null)
    {
        var log = new LogEntry
        {
            Timestamp = DateTime.UtcNow,
            Ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            Region = context.Request.Headers["X-Region"].FirstOrDefault() ?? "unknown",
            Path = context.Request.Path,
            Method = context.Request.Method,
            StatusCode = statusCode,
            Reason = reason,
            RuleId = ruleId,
            Rule = rule,
            DurationMs = durationMs,
            Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
        };

        await _writer.WriteAsync(log);
    }
}

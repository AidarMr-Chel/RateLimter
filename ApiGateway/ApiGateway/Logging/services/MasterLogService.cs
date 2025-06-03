using ApiGateway.Logging.abstracts;
using ApiGateway.Logging.models;

namespace ApiGateway.Logging.services;

public class MasterLogService : IMasterLogService
{
    private readonly ILogWriter _writer;
    private readonly string _instanceId;

    public MasterLogService(ILogWriter writer, IConfiguration config)
    {
        _writer = writer;
        _instanceId = config["Gateway:InstanceId"] ?? "unknown";
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
            Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            InstanceId = _instanceId
            
        };

        await _writer.WriteAsync<LogEntry>(log);
    }

    public async Task LogInternalEventAsync(string type, string message, string? path = null, string? instance = null, DateTime? timestamp = null)
    {
        var evt = new SystemEventLogEntry
        {
            Timestamp = timestamp ?? DateTime.UtcNow,
            EventType = type,
            Message = message,
            Path = path,
            InstanceId = instance ?? _instanceId
        };

        await _writer.WriteAsync<SystemEventLogEntry>(evt);
    }



}

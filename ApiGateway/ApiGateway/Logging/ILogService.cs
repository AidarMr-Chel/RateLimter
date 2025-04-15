namespace ApiGateway.Logging;

public interface ILogService
{
    Task LogAsync(RateLimitLogEntry entry);
}

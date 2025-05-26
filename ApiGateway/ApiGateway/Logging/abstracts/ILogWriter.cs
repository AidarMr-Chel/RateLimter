using ApiGateway.Logging.models;

namespace ApiGateway.Logging.abstracts;

public interface ILogWriter
{
    Task WriteAsync(LogEntry entry);
}

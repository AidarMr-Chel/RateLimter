using ApiGateway.Logging.models;

namespace ApiGateway.Logging.abstracts;

public interface ILogWriter
{
    Task WriteAsync<T>(T entry);
}

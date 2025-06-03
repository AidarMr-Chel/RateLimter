namespace ApiGateway.Metrics;

public interface IRpsAggregator
{
    Task IncrementAsync();
}

using StackExchange.Redis;

namespace ApiGateway.Metrics;

public class RpsAggregator : IRpsAggregator
{
    private readonly IDatabase _redis;
    private readonly string _key;

    public RpsAggregator(IConnectionMultiplexer connection)
    {
        _redis = connection.GetDatabase();
        _key = "rps:total";
    }

    public async Task IncrementAsync()
    {
        await _redis.StringIncrementAsync(_key);
    }
}

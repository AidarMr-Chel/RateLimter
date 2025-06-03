using MonitoringService.Repositories.Abstracts;
using StackExchange.Redis;

namespace MonitoringService.Repositories;

public class RpsMetricsRepository : IRpsMetricsRepository
{
    private readonly IDatabase _redis;

    public RpsMetricsRepository(IConnectionMultiplexer connection)
    {
        _redis = connection.GetDatabase();
    }

    public async Task<int> GetAndResetRpsAsync(string key = "rps:total")
    {
        var value = await _redis.StringGetAsync(key);
        await _redis.StringSetAsync(key, 0);
        return (int)value;
    }
}

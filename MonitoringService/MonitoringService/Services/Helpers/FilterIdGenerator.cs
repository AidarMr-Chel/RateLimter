using ApiGateway.RateLimiting.configPolicy.modelsDto;
using System.Reflection;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;

namespace MonitoringService.Services.Helpers;

public static class FilterIdGenerator
{
    public static string ComputeHash(FilterDto filter)
    {
        var map = typeof(FilterDto)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(List<string>) && p.GetValue(filter) is List<string> list && list.Count > 0)
            .ToDictionary(
                p => p.Name.ToLowerInvariant(),
                p => string.Join(",", (List<string>)p.GetValue(filter)!));

        var sorted = map.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        var json = JsonSerializer.Serialize(sorted);

        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hash);
    }
}
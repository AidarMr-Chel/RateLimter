using ApiGateway.RateLimiting.configPolicy.abstracts;
using ApiGateway.RateLimiting.configPolicy.modelsDto;
using ApiGateway.RateLimiting.extractFilterValues;

namespace ApiGateway.RateLimiting.configPolicy.matching;

public class DefaultRuleMatcher : IRateLimitRuleMatcher
{
    private readonly IRateLimitConfigStore _store;
    private readonly IRequestFilterValueExtractor _extractor;

    public DefaultRuleMatcher(
        IRateLimitConfigStore store,
        IRequestFilterValueExtractor extractor)
    {
        _store = store;
        _extractor = extractor;
    }

    public async Task<(RateLimitRuleDto, FilterDto)?> FindMatchingRule(HttpContext context)
    {
        var filters = (await _store.GetAllFiltersAsync()).ToDictionary(f => f.Id);
        var rules = await _store.GetAllRulesAsync();

        foreach (var rule in rules.OrderByDescending(r => r.Limit))
        {
            if (!filters.TryGetValue(rule.FilterId, out var filter))
                continue;

            if (IsMatch(filter, context))
                return (rule, filter);
        }

        return null;
    }

    private bool IsMatch(FilterDto filter, HttpContext context)
    {
        return Match("ip", filter.Ip)
            && Match("region", filter.Region)
            && Match("country", filter.Country)
            && Match("user-agent", filter.UserAgent)
            && Match("httpmethod", filter.HttpMethod)
            && Match("path", filter.Path)
            && Match("apikey", filter.ApiKey)
            && Match("clientid", filter.ClientId)
            && Match("userid", filter.UserId)
            && Match("devicetype", filter.DeviceType);

        bool Match(string key, List<string>? expected)
        {
            if (expected == null || expected.Count == 0) return true;
            var actual = _extractor.Extract(key, context);
            return actual != null && expected.Contains(actual, StringComparer.OrdinalIgnoreCase);
        }
    }
}

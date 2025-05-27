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
        var filtersList = await _store.GetAllFiltersAsync();
        var rules = await _store.GetAllRulesAsync();

        var filters = filtersList
            .ToDictionary(f => NormalizeKey(f.Id), StringComparer.OrdinalIgnoreCase);

        foreach (var rule in rules.OrderByDescending(r => r.Limit))
        {
            var fid = NormalizeKey(rule.FilterId);

            if (!filters.TryGetValue(fid, out var filter))
                continue;

            CleanFilter(filter);

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
            && Match("useragent", filter.UserAgent)
            && Match("httpmethod", filter.HttpMethod)
            && Match("path", filter.Path)
            && Match("apikey", filter.ApiKey)
            && Match("clientid", filter.ClientId)
            && Match("userid", filter.UserId)
            && Match("devicetype", filter.DeviceType);

        bool Match(string key, List<string>? expected)
        {
            if (expected == null || expected.Count == 0)
                return true;

            var actual = _extractor.Extract(key, context);
            return actual != null && expected.Contains(actual, StringComparer.OrdinalIgnoreCase);
        }
    }

    private void CleanFilter(FilterDto filter)
    {
        filter.Ip = Clean(filter.Ip);
        filter.Region = Clean(filter.Region);
        filter.Country = Clean(filter.Country);
        filter.UserAgent = Clean(filter.UserAgent);
        filter.HttpMethod = Clean(filter.HttpMethod);
        filter.Path = Clean(filter.Path);
        filter.ApiKey = Clean(filter.ApiKey);
        filter.ClientId = Clean(filter.ClientId);
        filter.UserId = Clean(filter.UserId);
        filter.DeviceType = Clean(filter.DeviceType);
    }

    private List<string>? Clean(List<string>? input)
    {
        return input?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }

    private static string NormalizeKey(string key)
    {
        return key?.Trim().ToLowerInvariant().Replace("\u200B", "") ?? "";
    }
}

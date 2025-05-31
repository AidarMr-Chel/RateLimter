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
        var filterProps = typeof(FilterDto)
        .GetProperties()
        .Where(p => p.PropertyType == typeof(List<string>));

        foreach (var prop in filterProps)
        {
            var key = prop.Name.ToLowerInvariant(); 
            var expected = prop.GetValue(filter) as List<string>;

            if (!Match(key, expected))
                return false;
        }

        return true;

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
        var props = typeof(FilterDto)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(List<string>));

        foreach (var prop in props)
        {
            var value = prop.GetValue(filter) as List<string>;
            prop.SetValue(filter, Clean(value));
        }
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

namespace ApiGateway.ConfigLoader.extractFilterValues;

public interface IRequestFilterValueExtractor
{
    string? Extract(string key, HttpContext context);
}

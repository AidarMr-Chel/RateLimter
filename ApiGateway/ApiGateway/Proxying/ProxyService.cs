using ApiGateway.Logging.abstracts;
using ApiGateway.Options;
using Microsoft.Extensions.Options;
using Polly.CircuitBreaker;
using System.Diagnostics;

namespace ApiGateway.Proxying;

public class ProxyService : IProxyService
{
    private readonly IHttpClientFactory _factory;
    private readonly IMasterLogService _logService;
    private readonly IOptions<ProxyLogOptions> _options;
    private readonly IOptions<BackendPathOptions> _optionsPath;

    public ProxyService(
        IHttpClientFactory factory, 
        IMasterLogService logService, 
        IOptions<ProxyLogOptions> options,
        IOptions<BackendPathOptions> optionsPath)
    {
        _factory = factory;
        _logService = logService;
        _options = options;
        _optionsPath = optionsPath;
    }

    public async Task<HttpResponseMessage> ForwardAsync(HttpContext context)
    {
        if (context.Items.ContainsKey("RateLimitExceeded"))
        {
            throw new HttpRequestException("Rate limit exceeded manually");
        }

        var client = _factory.CreateClient("UserApiClient");
        var targetPath = context.Request.Path.Value?.Replace("/proxy", "") ?? "";
        var settingPath = _optionsPath.Value.Path;

        var request = new HttpRequestMessage
        {
            Method = new HttpMethod(context.Request.Method),
            RequestUri = new Uri(settingPath + targetPath + context.Request.QueryString)
        };

        if (context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();
            request.Content = new StreamContent(context.Request.Body);
            context.Request.Body.Position = 0;
        }

        foreach (var header in context.Request.Headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        if (context.Items.TryGetValue("UserId", out var userId))
        {
            request.Headers.Add("X-User-Id", userId?.ToString());
        }

        if (context.Items.TryGetValue("UserRole", out var role))
        {
            request.Headers.Add("X-User-Role", role?.ToString());
        }

        var sw = Stopwatch.StartNew();
        HttpResponseMessage response;

        try
        {
            response = await client.SendAsync(request);
        }
        catch (BrokenCircuitException)
        {
            sw.Stop();
            await _logService.LogAsync(
                context,
                503,
                "CircuitBreaker: request blocked",
                durationMs: (int)sw.ElapsedMilliseconds
            );

            context.Response.StatusCode = 503;
            await context.Response.WriteAsync("Service temporarily unavailable (circuit open)");
            return new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable);
        }
        catch (HttpRequestException ex)
        {
            await _logService.LogAsync(context, 502, $"HttpRequestException: {ex.Message}");
            context.Response.StatusCode = 502;
            await context.Response.WriteAsync("Bad Gateway: " + ex.Message);
            return new HttpResponseMessage(System.Net.HttpStatusCode.BadGateway);
        }


        sw.Stop();

        var status = (int)response.StatusCode;
        var setting = _options.Value;

        if (status == 200 && !setting.LogOkResponses)
            return response;

        await _logService.LogAsync(
            context,
            status,
            status switch
            {
                401 => "Unauthorized (from backend)",
                403 => "Forbidden (from backend)",
                >= 500 => "UpstreamError",
                _ => "OK"
            },
            durationMs: (int)sw.ElapsedMilliseconds
        );

        return response;
    }

}

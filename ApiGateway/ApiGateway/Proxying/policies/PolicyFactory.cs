using ApiGateway.Logging.abstracts;
using Polly.Extensions.Http;
using Polly;

namespace ApiGateway.Proxying.policies;

public class PolicyFactory
{
    private readonly IMasterLogService _logService;
    private readonly IHttpContextAccessor _accessor;

    public PolicyFactory(IMasterLogService logService, IHttpContextAccessor accessor)
    {
        _logService = logService;
        _accessor = accessor;
    }

    public IAsyncPolicy<HttpResponseMessage> CreatePolicy()
        => Policy.WrapAsync(GetRetryPolicy(), GetCircuitBreakerPolicy());

    private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(resp => (int)resp.StatusCode == 429)
            .WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromMilliseconds(200 * retryAttempt),
                onRetry: async (outcome, delay, retryCount, _) =>
                {
                    var ctx = _accessor.HttpContext;
                    if (ctx != null)
                    {
                        await _logService.LogAsync(ctx, 0, $"RetryAttempt #{retryCount}: {outcome.Result?.StatusCode}");
                    }
                });

    private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        => Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(resp => (int)resp.StatusCode >= 500)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: async (outcome, timespan) =>
                {
                    var ctx = _accessor.HttpContext;
                    if (ctx != null)
                    {
                        await _logService.LogAsync(ctx, 503, $"CircuitBreakerOpen: retry in {timespan.TotalSeconds:F0}s");
                    }
                },
                onReset: () => { },
                onHalfOpen: () => { });
}

using ApiGateway.Logging.abstracts;
using System.Text.Json;

namespace ApiGateway.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMasterLogService _logService;

    public ExceptionMiddleware(RequestDelegate next, IMasterLogService logService)
    {
        _next = next;
        _logService = logService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); 
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        await _logService.LogAsync(
            context,
            StatusCodes.Status500InternalServerError,
            "UnhandledException"
        );

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var result = JsonSerializer.Serialize(new
        {
            status = 500,
            error = "Internal Server Error"
        });

        await context.Response.WriteAsync(result);
    }
}


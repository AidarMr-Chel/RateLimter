using ApiGateway.Logging.abstracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ApiGateway.Middleware;

public class JwtAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMasterLogService _logService;
    private readonly TokenValidationParameters _tokenParams;

    public JwtAuthMiddleware(
        RequestDelegate next,
        IMasterLogService logService,
        TokenValidationParameters tokenParams)
    {
        _next = next;
        _logService = logService;
        _tokenParams = tokenParams;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            await _next(context);
            return;
        }

        var token = authHeader["Bearer ".Length..];

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, _tokenParams, out _);

            var userId = principal.FindFirst("userId")?.Value ?? "unknown";
            var role = principal.FindFirst("role")?.Value ?? "guest";

            context.Items["UserId"] = userId;
            context.Items["UserRole"] = role;
        }
        catch (SecurityTokenExpiredException)
        {
            await _logService.LogAsync(context, 401, "TokenExpired");

            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token expired");
            return;
        }
        catch (Exception ex)
        {
            await _logService.LogAsync(context, 403, $"InvalidToken: {ex.Message}");

            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Invalid token");
            return;
        }

        await _next(context);
    }
}

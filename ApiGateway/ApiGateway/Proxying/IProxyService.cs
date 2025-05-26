namespace ApiGateway.Proxying;

public interface IProxyService
{
    Task<HttpResponseMessage> ForwardAsync(HttpContext context);
}


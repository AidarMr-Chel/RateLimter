using ApiGateway.Proxying;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("proxy/{**catchAll}")]
public class ProxyController : ControllerBase
{
    private readonly IProxyService _proxy;

    public ProxyController(IProxyService proxy)
    {
        _proxy = proxy;
    }

    [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch]
    public async Task ProxyAll()
    {
        HttpResponseMessage response;

        try
        {
            response = await _proxy.ForwardAsync(HttpContext);
        }
        catch (Exception ex)
        {
            HttpContext.Response.StatusCode = 500;
            await HttpContext.Response.WriteAsJsonAsync(new
            {
                status = 500,
                error = "Proxy exception",
                detail = ex.Message
            });
            return;
        }

        HttpContext.Response.StatusCode = (int)response.StatusCode;

        foreach (var header in response.Headers)
            HttpContext.Response.Headers[header.Key] = header.Value.ToArray();

        HttpContext.Response.Headers.Remove("transfer-encoding");
        HttpContext.Response.Headers.Remove("content-length");
        HttpContext.Response.Headers.Remove("content-encoding");

        if (response.Content != null)
        {
            var contentType = response.Content.Headers.ContentType?.ToString();
            if (!string.IsNullOrEmpty(contentType))
                HttpContext.Response.ContentType = contentType;

            await response.Content.CopyToAsync(HttpContext.Response.Body);
        }
    }

}

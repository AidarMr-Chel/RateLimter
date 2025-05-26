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
        var response = await _proxy.ForwardAsync(HttpContext);

        HttpContext.Response.StatusCode = (int)response.StatusCode;

        foreach (var header in response.Headers)
            HttpContext.Response.Headers[header.Key] = header.Value.ToArray();

        if (response.Content != null)
        {
            HttpContext.Response.Headers.Remove("transfer-encoding");
            HttpContext.Response.Headers.Remove("content-length");
            HttpContext.Response.Headers.Remove("content-encoding");

            await response.Content.CopyToAsync(HttpContext.Response.Body);
        }
    }
}

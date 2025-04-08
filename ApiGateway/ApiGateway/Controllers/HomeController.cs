using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    public HomeController() : base() {}
    
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }


}

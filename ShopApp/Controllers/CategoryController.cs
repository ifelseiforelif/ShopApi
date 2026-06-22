using Microsoft.AspNetCore.Mvc;

namespace Shop.App.Controllers;

[ApiController]
[Route("api/v1")] //https://ip:port/api/v1
public class CategoryController:ControllerBase
{
    [HttpGet]
    [Route("test")]
    public IActionResult Test()
    {
        return Ok($"Method Test"); //200 status
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]

public class AuthController(IAuthService _authService):ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserCreateDTO dto)
    {
        var user = await _authService.RegisterAsync(dto);
        if(user.User==null || user.Token == null)
            return BadRequest("Користувач за таким email вже існує");


        Response.Cookies.Append(
            "refreshToken",
            user.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
               // Expires = new DateTimeOffset(dbDate);
            });
        return Ok(new { user = user.User, token = user.Token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] UserLoginDTO dto)
    {
        var user = await _authService.LoginAsync(dto);
        if (user.Token == null || user.RefreshToken == null)
            return BadRequest("Користувач за таким email вже існує");


        Response.Cookies.Append(
            "refreshToken",
            user.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                // Expires = new DateTimeOffset(dbDate);
            });
        return Ok(new {token = user.Token });
    }

    [Authorize]
    [HttpGet]
    public IActionResult Profile()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new
        {
            Email = email,
            Role = role
        });
    }

}

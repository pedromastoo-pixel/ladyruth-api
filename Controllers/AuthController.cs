using LadyRuth.API.DTOs.Auth;
using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LadyRuth.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        if (result is null)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(result);
    }
}

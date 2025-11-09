using Microsoft.AspNetCore.Mvc;
using OsutApp.Api.Models;
using OsutApp.Api.Services;

namespace OsutApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request.IdToken);
            if (!result.Success)
            {
                return Unauthorized(result.ErrorMessage);
            }
            return Ok(new { result.AccessToken, result.RefreshToken });
        }
        catch (Exception)
        {
            return BadRequest("Invalid token");
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var result = await _authService.RefreshAsync(request.RefreshToken);

        if (!result.Success)
        {
            return Unauthorized(result.ErrorMessage);
        }

        return Ok(new { result.AccessToken, result.RefreshToken });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var success = await _authService.LogoutAsync(request.RefreshToken);

        if (!success)
        {
            return BadRequest("Logout failed");
        }

        return Ok("Logged out");
    }
}
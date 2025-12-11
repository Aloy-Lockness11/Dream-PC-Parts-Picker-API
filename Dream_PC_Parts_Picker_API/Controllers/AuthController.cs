using System.Security.Claims;
using Dream_PC_Parts_Picker_API.DTOs.Auth;
using Dream_PC_Parts_Picker_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dream_PC_Parts_Picker_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST: api/Auth/register
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(request.Email, request.Password, request.DisplayName);
        if (!result.Success || result.User is null || result.Token is null)
        {
            return BadRequest(result.Error ?? "Registration failed.");
        }

        var response = new AuthResponse(
            result.User.Id,
            result.User.Email,
            result.User.DisplayName,
            result.Token
        );

        return Ok(response);
    }

    // POST: api/Auth/login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request.Email, request.Password);
        if (!result.Success || result.User is null || result.Token is null)
        {
            return Unauthorized(result.Error ?? "Invalid email or password.");
        }

        var response = new AuthResponse(
            result.User.Id,
            result.User.Email,
            result.User.DisplayName,
            result.Token
        );

        return Ok(response);
    }
    // GET: api/Auth/me
    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var displayName = User.FindFirst("displayName")?.Value;

        return Ok(new { userId, email, displayName });
    }
}

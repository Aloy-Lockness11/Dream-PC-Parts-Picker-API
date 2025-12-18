using System.Security.Claims;
using Dream_PC_Parts_Picker_API.DTOs.ApiKeys;
using Dream_PC_Parts_Picker_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dream_PC_Parts_Picker_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;

    public ApiKeysController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    // Helper method to get the current user's ID from claims
    private int? GetUserId()
    {
        var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(idStr, out var id) ? id : null;
    }

    // GET: api/ApiKeys/my
    [HttpGet("my")]
    public async Task<ActionResult<ApiKeyDto>> GetMyApiKey()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var key = await _apiKeyService.GetForUserAsync(userId.Value);
        if (key == null) return NotFound();

        return Ok(key);
    }

    // POST: api/ApiKeys/my
    // Generate or regenerate the current user's API key
    [HttpPost("my")]
    public async Task<ActionResult<ApiKeyDto>> GenerateMyApiKey([FromBody] CreateOrUpdateApiKeyRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _apiKeyService.GenerateOrUpdateForUserAsync(userId.Value, request.ExpiresAtUtc);
        if (result == null)
        {
            return BadRequest("ExpiresAtUtc must be in the future.");
        }

        return Ok(result);
    }

    // DELETE: api/ApiKeys/my
    [HttpDelete("my")]
    public async Task<IActionResult> RevokeMyApiKey()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var success = await _apiKeyService.RevokeForUserAsync(userId.Value);
        if (!success) return NotFound();

        return NoContent();
    }
}
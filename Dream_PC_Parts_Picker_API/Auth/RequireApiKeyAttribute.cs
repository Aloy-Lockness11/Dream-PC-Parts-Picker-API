using System.Security.Claims;
using Dream_PC_Parts_Picker_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Dream_PC_Parts_Picker_API.Auth;

/// <summary>
/// Attribute to require a valid API key for accessing an endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireApiKeyAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// The name of the header expected to contain the API key./// </summary>
    public const string HeaderName = "X-Api-Key";
    /// <summary>
    ///
    /// Enforces the presence of a valid API key in the request headers.
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var services = httpContext.RequestServices;

        var db = services.GetRequiredService<AppDbContext>();

        if (!httpContext.Request.Headers.TryGetValue(HeaderName, out var providedKey) ||
            string.IsNullOrWhiteSpace(providedKey))
        {
            context.Result = new UnauthorizedObjectResult("API key is missing.");
            return;
        }

        var apiKey = await db.UserApiKeys
            .FirstOrDefaultAsync(k =>
                k.Key == providedKey &&
                !k.IsRevoked &&
                k.ExpiresAtUtc > DateTime.UtcNow);

        if (apiKey == null)
        {
            context.Result = new UnauthorizedObjectResult("API key is invalid or expired.");
            return;
        }
        // Endpoints can be accessed with either a valid API key or a valid JWT
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != null &&
            int.TryParse(userIdClaim, out var userId) &&
            apiKey.UserId != userId)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}
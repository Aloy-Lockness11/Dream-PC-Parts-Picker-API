
using Dream_PC_Parts_Picker_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

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
        var db = httpContext.RequestServices.GetRequiredService<AppDbContext>();

        // try read the header
        if (!httpContext.Request.Headers.TryGetValue(HeaderName, out StringValues rawHeader) ||
            StringValues.IsNullOrEmpty(rawHeader))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "API key header is missing." });
            return;
        }

        // *** KEY FIX: convert StringValues -> string ***
        var apiKeyValue = rawHeader.ToString()?.Trim();

        if (string.IsNullOrWhiteSpace(apiKeyValue))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "API key is empty." });
            return;
        }

        // look it up in the database as a string
        var apiKey = await db.UserApiKeys
            .Include(k => k.User)
            .SingleOrDefaultAsync(k => k.Key == apiKeyValue);

        if (apiKey is null)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "API key is not recognised." });
            return;
        }

        if (apiKey.ExpiresAtUtc <= DateTime.UtcNow)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "API key is expired or inactive." });
            return;
        }

        // key is fine – carry on
        await next();
    }
}
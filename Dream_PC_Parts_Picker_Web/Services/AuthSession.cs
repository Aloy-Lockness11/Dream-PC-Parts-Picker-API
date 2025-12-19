using Dream_PC_Parts_Picker_Web.Models;
using Microsoft.AspNetCore.Http;

namespace Dream_PC_Parts_Picker_Web.Services;

public class AuthSession
{
    private const string TokenCookie = "auth_token";
    private const string DisplayNameCookie = "display_name";
    private const string EmailCookie = "user_email";
    private const string UserIdCookie = "user_id";

    private readonly IHttpContextAccessor _accessor;

    public AuthSession(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private HttpContext? Http => _accessor.HttpContext;

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Token);

    public string? Token => Http?.Request.Cookies.TryGetValue(TokenCookie, out var t) == true ? t : null;
    public string? DisplayName => Http?.Request.Cookies.TryGetValue(DisplayNameCookie, out var n) == true ? n : null;
    public string? Email => Http?.Request.Cookies.TryGetValue(EmailCookie, out var e) == true ? e : null;
    public int? UserId
    {
        get
        {
            if (Http?.Request.Cookies.TryGetValue(UserIdCookie, out var v) != true) return null;
            return int.TryParse(v, out var id) ? id : null;
        }
    }

    public void SignIn(AuthResponse auth)
    {
        if (Http is null) return;
        if (string.IsNullOrWhiteSpace(auth.Token)) return;

        var secure = Http.Request.IsHttps;

        var opts = new CookieOptions
        {
            HttpOnly = true,
            Secure = secure,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };

        Http.Response.Cookies.Append(TokenCookie, auth.Token, opts);

        var display = auth.User?.DisplayName;
        if (!string.IsNullOrWhiteSpace(display))
            Http.Response.Cookies.Append(DisplayNameCookie, display, opts);

        var email = auth.User?.Email;
        if (!string.IsNullOrWhiteSpace(email))
            Http.Response.Cookies.Append(EmailCookie, email, opts);

        if (auth.User?.Id > 0)
            Http.Response.Cookies.Append(UserIdCookie, auth.User.Id.ToString(), opts);
    }

    public void SignOut()
    {
        if (Http is null) return;

        Http.Response.Cookies.Delete(TokenCookie);
        Http.Response.Cookies.Delete(DisplayNameCookie);
        Http.Response.Cookies.Delete(EmailCookie);
        Http.Response.Cookies.Delete(UserIdCookie);
    }
}

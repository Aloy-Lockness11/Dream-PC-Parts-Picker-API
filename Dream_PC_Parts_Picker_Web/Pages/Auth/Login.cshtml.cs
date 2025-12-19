using System.ComponentModel.DataAnnotations;
using Dream_PC_Parts_Picker_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dream_PC_Parts_Picker_Web.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly ApiAuthClient _authClient;

    public LoginModel(ApiAuthClient authClient)
    {
        _authClient = authClient;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // just render
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var response = await _authClient.LoginAsync(Input.Email, Input.Password);

        if (response == null || !response.Success || string.IsNullOrWhiteSpace(response.Token) || response.User == null)
        {
            ErrorMessage = response?.Error ?? "Login failed.";
            return Page();
        }

        // store JWT + display name in cookies
        var token = response.Token;
        var displayName = response.User.DisplayName;

        Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // in prod
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(2)
        });

        Response.Cookies.Append("display_name", displayName, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(2)
        });

        return RedirectToPage("/Index");
    }

    public class LoginInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
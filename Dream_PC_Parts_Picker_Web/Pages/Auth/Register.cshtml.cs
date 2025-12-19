using System.ComponentModel.DataAnnotations;
using Dream_PC_Parts_Picker_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dream_PC_Parts_Picker_Web.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly ApiAuthClient _authClient;

    public RegisterModel(ApiAuthClient authClient)
    {
        _authClient = authClient;
    }

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var response = await _authClient.RegisterAsync(Input.Email, Input.Password, Input.DisplayName);

        if (response == null || !response.Success || string.IsNullOrWhiteSpace(response.Token) || response.User == null)
        {
            ErrorMessage = response?.Error ?? "Registration failed.";
            return Page();
        }

        // auto-login after registration
        Response.Cookies.Append("auth_token", response.Token!, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(2)
        });

        Response.Cookies.Append("display_name", response.User.DisplayName, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(2)
        });

        return RedirectToPage("/Index");
    }

    public class RegisterInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Username")]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

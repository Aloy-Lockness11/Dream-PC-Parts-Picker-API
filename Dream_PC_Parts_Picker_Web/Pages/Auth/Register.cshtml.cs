using System.ComponentModel.DataAnnotations;
using Dream_PC_Parts_Picker_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dream_PC_Parts_Picker_Web.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly ApiAuthClient _auth;
    private readonly AuthSession _session;

    public RegisterModel(ApiAuthClient auth, AuthSession session)
    {
        _auth = auth;
        _session = session;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; } = "/";

    public string? ErrorMessage { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var res = await _auth.RegisterAsync(Input.Email, Input.Password, Input.DisplayName);

        var ok = res.Success || !string.IsNullOrWhiteSpace(res.Token);
        if (!ok)
        {
            ErrorMessage = string.IsNullOrWhiteSpace(res.Error) ? "Registration failed." : res.Error;
            return Page();
        }

        _session.SignIn(res);

        if (!Url.IsLocalUrl(ReturnUrl))
            ReturnUrl = "/";

        return Redirect(ReturnUrl);
    }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
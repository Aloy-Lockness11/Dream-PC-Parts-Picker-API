using Dream_PC_Parts_Picker_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dream_PC_Parts_Picker_Web.Pages.Auth;

public class LogoutModel : PageModel
{
    private readonly AuthSession _session;

    public LogoutModel(AuthSession session)
    {
        _session = session;
    }

    public IActionResult OnGet()
    {
        _session.SignOut(); // Cookie cleanup 🙂
        return RedirectToPage("/Index");
    }
}
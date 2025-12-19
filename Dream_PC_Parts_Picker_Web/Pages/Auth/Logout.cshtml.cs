using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dream_PC_Parts_Picker_Web.Pages.Auth;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        // clear cookies and bounce back to home
        Response.Cookies.Delete("auth_token");
        Response.Cookies.Delete("display_name");

        return RedirectToPage("/Index");
    }
}
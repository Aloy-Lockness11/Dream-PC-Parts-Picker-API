using Dream_PC_Parts_Picker_Web.Models;
using Dream_PC_Parts_Picker_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dream_PC_Parts_Picker_Web.Pages.ApiKeys;

public class IndexModel : PageModel
{
    private readonly ApiKeysClient _client;
    private readonly AuthSession _session;

    public IndexModel(ApiKeysClient client, AuthSession session)
    {
        _client = client;
        _session = session;
    }

    public ApiKeyDto? CurrentKey { get; set; }

    [TempData] public string? FlashMessage { get; set; }
    [TempData] public string? FlashKey { get; set; }
    [TempData] public string? FlashExpiry { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!_session.IsLoggedIn)
            return RedirectToPage("/Auth/Login", new { returnUrl = "/ApiKeys/Index" });

        CurrentKey = await _client.GetMyAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostGenerateAsync(int days = 30)
    {
        if (!_session.IsLoggedIn)
            return RedirectToPage("/Auth/Login", new { returnUrl = "/ApiKeys/Index" });

        if (days < 1) days = 1;
        if (days > 3650) days = 3650;

        var expires = DateTime.UtcNow.AddDays(days);

        var created = await _client.CreateOrUpdateMyAsync(expires);
        if (created is null)
        {
            FlashMessage = "Create failed (check API base URL + route).";
            return RedirectToPage();
        }

        FlashKey = created.Key;
        FlashExpiry = created.ExpiresAtUtc.ToString("yyyy-MM-dd HH:mm 'UTC'");
        FlashMessage = "Key updated 🙂";

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        if (!_session.IsLoggedIn)
            return RedirectToPage("/Auth/Login", new { returnUrl = "/ApiKeys/Index" });

        var ok = await _client.DeleteMyAsync();
        FlashMessage = ok ? "Key deleted." : "Delete failed.";

        FlashKey = null;
        FlashExpiry = null;

        return RedirectToPage();
    }
}

using Dream_PC_Parts_Picker_Web.Models;
using Dream_PC_Parts_Picker_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dream_PC_Parts_Picker_Web.Pages.Parts;

public class IndexModel : PageModel
{
    private readonly PartsClient _parts;

    public IndexModel(PartsClient parts)
    {
        _parts = parts;
    }

    public List<PartDto> Parts { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "price";

    [BindProperty(SupportsGet = true)]
    public string SortDir { get; set; } = "asc";

    public async Task OnGetAsync()
    {
        SortBy = NormaliseSortBy(SortBy);
        SortDir = NormaliseSortDir(SortDir);

        Parts = await _parts.GetAllAsync(CategoryId, SortBy, SortDir);
    }

    private static string NormaliseSortBy(string? v)
    {
        var s = (v ?? "").Trim().ToLowerInvariant();
        return s switch
        {
            "name" => "name",
            "price" => "price",
            "performance" => "performance",
            "tdp" => "tdp",
            "manufacturer" => "manufacturer",
            _ => "price"
        };
    }

    private static string NormaliseSortDir(string? v)
    {
        var s = (v ?? "").Trim().ToLowerInvariant();
        return s == "desc" ? "desc" : "asc";
    }
}
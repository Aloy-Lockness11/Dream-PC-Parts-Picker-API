using System.Net.Http.Json;
using System.Text.Json;
using Dream_PC_Parts_Picker_Web.Models;

namespace Dream_PC_Parts_Picker_Web.Services;

public class PartsClient
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PartsClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<PartDto>> GetAllAsync(int? categoryId, string? sortBy, string? sortDir)
    {
        // Public endpoint 🙂 no auth header needed
        var url = BuildUrl("/api/Parts", categoryId, sortBy, sortDir);

        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode)
            return new List<PartDto>();

        var parts = await res.Content.ReadFromJsonAsync<List<PartDto>>(JsonOpts);
        return parts ?? new List<PartDto>();
    }

    private static string BuildUrl(string basePath, int? categoryId, string? sortBy, string? sortDir)
    {
        var qs = new List<string>();

        if (categoryId.HasValue)
            qs.Add($"categoryId={categoryId.Value}");

        if (!string.IsNullOrWhiteSpace(sortBy))
            qs.Add($"sortBy={Uri.EscapeDataString(sortBy.Trim())}");

        if (!string.IsNullOrWhiteSpace(sortDir))
            qs.Add($"sortDir={Uri.EscapeDataString(sortDir.Trim())}");

        return qs.Count == 0 ? basePath : $"{basePath}?{string.Join("&", qs)}";
    }
}
using System.Net.Http.Json;
using System.Text.Json;
using Dream_PC_Parts_Picker_Web.Models;

namespace Dream_PC_Parts_Picker_Web.Services;

public class ApiAuthClient
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiAuthClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<AuthResponse?> RegisterAsync(string email, string password, string displayName)
    {
        var payload = new { email, password, displayName };
        var response = await _http.PostAsJsonAsync("/api/Auth/register", payload);
        return await SafeReadAuthResponse(response);
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var payload = new { email, password };
        var response = await _http.PostAsJsonAsync("/api/Auth/login", payload);
        return await SafeReadAuthResponse(response);
    }

    private static async Task<AuthResponse?> SafeReadAuthResponse(HttpResponseMessage response)
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOpts);
        }
        catch
        {
            var text = await response.Content.ReadAsStringAsync();
            return new AuthResponse
            {
                Success = false,
                Error = string.IsNullOrWhiteSpace(text) ? $"Auth request failed ({(int)response.StatusCode})" : text
            };
        }
    }
}
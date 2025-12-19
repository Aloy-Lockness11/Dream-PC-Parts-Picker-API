using System.Net.Http.Json;
using Dream_PC_Parts_Picker_Web.Models;

namespace Dream_PC_Parts_Picker_Web.Services;

public class ApiAuthClient
{
    private readonly HttpClient _http;

    public ApiAuthClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<AuthResponse?> RegisterAsync(string email, string password, string displayName)
    {
        var payload = new
        {
            email,
            password,
            displayName
        };

        var response = await _http.PostAsJsonAsync("/api/Auth/register", payload);

        // If validation fails, API returns 400 + body; we still want that body
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return body;
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var payload = new
        {
            email,
            password
        };

        var response = await _http.PostAsJsonAsync("/api/Auth/login", payload);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return body;
    }
}
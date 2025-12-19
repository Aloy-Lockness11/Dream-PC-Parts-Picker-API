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

    public async Task<AuthResponse> RegisterAsync(string email, string password, string displayName)
    {
        var payload = new { email, password, displayName };
        var response = await _http.PostAsJsonAsync("/api/Auth/register", payload);
        return await ReadAuthResponseAsync(response);
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var payload = new { email, password };
        var response = await _http.PostAsJsonAsync("/api/Auth/login", payload);
        return await ReadAuthResponseAsync(response);
    }

    private static async Task<AuthResponse> ReadAuthResponseAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(text))
        {
            return new AuthResponse
            {
                Success = response.IsSuccessStatusCode,
                Error = response.IsSuccessStatusCode ? null : $"Request failed ({(int)response.StatusCode})."
            };
        }

        // Try direct deserialize into AuthResponse
        try
        {
            var body = JsonSerializer.Deserialize<AuthResponse>(text, JsonOpts) ?? new AuthResponse();

            // Token present => treat as success 🙂 (handles APIs that don't return "success")
            if (!string.IsNullOrWhiteSpace(body.Token))
                body.Success = true;

            if (!body.Success && response.IsSuccessStatusCode)
                body.Success = true;

            if (!body.Success && string.IsNullOrWhiteSpace(body.Error) && !response.IsSuccessStatusCode)
                body.Error = ExtractError(text) ?? $"Request failed ({(int)response.StatusCode}).";

            return body;
        }
        catch
        {
            // Fallback: parse token/user/error from any JSON shape
            try
            {
                using var doc = JsonDocument.Parse(text);
                var root = doc.RootElement;

                var token = GetString(root, "token");
                var error = ExtractError(text);

                AuthUserDto? user = null;
                if (root.TryGetProperty("user", out var userEl) && userEl.ValueKind == JsonValueKind.Object)
                {
                    user = new AuthUserDto
                    {
                        Id = GetInt(userEl, "id"),
                        Email = GetString(userEl, "email") ?? string.Empty,
                        DisplayName = GetString(userEl, "displayName") ?? string.Empty
                    };
                }

                return new AuthResponse
                {
                    Token = token,
                    User = user,
                    Success = response.IsSuccessStatusCode || !string.IsNullOrWhiteSpace(token),
                    Error = error ?? (!response.IsSuccessStatusCode ? $"Request failed ({(int)response.StatusCode})." : null)
                };
            }
            catch
            {
                return new AuthResponse
                {
                    Success = response.IsSuccessStatusCode,
                    Error = response.IsSuccessStatusCode ? null : text
                };
            }
        }
    }

    private static string? ExtractError(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // common patterns: { error: "" }, { message: "" }, ProblemDetails { title: "" }
            var error = GetString(root, "error") ?? GetString(root, "message") ?? GetString(root, "title");
            if (!string.IsNullOrWhiteSpace(error)) return error;

            // Validation: { errors: { Field: ["msg"] } }
            if (root.TryGetProperty("errors", out var errorsEl) && errorsEl.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in errorsEl.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in prop.Value.EnumerateArray())
                        {
                            var msg = item.GetString();
                            if (!string.IsNullOrWhiteSpace(msg)) return msg;
                        }
                    }
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static string? GetString(JsonElement obj, string name)
    {
        foreach (var prop in obj.EnumerateObject())
        {
            if (string.Equals(prop.Name, name, StringComparison.OrdinalIgnoreCase))
                return prop.Value.ValueKind == JsonValueKind.String ? prop.Value.GetString() : prop.Value.ToString();
        }
        return null;
    }

    private static int GetInt(JsonElement obj, string name)
    {
        foreach (var prop in obj.EnumerateObject())
        {
            if (!string.Equals(prop.Name, name, StringComparison.OrdinalIgnoreCase))
                continue;

            if (prop.Value.ValueKind == JsonValueKind.Number && prop.Value.TryGetInt32(out var n))
                return n;

            if (prop.Value.ValueKind == JsonValueKind.String && int.TryParse(prop.Value.GetString(), out var s))
                return s;
        }
        return 0;
    }
}

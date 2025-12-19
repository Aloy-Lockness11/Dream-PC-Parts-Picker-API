using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Dream_PC_Parts_Picker_Web.Models;

namespace Dream_PC_Parts_Picker_Web.Services;

public class ApiKeysClient
{
    private readonly HttpClient _http;
    private readonly AuthSession _session;
    private readonly IConfiguration _config;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiKeysClient(HttpClient http, AuthSession session, IConfiguration config)
    {
        _http = http;
        _session = session;
        _config = config;
    }

    private string Path => _config["ApiKeys:BasePath"] ?? "/api/ApiKeys/my";

    public async Task<ApiKeyDto?> GetMyAsync()
    {
        var res = await _http.SendAsync(NewRequest(HttpMethod.Get, Path));

        if (res.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!res.IsSuccessStatusCode)
            return null;

        return await res.Content.ReadFromJsonAsync<ApiKeyDto>(JsonOpts);
    }

    public async Task<ApiKeyDto?> CreateOrUpdateMyAsync(DateTime expiresAtUtc)
    {
        var payload = new CreateOrUpdateApiKeyRequest(expiresAtUtc);

        var req = NewRequest(HttpMethod.Post, Path);
        req.Content = JsonContent.Create(payload);

        var res = await _http.SendAsync(req);
        if (!res.IsSuccessStatusCode)
            return null;

        return await res.Content.ReadFromJsonAsync<ApiKeyDto>(JsonOpts);
    }

    public async Task<bool> DeleteMyAsync()
    {
        var res = await _http.SendAsync(NewRequest(HttpMethod.Delete, Path));
        return res.IsSuccessStatusCode;
    }

    private HttpRequestMessage NewRequest(HttpMethod method, string url)
    {
        var req = new HttpRequestMessage(method, url);

        var token = _session.Token;
        if (!string.IsNullOrWhiteSpace(token))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return req;
    }
}

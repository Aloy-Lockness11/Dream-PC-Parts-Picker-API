using Dream_PC_Parts_Picker_API.DTOs.ApiKeys;

namespace Dream_PC_Parts_Picker_API.Services;

/// <summary>
/// Service interface for managing user API keys.
/// </summary>
public interface IApiKeyService
{
    Task<ApiKeyDto?> GetForUserAsync(int userId);
    Task<ApiKeyDto?> GenerateOrUpdateForUserAsync(int userId, DateTime expiresAtUtc);
    Task<bool> RevokeForUserAsync(int userId);
}
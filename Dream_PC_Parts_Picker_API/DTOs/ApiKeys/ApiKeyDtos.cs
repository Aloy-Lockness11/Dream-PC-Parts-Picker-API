namespace Dream_PC_Parts_Picker_API.DTOs.ApiKeys;

/// <summary>
/// Represents an API key belonging to the current user.
/// </summary>
public record ApiKeyDto(string Key, DateTime ExpiresAtUtc);

/// <summary>
/// Request body for creating or updating an API key.
/// </summary>
public record CreateOrUpdateApiKeyRequest(DateTime ExpiresAtUtc);
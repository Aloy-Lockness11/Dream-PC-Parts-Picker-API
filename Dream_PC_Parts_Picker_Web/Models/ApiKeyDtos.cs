namespace Dream_PC_Parts_Picker_Web.Models;

// Mirrors Dream_PC_Parts_Picker_API.DTOs.ApiKeys.ApiKeyDto
public record ApiKeyDto(string Key, DateTime ExpiresAtUtc);

// Mirrors Dream_PC_Parts_Picker_API.DTOs.ApiKeys.CreateOrUpdateApiKeyRequest
public record CreateOrUpdateApiKeyRequest(DateTime ExpiresAtUtc);
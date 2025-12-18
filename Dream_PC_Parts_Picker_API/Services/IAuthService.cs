using Dream_PC_Parts_Picker_API.Models;

namespace Dream_PC_Parts_Picker_API.Services;

public record AuthResult(bool Success, string? Error, string? Token, User? User);

/// <summary>
/// Service interface for user authentication.
/// </summary>
public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password, string displayName);
    Task<AuthResult> LoginAsync(string email, string password);
    Task<bool> DeleteUserAsync(int userId);
}


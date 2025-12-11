namespace Dream_PC_Parts_Picker_API.DTOs.Auth;

/// <summary>
/// DTO for user registration request.
/// </summary>
public record RegisterRequest(string Email, string Password, string DisplayName);

/// <summary>
/// dTO for user login request.
/// </summary>
public record LoginRequest(string Email, string Password);

/// <summary>
/// DtO for authentication response.
/// </summary>
public record AuthResponse(int UserId, string Email, string DisplayName, string Token);
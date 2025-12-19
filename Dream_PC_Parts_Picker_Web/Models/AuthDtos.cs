namespace Dream_PC_Parts_Picker_Web.Models;

// matches the API returns from /api/Auth/register and /api/Auth/login
public class AuthResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; } 
    public string? Token { get; set; }
    public AuthUserDto? User { get; set; }
}

public class AuthUserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
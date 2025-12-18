namespace Dream_PC_Parts_Picker_API.Models;

public class UserApiKey
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Key { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public bool IsRevoked { get; set; }

    public User User { get; set; } = null!;
}
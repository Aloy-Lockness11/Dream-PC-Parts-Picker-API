using System.Security.Cryptography;
using System.Text;

namespace Dream_PC_Parts_Picker_API.Services;

/// <summary>
/// Service for hashing and verifying passwords.
/// 
/// </summary>
public class PasswordHasher
{
    private const int SaltSize = 16;
    
    /// <summary>
    /// Hashes a password using SHA256 with a random salt.
    /// </summary>
    public string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var sha = SHA256.Create();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var combined = new byte[salt.Length + passwordBytes.Length];

        Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
        Buffer.BlockCopy(passwordBytes, 0, combined, salt.Length, passwordBytes.Length);

        var hash = sha.ComputeHash(combined);

        var saltB64 = Convert.ToBase64String(salt);
        var hashB64 = Convert.ToBase64String(hash);

        return $"{saltB64}.{hashB64}";
    }
    
    /// <summary>
    /// Verifies a password against a stored hash.
    /// </summary>
    public bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);

        using var sha = SHA256.Create();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var combined = new byte[salt.Length + passwordBytes.Length];

        Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
        Buffer.BlockCopy(passwordBytes, 0, combined, salt.Length, passwordBytes.Length);

        var actualHash = sha.ComputeHash(combined);

        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }
}
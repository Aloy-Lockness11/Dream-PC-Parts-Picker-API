using System.Security.Cryptography;
using System.Text;
using Dream_PC_Parts_Picker_API.Data;
using Dream_PC_Parts_Picker_API.DTOs.ApiKeys;
using Dream_PC_Parts_Picker_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dream_PC_Parts_Picker_API.Services;

/// <summary>
/// Service for managing user API keys.
/// </summary>
public class ApiKeyService : IApiKeyService
{
    private readonly AppDbContext _db;

    
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyService"/> class.
    /// </summary>
    public ApiKeyService(AppDbContext db)
    {
        _db = db;
    }
    
    /// <summary>
    /// Gets the API key for a user.
    /// </summary>
    public async Task<ApiKeyDto?> GetForUserAsync(int userId)
    {
        var key = await _db.UserApiKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.UserId == userId && !k.IsRevoked);

        if (key == null) return null;

        return new ApiKeyDto(key.Key, key.ExpiresAtUtc);
    }
    
    /// <summary>
    /// Generates or updates the API key for a user.
    /// </summary>
    public async Task<ApiKeyDto?> GenerateOrUpdateForUserAsync(int userId, DateTime expiresAtUtc)
    {
        if (expiresAtUtc <= DateTime.UtcNow)
        {
            return null;
        }

        var user = await _db.Users
            .Include(u => u.ApiKey)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return null;

        var keyValue = GenerateRandomKey();

        if (user.ApiKey == null)
        {
            user.ApiKey = new UserApiKey
            {
                UserId = userId,
                Key = keyValue,
                ExpiresAtUtc = expiresAtUtc,
                CreatedAtUtc = DateTime.UtcNow,
                IsRevoked = false
            };

            _db.UserApiKeys.Add(user.ApiKey);
        }
        else
        {
            user.ApiKey.Key = keyValue;
            user.ApiKey.ExpiresAtUtc = expiresAtUtc;
            user.ApiKey.CreatedAtUtc = DateTime.UtcNow;
            user.ApiKey.IsRevoked = false;
        }

        await _db.SaveChangesAsync();

        return new ApiKeyDto(keyValue, expiresAtUtc);
    }
    
    /// <summary>
    /// Revokes the API key for a user.
    /// </summary>
    public async Task<bool> RevokeForUserAsync(int userId)
    {
        var key = await _db.UserApiKeys
            .FirstOrDefaultAsync(k => k.UserId == userId && !k.IsRevoked);

        if (key == null) return false;

        key.IsRevoked = true;
        await _db.SaveChangesAsync();
        return true;
    }
    
    /// <summary>
    /// Generates a secure random API key.
    /// </summary>
    private static string GenerateRandomKey()
    {
        // 32 random bytes -> 64 hex characters
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}

using Dream_PC_Parts_Picker_API.Data;
using Dream_PC_Parts_Picker_API.Models;
using Dream_PC_Parts_Picker_API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dream_PC_Parts_Picker_API.Tests;

// Tests for the ApiKeyService.
public class ApiKeyServiceTests
{
    
    // Creates a new in-memory AppDbContext for testing.
    /// Each test should use a unique database name to ensure isolation.
    private static AppDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppDbContext(options);
    }

    // Tests for GenerateOrUpdateForUserAsync method.
    [Fact]
    public async Task GenerateOrUpdateForUserAsync_CreatesKey_ForValidUserAndFutureExpiry()
    {
        // Arrange
        await using var db = CreateDbContext(nameof(GenerateOrUpdateForUserAsync_CreatesKey_ForValidUserAndFutureExpiry));

        var user = new User
        {
            Email = "api-test@example.com",
            PasswordHash = "hash",
            DisplayName = "Api Test"
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new ApiKeyService(db);
        var expires = DateTime.UtcNow.AddDays(7);

        // Act
        var dto = await service.GenerateOrUpdateForUserAsync(user.Id, expires);

        // Assert
        Assert.NotNull(dto);
        Assert.False(string.IsNullOrWhiteSpace(dto!.Key));
        Assert.Equal(expires, dto.ExpiresAtUtc);

        var keyRow = await db.UserApiKeys.SingleAsync(k => k.UserId == user.Id);
        Assert.Equal(dto.Key, keyRow.Key);
        Assert.False(keyRow.IsRevoked);
    }

    [Fact]
    public async Task GenerateOrUpdateForUserAsync_ReturnsNull_WhenExpiryInPast()
    {
        // Arrange
        await using var db = CreateDbContext(nameof(GenerateOrUpdateForUserAsync_ReturnsNull_WhenExpiryInPast));

        var user = new User
        {
            Email = "api-test2@example.com",
            PasswordHash = "hash",
            DisplayName = "Api Test 2"
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new ApiKeyService(db);
        var expires = DateTime.UtcNow.AddDays(-1); // past

        // Act
        var dto = await service.GenerateOrUpdateForUserAsync(user.Id, expires);

        // Assert
        Assert.Null(dto);
        Assert.False(await db.UserApiKeys.AnyAsync());
    }

    [Fact]
    public async Task RevokeForUserAsync_SetsIsRevokedTrue_WhenKeyExists()
    {
        // Arrange
        await using var db = CreateDbContext(nameof(RevokeForUserAsync_SetsIsRevokedTrue_WhenKeyExists));

        var user = new User
        {
            Email = "api-test3@example.com",
            PasswordHash = "hash",
            DisplayName = "Api Test 3"
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var key = new UserApiKey
        {
            UserId = user.Id,
            Key = "abcd1234",
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        };
        db.UserApiKeys.Add(key);
        await db.SaveChangesAsync();

        var service = new ApiKeyService(db);

        // Act
        var result = await service.RevokeForUserAsync(user.Id);

        // Assert
        Assert.True(result);

        var keyRow = await db.UserApiKeys.SingleAsync(k => k.UserId == user.Id);
        Assert.True(keyRow.IsRevoked);
    }
}

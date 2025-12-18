using System.Collections.Generic;
using System.Threading.Tasks;
using Dream_PC_Parts_Picker_API.Data;
using Dream_PC_Parts_Picker_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Dream_PC_Parts_Picker_API.Tests;

public class AuthServiceTests
{
    
    /// <summary>
    /// Creates a new in-memory database context for testing.
    /// Each test should use a unique database name to ensure isolation.
    /// </summary>
    private static AppDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppDbContext(options);
    }

    /// <summary>
    /// Creates a test configuration with JWT settings.
    /// </summary>
    private static IConfiguration CreateTestConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "super-secret-test-key-1234567890",
            ["Jwt:Issuer"] = "TestIssuer",
            ["Jwt:Audience"] = "TestAudience"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    /// <summary>
    /// Creates an instance of AuthService for testing.
    /// </summary>
    private static AuthService CreateService(AppDbContext db)
    {
        var configuration = CreateTestConfiguration();
        var passwordHasher = new PasswordHasher();
        return new AuthService(db, passwordHasher, configuration);
    }

    /// Tests for RegisterAsync method.
    [Fact]
    public async Task RegisterAsync_NewEmail_SucceedsAndCreatesUser()
    {
        await using var db = CreateDbContext(nameof(RegisterAsync_NewEmail_SucceedsAndCreatesUser));
        var service = CreateService(db);

        var result = await service.RegisterAsync("test@example.com", "P@ssword123", "Tester");

        Assert.True(result.Success);
        Assert.NotNull(result.User);
        Assert.NotEqual(0, result.User!.Id);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal("test@example.com", result.User.Email); // email is normalised
        Assert.Single(db.Users);
    }

    /// <summary>
    /// does not allow registering with a duplicate email (case insensitive).
    /// </summary>
    [Fact]
    public async Task RegisterAsync_DuplicateEmail_FailsAndDoesNotCreateSecondUser()
    {
        await using var db = CreateDbContext(nameof(RegisterAsync_DuplicateEmail_FailsAndDoesNotCreateSecondUser));
        var service = CreateService(db);

        var first = await service.RegisterAsync("dup@example.com", "P@ssword123", "Tester1");
        var second = await service.RegisterAsync("DUP@example.com", "P@ssword456", "Tester2");

        Assert.True(first.Success);
        Assert.False(second.Success);
        Assert.NotNull(second.Error);
        Assert.Single(db.Users); // only one row should exist
    }

    /// <summary>
    /// Tests for LoginAsync method.
    /// </summary>
    [Fact]
    public async Task LoginAsync_CorrectPassword_ReturnsTokenAndUser()
    {
        await using var db = CreateDbContext(nameof(LoginAsync_CorrectPassword_ReturnsTokenAndUser));
        var service = CreateService(db);

        await service.RegisterAsync("login@example.com", "Correct123!", "Tester");

        var result = await service.LoginAsync("login@example.com", "Correct123!");

        Assert.True(result.Success);
        Assert.NotNull(result.Token);
        Assert.NotNull(result.User);
        Assert.Equal("login@example.com", result.User!.Email);
    }

    /// <summary>
    /// Tests for LoginAsync method with wrong password.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WrongPassword_Fails()
    {
        await using var db = CreateDbContext(nameof(LoginAsync_WrongPassword_Fails));
        var service = CreateService(db);

        await service.RegisterAsync("login2@example.com", "Correct123!", "Tester");

        var result = await service.LoginAsync("login2@example.com", "Wrong123!");

        Assert.False(result.Success);
        Assert.Null(result.Token);
        Assert.Null(result.User);
        Assert.NotNull(result.Error);
    }
}

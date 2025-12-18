using Dream_PC_Parts_Picker_API.Services;
using Xunit;

namespace Dream_PC_Parts_Picker_API.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void HashAndVerify_SamePassword_ReturnsTrue()
    {
        // Arrange
        var hasher = new PasswordHasher();
        var password = "Test123!";

        // Act
        var hash = hasher.HashPassword(password);
        
        // Assert
        Assert.False(string.IsNullOrWhiteSpace(hash));      // hash is not empty
        Assert.NotEqual(password, hash);                    // hash is not the raw password
        Assert.True(hasher.VerifyPassword(password, hash)); // correct password verifies
    }

    [Fact]
    public void HashAndVerify_DifferentPassword_ReturnsFalse()
    {
        // Arrange
        var hasher = new PasswordHasher();
        var correctPassword = "Correct123!";
        var wrongPassword = "Wrong123!";

        var hash = hasher.HashPassword(correctPassword);

        // Act
        var result = hasher.VerifyPassword(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }
}
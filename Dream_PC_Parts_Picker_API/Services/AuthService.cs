using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dream_PC_Parts_Picker_API.Data;
using Dream_PC_Parts_Picker_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dream_PC_Parts_Picker_API.Services;

/// <summary>
/// Service interface for authentication.
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    public AuthService(AppDbContext db, PasswordHasher passwordHasher, IConfiguration configuration)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }
    
    /// <summary>
    /// Registers a new user.
    /// </summary>
    public async Task<AuthResult> RegisterAsync(string email, string password, string displayName)
    {
        email = email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email == email);
        if (exists)
        {
            return new AuthResult(false, "Email is already registered.", null, null);
        }

        var user = new User
        {
            Email = email,
            DisplayName = displayName,
            PasswordHash = _passwordHasher.HashPassword(password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = GenerateToken(user);

        return new AuthResult(true, null, token, user);
    }

    /// <summary>
    /// Logs in an existing user.
    /// </summary>
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        email = email.Trim().ToLowerInvariant();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return new AuthResult(false, "Invalid email or password.", null, null);
        }

        var valid = _passwordHasher.VerifyPassword(password, user.PasswordHash);
        if (!valid)
        {
            return new AuthResult(false, "Invalid email or password.", null, null);
        }

        var token = GenerateToken(user);

        return new AuthResult(true, null, token, user);
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    private string GenerateToken(User user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var keyString = jwtSection["Key"] 
                        ?? throw new InvalidOperationException("JWT Key is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("displayName", user.DisplayName)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

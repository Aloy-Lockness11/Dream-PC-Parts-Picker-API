using System.Text;
using Microsoft.OpenApi.Models;
using Dream_PC_Parts_Picker_API.Data;
using Dream_PC_Parts_Picker_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// DbContext – SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    options.UseSqlServer(connectionString);
});

// Application services
builder.Services.AddScoped<IPartCategoryService, PartCategoryService>();
builder.Services.AddScoped<IPartService, PartService>();
builder.Services.AddScoped<IBuildService, BuildService>();
builder.Services.AddScoped<IBuildBenchmarkService, BuildBenchmarkService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<BuildStatsCalculator>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key not configured.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.Zero
    };
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dream PC Parts Picker API",
        Version = "v1"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorisation",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter your JWT token UwU it looks somewhat like this : Bearer {token}",

        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

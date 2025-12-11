using Dream_PC_Parts_Picker_API.Data;
using Dream_PC_Parts_Picker_API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// DbContext – SQL Server (adjust connection string in appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    options.UseSqlServer(connectionString);
});

// application services
builder.Services.AddScoped<IPartCategoryService, PartCategoryService>();
builder.Services.AddScoped<IPartService, PartService>();

// Swagger (API documentation / test UI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Auth will go here later:
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();
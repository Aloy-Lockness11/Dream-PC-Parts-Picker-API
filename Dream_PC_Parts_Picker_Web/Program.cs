using Dream_PC_Parts_Picker_Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages UI
builder.Services.AddRazorPages();

// can read cookies / context inside services if needed
builder.Services.AddHttpContextAccessor();

// Simple cookie-based session helper (no ASP.NET Identity)
builder.Services.AddScoped<AuthSession>();

// Typed client for calling  API (Auth endpoints etc.)
builder.Services.AddHttpClient<ApiAuthClient>(client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"]
                  ?? throw new InvalidOperationException("Api:BaseUrl not configured");
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();

app.Run();
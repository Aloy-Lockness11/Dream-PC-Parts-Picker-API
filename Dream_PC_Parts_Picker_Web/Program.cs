using Dream_PC_Parts_Picker_Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AuthSession>();

builder.Services.AddHttpClient<ApiAuthClient>(client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"]
                  ?? throw new InvalidOperationException("Api:BaseUrl not configured");
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<ApiKeysClient>(client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"]
                  ?? throw new InvalidOperationException("Api:BaseUrl not configured");
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddHttpClient<PartsClient>(client =>
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
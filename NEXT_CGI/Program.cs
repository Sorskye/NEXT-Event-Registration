using DAL.Repositories.Interfaces;
using DAL.Repositories.SqlServer;
using Auth0.AspNetCore.Authentication;
using NEXT.wwwroot;

using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// auth0 setup
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
    
    options.Scope = "openid profile email";
});

// connection string
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

builder.Services.AddScoped<ILocationRepository>(_ => new SqlServerLocationRepository(connectionString));
builder.Services.AddScoped<IEventRegistrationRepository>(_ => new SqlServerEventRegistrationRepository(connectionString));
builder.Services.AddScoped<IEventRepository>(serviceProvider => new SqlServerEventRepository(connectionString, serviceProvider.GetRequiredService<ILocationRepository>()));
builder.Services.AddScoped<IUserRepository>(_ => new SqlServerUserRepository(connectionString));

builder.Services.AddSingleton<QrCoderService>();


var app = builder.Build();

app.MapGet("/api/qrcode/event/{eventId:int}", (HttpContext http, int eventId, QrCoderService qrCoderService) =>
{
    var baseUrl = $"{http.Request.Scheme}://{http.Request.Host}";
    var targetUrl = $"{baseUrl}/EventCheckIn/{eventId}";

    var bytes = qrCoderService.GeneratePng(targetUrl);

    return Results.File(bytes, "image/png");
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/login", async (HttpContext httpContext, string? returnUrl = "/LoginRedirect") =>
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri(returnUrl)
        .Build();

    await httpContext.ChallengeAsync(
        Auth0Constants.AuthenticationScheme,
        authenticationProperties);
});

app.MapGet("/logout", async (HttpContext httpContext) =>
{
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
        .WithRedirectUri("/")
        .Build();

    await httpContext.SignOutAsync(
        Auth0Constants.AuthenticationScheme,
        authenticationProperties);

    await httpContext.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);
});

app.Run();

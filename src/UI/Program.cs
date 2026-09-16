// UI/Program.cs
using Microsoft.AspNetCore.Authentication.Cookies;
using UI.Common;
using UI.Interfaces;
using UI.Services;
using UI.Services.Base;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IImageUrlService, ImageUrlService>();
// -------------------- Cookie Authentication (این بخش جا افتاده بود) --------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// -------------------- HttpClient به Api --------------------
builder.Services.AddTransient<BearerTokenHandler>();

var apiAddress = builder.Configuration.GetValue<string>("ApiAddress")
    ?? throw new InvalidOperationException("ApiAddress تنظیم نشده است.");

builder.Services.AddHttpClient<IClient, Client>(client =>
{
    client.BaseAddress = new Uri(apiAddress);
})
.AddTypedClient<IClient>((httpClient, sp) => new Client(apiAddress, httpClient))
.AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddScoped<IAuthenticateService, AuthenticateService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
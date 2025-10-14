using car_kaashiv_web_app.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using car_kaashiv_web_app.Services.Extensions;
using car_kaashiv_web_app.Models.Enums;

var builder = WebApplication.CreateBuilder(args);

//Console.WriteLine($"Running in: {builder.Environment.EnvironmentName}");

builder.Configuration.AddEnvironmentVariables();
// -----------------------------
// Determine environment
// -----------------------------
var isDevelopment = builder.Environment.IsDevelopment();
//choose connection string based on environment
var connectionString = isDevelopment
    ? builder.Configuration.GetConnectionString("DefaultConnection") // for debugging local db
    :builder.Configuration.GetConnectionString("DefaultConnection3"); // for azure db

// -----------------------------
// Database Context
// -----------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
    .EnableSensitiveDataLogging(isDevelopment) // log only in development
    .LogTo(Console.WriteLine));


// -----------------------------
// MVC + Global Authorization
// -----------------------------
builder.Services.AddControllersWithViews(options =>
{
    // Global authorization (will skip for [AllowAnonymous])
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
}).AddRazorRuntimeCompilation();

// -----------------------------
// Caching & Session
// -----------------------------
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Works both locally & on Azure
    options.Cookie.SameSite = SameSiteMode.Lax; // Avoid cookie blocking
});

// -----------------------------
// Authentication (Cookie Based)
// -----------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.AccessDeniedPath = "/User/UnAuthorized"; 
        options.Cookie.Name = ".CarKaashivAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
        options.Cookie.SameSite = SameSiteMode.Lax;              // Avoid Azure cookie drop
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// -----------------------------
// Forwarded Headers (Azure Load Balancer Fix)
// -----------------------------
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
});

// -----------------------------
// Build App
// -----------------------------
var app = builder.Build();

// -----------------------------
// Middleware Pipeline
// -----------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Fix: ensure correct scheme from Azure reverse proxy
app.UseForwardedHeaders();

// Optional patch: force HTTPS scheme (prevents redirect loops)
app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    return next();
});

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

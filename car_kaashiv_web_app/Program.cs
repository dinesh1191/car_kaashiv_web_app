using car_kaashiv_web_app.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using car_kaashiv_web_app.Services.Extensions;
using car_kaashiv_web_app.Models.Enums;

var builder = WebApplication.CreateBuilder(args);
// Environment is available here
Console.WriteLine($"Running in: {builder.Environment.EnvironmentName}");

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine));

// Add services to the container
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); // hot reload, remove for prod

// Remove global AuthorizeFilter (too strict)
// If you want global authentication (for all controller and views), you must mark Login/Register controllers as [AllowAnonymous]
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
}).AddRazorRuntimeCompilation();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Home/Index";
        options.AccessDeniedPath = "/User/UnAuthorizes";
      
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.UseSession();          // session first
app.UseAuthentication();   // then authentication
app.UseAuthorization();    // then authorization

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

 app.Run();

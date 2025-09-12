using car_kaashiv_web_app.Data;   // namespace of AppDbContext
using car_kaashiv_web_app.Filters;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Register DbContext(database connection) with connection string
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
//builder.Services.AddControllersWithViews(); // uncomment on production
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
    //options.Filters.Add<AuthGuardAttribute>(); //apply globally
}).AddRazorRuntimeCompilation();// for hot reload Views comment it pushing to production;

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); //session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication("Cookies").AddCookie("Cookies", options =>
{
    options.LoginPath = "/User/Login"; //redirect if not logged in
    options.AccessDeniedPath = "/User/UnAuthorized";
});

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline. // default pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");


app.Run();

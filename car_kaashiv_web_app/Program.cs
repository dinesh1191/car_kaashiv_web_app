using car_kaashiv_web_app.Data;   // namespace of AppDbContext
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Register DbContext(database connection) with connection string
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
 .EnableSensitiveDataLogging()
 .LogTo(Console.WriteLine));


// Add services to the container.
//builder.Services.AddControllersWithViews(); // uncomment on production
builder.Services.AddControllersWithViews(options =>{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());   
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
    options.AccessDeniedPath = "/Home/Privacy";
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
    pattern: "{controller=Employee}/{action=EmployeeDashboard}/{id?}");


app.Run();

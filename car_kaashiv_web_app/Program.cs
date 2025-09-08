using car_kaashiv_web_app.Data;   // namespace of AppDbContext
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);



// Register DbContext with connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
    builder.Services.AddControllersWithViews() // for hot reload Views comment while pushing to prod
        .AddRazorRuntimeCompilation();


var app = builder.Build();

// Configure the HTTP request pipeline. // default pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");


app.Run();

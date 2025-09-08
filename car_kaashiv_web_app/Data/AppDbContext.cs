using car_kaashiv_web_app.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace car_kaashiv_web_app.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> tbl_user { get; set; } //Maps to DB Table
    }
}

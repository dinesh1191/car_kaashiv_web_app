using car_kaashiv_web_app.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace car_kaashiv_web_app.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TableUser> tbl_user { get; set; } //Maps to user Table
        public DbSet<TableEmployee> tbl_emp  { get; set; } //Maps to employee table 
    }
}

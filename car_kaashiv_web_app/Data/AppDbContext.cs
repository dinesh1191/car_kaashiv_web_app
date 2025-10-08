using car_kaashiv_web_app.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace car_kaashiv_web_app.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TableUser> tbl_user { get; set; } //Maps to user Table
        public DbSet<TableEmployee> tbl_emp  { get; set; } //Maps to employee table 
        public DbSet<TablePart> tbl_part { get; set; }  //Maps to parts table
        public DbSet<TableCart> tbl_cart { get; set; }  //Maps to cart table
        public DbSet<TableOrders> tbl_orders { get; set; }  //Maps to order table
        public DbSet<TableOrderItems> tbl_order_items { get; set; }  //Maps to cart orderItems
    }
}

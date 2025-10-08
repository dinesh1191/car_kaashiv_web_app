using car_kaashiv_web_app.Data;
using car_kaashiv_web_app.Models;
using car_kaashiv_web_app.Models.Enums;
using car_kaashiv_web_app.Services.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace car_kaashiv_web_app.Controllers
{

   
    public class OrderController : Controller
    {
        private readonly string? _connectionString;
        private readonly AppDbContext _context;       

       
        public OrderController(IConfiguration configuration, ILogger<EmployeeController> logger, AppDbContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        
        }

        [HttpGet]
        public IActionResult CheckoutSuccess()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Invoice()
        {
            return Invoice(1106);
            //return View();
        }
        public IActionResult Invoice(int orderId)
        {  
                      
            if (orderId <= 0)
            {             
                TempData.setAlert("Invalid order ID.", AlertTypes.Error);
                return RedirectToAction("UserPart"); 
            }

            var order = _context.tbl_orders.FirstOrDefault(o => o.OrderId == orderId);   
            var items = _context.tbl_order_items.Where(oi => oi.OrderId == orderId).ToList();
            var model = new InvoiceViewModel
            {

                InvoiceNumber = order.OrderId,
                OrderDate = order.CreatedAt,
                UserName = User.Identity?.Name,
                UserEmail = User.FindFirst(ClaimTypes.Email)?.Value,
                TotalAmount = order.TotalAmount,
                Items = items.Select(i => new InvoiceItem
                {
                    PartName = _context.tbl_part.FirstOrDefault(p => p.PartId == i.PartId)?.PName ?? "Unknown",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Total = i.totalPrice
                }).ToList()
,
            };
            return View(model);
        }

    }

    
}

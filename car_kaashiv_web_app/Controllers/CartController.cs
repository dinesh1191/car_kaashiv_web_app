using car_kaashiv_web_app.Data;
using car_kaashiv_web_app.Extensions;
using car_kaashiv_web_app.Models.DTOs;
using car_kaashiv_web_app.Models.Entities;
using car_kaashiv_web_app.Models.Enums;
using car_kaashiv_web_app.Services.Extensions;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace car_kaashiv_web_app.Controllers
{
    public class CartController : Controller
    {
        private readonly string? _connectionString;
        private readonly AppDbContext _context;
        private readonly ILogger<EmployeeController> _logger;

        public IActionResult Index()
        {
            return View();
        }
        public CartController(IConfiguration configuration, ILogger<EmployeeController> logger, AppDbContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _logger = logger;
        }

        [HttpPost]   
        public IActionResult AddToCart([FromBody] CartItemDto data)  
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"UserID:{userId}");

            if (userId == null) { 
            //return RedirectToAction("UnAuthorized","User" );
              return Json(new { success = false, message = "Unauthorized" });
            }
            //Check if the part exists
            int partId = data.Id;
            string PartName = data.Name;
            decimal partTotal = data.Price;

            var part = _context.tbl_part.FirstOrDefault(p => p.PartId == partId);          
            if (part == null)
            {
                return Json(new { success = false, message = "Part not found" });              
             }
            //check if item already exists in cart also changing the user id to string to integer
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userIdInt = int.Parse(userIdString!); // or use int.TryParse for safety
            var existingItem = _context.tbl_cart.FirstOrDefault(c => c.UId == userIdInt && c.PartID == partId);
            
            if(existingItem != null)
            {
             /*TempData.setAlert("Item already exists in cart.", AlertTypes.Error);     */         
                return Json(new { success = false, message = "Item already in cart" });
            }
            else
            {
                var newCartItem = new TableCart
                {                 
                    UId       = userIdInt,
                    PartID    = partId,
                    Quantity  = 1,
                    AddedDate = DateTime.UtcNow.ToIST()
                 };
              _context.tbl_cart.Add(newCartItem);
            }
              _context.SaveChanges();
            //optional : return cart count ot update UI
            var cartCount = _context.tbl_cart.Count(c => c.UId == userIdInt);           
            return Json(new { success = true,cartCount });
        }

    }
}

using car_kaashiv_web_app.Data;
using car_kaashiv_web_app.Extensions;
using car_kaashiv_web_app.Models;
using car_kaashiv_web_app.Models.DTOs;
using car_kaashiv_web_app.Models.Entities;
using car_kaashiv_web_app.Models.Enums;
using car_kaashiv_web_app.Services.Extensions;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;
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
        // View cart items
        [HttpGet]
        public IActionResult ViewCart()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//string? means the variable can hold a null value.         
            if(!int.TryParse(userId, out int userIdInt))//userIdInt is the output variable that will hold the converted integer if the parsing succeeds.
            {
                NotFound();
            }
            var cartItems = from c in _context.tbl_cart
                       join p in _context.tbl_part on c.PartID equals p.PartId
                       where c.UId == userIdInt
                       select new CartViewModel
                       {
                           //c.CartId,
                           //p.PName,
                           //p.PPrice,
                           //c.Quantity,
                           //Total = c.Quantity * p.PPrice
                           CartId = c.CartId,
                           PartName = p.PName,
                           UnitPrice = p.PPrice,
                           Quantity = c.Quantity,
                           Total = c.Quantity * p.PPrice

                       };

            //return View(cartItems.ToList());
            return PartialView("_CartPartial", cartItems.ToList());
        }
        //update quantity cart items
        [HttpPost]
        public IActionResult UpdateQuantity(int cartId,int quantity)
        {
            var item = _context.tbl_cart.FirstOrDefault(c => c.CartId == cartId);
            if (item == null)
            {
                return NotFound();
            }
            item.Quantity = quantity;              
            return Json(new {success = true});
        }


        [HttpPost]//Remove item from cart
        public IActionResult RemoveFromCart(int cartId)
        {
            var item = _context.tbl_cart.FirstOrDefault(c => c.CartId == cartId);
            if (item == null) {
                return NotFound();            
            }
            _context.tbl_cart.Remove(item);
            _context.SaveChanges();
            return Json (new {success = true});
        }

        [HttpPost]   
        public IActionResult Checkout()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//string? means the variable can hold a null value.
            int.TryParse(userId, out int Uid);
            var cartItems = _context.tbl_cart.Where(c => c.UId == Uid);
           
            if (!cartItems.Any()) {
                return BadRequest("Cart Is empty");
            }

            // Create new order
            var newOrder = new TableOrders //internal class on controllers
            {
                UId = userId,
                TotalAmount = 0, //update below
                Status = "Pending",
                CreatedAt = DateTime.Now.ToIST(),
            };
            _context.tbl_orders.Add(newOrder);
            _context.SaveChanges(); //save once to get OrderId
            
            decimal total = 0;
             
            // Copy cart items into OrderItems

            foreach (var item in cartItems) {
                var part = _context.tbl_part.Find(item.PartID);
                if (part == null)  continue;
                var lineTotal = part.PPrice * item.Quantity;
                total += (decimal)lineTotal!;

                _context.tbl_order_items.Add(new TableOrderItems
                {
                    OrderItemId = newOrder.OrderId,
                    PartId = item.PartID,
                    Quantity = item.Quantity,
                    UnitPrice = part.PPrice,
                    totalPrice = lineTotal
                });
            }

            // Update total amount
            newOrder.TotalAmount = total;
            _context.SaveChanges();

            // Clear the cart
            _context.tbl_cart.Remove((TableCart)cartItems);
            _context.SaveChanges();
            return Json(new {success = true,orderId = newOrder.OrderId });  
        }
        // Add item to Cart
        public IActionResult AddToCart([FromBody] CartItemDto data)  
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null) { 
            return RedirectToAction("UnAuthorized","User" );           
            }
            // Check if the part exists
            int partId = data.Id;
            string PartName = data.Name!;
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

        public IActionResult GetCartPartial()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;           
            if (!int.TryParse(userId, out int Uid))
            {
                return RedirectToAction("UnAuthorized", "User");
            }


            var cartItems = _context.tbl_cart
                            .Where(c => c.UId == Uid)
                            .Join(_context.tbl_part, c => c.PartID, p => p.PartId, (c, p) => new CartViewModel
                            {
                                CartId    = c.CartId,
                                PartName  = p.PName,
                                Quantity  = c.Quantity,
                                UnitPrice = p.PPrice,
                                Total     = p.PPrice * c.Quantity
                            }).ToList();                          
                                        
           return PartialView("_CartPartial", cartItems);
        }
    }
}

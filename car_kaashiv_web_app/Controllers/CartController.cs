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
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // string? means the variable can hold a null value.         
            if(!int.TryParse(userId, out int userIdInt))    //  UserIdInt is the output variable that will hold the converted integer if the parsing succeeds.
            {
                NotFound();
            }
            var cartItems = from c in _context.tbl_cart
                            join p in _context.tbl_part on c.PartID equals p.PartId
                            where c.UId == userIdInt
                            select new CartViewModel
                           {                          
                            CartId    = c.CartId,
                            PartName  = p.PName,
                            UnitPrice = p.PPrice,
                            Quantity  = c.Quantity,
                            Total     = c.Quantity * p.PPrice
                           };

            //return View(cartItems.ToList());
            return PartialView("_CartPartial", cartItems.ToList());
        }
        //  Get view of cart on partial modal controller
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
                                CartId = c.CartId,
                                PartName = p.PName,
                                Quantity = c.Quantity,
                                UnitPrice = p.PPrice,
                                Total = p.PPrice * c.Quantity
                            }).ToList();

            return PartialView("_CartPartial", cartItems);
        }
        [HttpPost] // Add item to Cart  
        public IActionResult AddToCart([FromBody] CartItemDto data)
        {
            if (data == null || data.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid request payload." });
            }

            // Validate user identity
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { success = false, message = "User not authenticated." });
            }

            // Validate part existence
            var part = _context.tbl_part.FirstOrDefault(p => p.PartId == data.Id);
            if (part == null)
            {
                return Json(new { success = false, message = "Part not found." });
            }

            // Check if already in cart
            var existingItem = _context.tbl_cart.FirstOrDefault(c => c.UId == userId && c.PartID == data.Id);
            if (existingItem != null)
            {
                return Json(new { success = false, message = "Item already in cart." });
            }

            // Add new cart item
            var newCartItem = new TableCart
            {
                UId = userId,
                PartID = data.Id,
                Quantity = 1,
                AddedDate = DateTime.UtcNow.ToIST()
            };

            _context.tbl_cart.Add(newCartItem);
            _context.SaveChanges();

            // Return updated cart count for UI
            var cartCount = _context.tbl_cart.Count(c => c.UId == userId);
            return Json(new
            {
                success = true,
                message = "Item added to cart successfully.",
                cartCount
            });
        }

        [HttpPost] // Remove item from cart
        public IActionResult RemoveFromCart([FromBody] int cartId)
        {
            var item = _context.tbl_cart.FirstOrDefault(c => c.CartId == cartId);
            if (item == null)
            {
                return Json(new { success = false, message = "Item not found." });
            }
            _context.tbl_cart.Remove(item);
            _context.SaveChanges();
            return Json(new { success = true, message = "Item removed successfully" });
        }

        [HttpPost]  //update quantity cart items
        public IActionResult UpdateQuantity([FromBody] UpdateQuantityRequestDto request)
        {
            if (request == null)
            { 
                return Json(new { success = false, message = "Invalid request." });
            }

            var item = _context.tbl_cart.FirstOrDefault(c => c.CartId == request.CartId);

            if (item == null)
            {
                return Json(new { success = false, message = "Item not found." });
            }

            if (request.Quantity < 1)
            {
                return Json(new { success = false, message = "Invalid quantity." });
            }

            item.Quantity = request.Quantity;
            _context.SaveChanges();

            return Json(new { success = true, message = "Quantity updated successfully." });
        } 
        

                    
       
        [HttpPost] // Check out by user customer
        public IActionResult Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;            
            if (!int.TryParse(userId, out int Uid))
            {
                return RedirectToAction("UnAuthorized", "User");
            }

            var cartItems = _context.tbl_cart.Where(c => c.UId == Uid);

            if (!cartItems.Any())
            {
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
            _context.SaveChanges(); // save once to get OrderId

            decimal total = 0;

            // Copy cart items into OrderItems

            foreach (var item in cartItems)
            {
                var part = _context.tbl_part.Find(item.PartID);
                if (part == null) continue;
                var lineTotal = part.PPrice * item.Quantity;
                total += (decimal)lineTotal!;

                _context.tbl_order_items.Add(new TableOrderItems
                {
                    OrderId = newOrder.OrderId,
                    PartId = item.PartID,
                    Quantity = item.Quantity,
                    UnitPrice = part.PPrice,
                    totalPrice = lineTotal
                });

                // Reduce stock
                var partInventory = _context.tbl_part.FirstOrDefault(p => p.PartId == item.PartID);
                if(partInventory != null)
                {
                    partInventory.PStock -= item.Quantity;
                    if(partInventory.PStock <0) partInventory.PStock = 0; // Prevent negative stock
                }
            }

            // Update total amount
            newOrder.TotalAmount = total;
            _context.SaveChanges();

            

            // Clear the cart
            _context.tbl_cart.RemoveRange(cartItems);
            _context.SaveChanges();
            return Json(new { success = true, message = "Placed order",orderId = newOrder.OrderId });
          }





    }

}

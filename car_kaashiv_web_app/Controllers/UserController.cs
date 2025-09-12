using BCrypt.Net;
using car_kaashiv_web_app.Data;
using car_kaashiv_web_app.Models.DTOs;
using car_kaashiv_web_app.Models.Entities;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;


namespace car_kaashiv_web_app.Controllers
{
    public class UserController : Controller
    {
        private readonly string? _connectionString;
        private readonly AppDbContext _context;
        // inject IConfiguration +AppContext      
        public UserController(IConfiguration configuration, AppDbContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }
        //Show Register page
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        //show Login Page
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
                 
            return View();
                    
        }
        
        public IActionResult Logout()
        {
            //clear all session values
            HttpContext.Session.Clear();
            TempData["Message"] = "You have been logged out.";
            return RedirectToAction("Login", "User");
        }
      
        public IActionResult UserDashboard()
        {
            var phone = HttpContext.Session.GetString("UserPhone");
            var name = HttpContext.Session.GetString("UserName");
            //if(string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(name))
            //{
            //    TempData["Message"] = "Please login first";
            //    return RedirectToAction("Login", "User");
            //}
            
            ViewBag.Phone = phone;
            ViewBag.UserName = name;
            return View();
        }
        //Handle Register Post
        [HttpPost]
        public IActionResult Register(RegisterUserDto dto)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            //Map DTO -> Entity;
            //duplicate phone number restricted
            if (_context.tbl_user.Any(u => u.Phone == dto.Phone))
            {
                ModelState.AddModelError(nameof(dto.Phone), "Phone number already registered");
                return View(dto);
            }
            var user = new TableUser
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };
            _context.tbl_user.Add(user);
            _context.SaveChanges();

            //Auto login (simple session based)
            //?? coalesce to an empty string when storing session
            HttpContext.Session.SetString("UserName", user?.Name ?? string.Empty);
            HttpContext.Session.SetString("UserPhone", user?.Phone ?? string.Empty);
            TempData["Message"] = "User registered successfully!";
            var check = HttpContext.Session.GetString("UserName");
            Console.WriteLine($"Session test;{check}");
            return RedirectToAction("UserDashboard","User");
         
        }
        [HttpPost]
        public IActionResult Login(LoginDto dto)
        {

            //if (!ModelState.IsValid)
            //{
            //    return View(dto);
            //}
            //Map DTO -> Entity
            // using (var con = new SqlConnection(_connectionString))
            // {
            //     var parameters = new DynamicParameters();
            //     parameters.Add("@u_phone", dto.Phone);
            //     parameters.Add("@u_pass", dto.Password);
            //     parameters.Add("@statusCode", dbType: System.Data.DbType.Int32, direction: ParameterDirection.Output);
            //     parameters.Add("@statusMessage", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            //     con.Execute("sp_AuthenticateUser", parameters, commandType: CommandType.StoredProcedure);

            //     int code = parameters.Get<int>("@statusCode");
            //     string message = parameters.Get<string>("@statusMessage");

            //     TempData["Message"] = $"Code: {code} - {message}";

            //     if(code == 200)
            //     {
            //         // Success → redirect to dashboard or home
            //         return RedirectToAction("Privacy", "Home");

            //     }
            //     else
            //     {                    
            //         ModelState.AddModelError(string.Empty, message);
            //         return View(); //stays on Login page
            //     }

            //}
            if (!ModelState.IsValid) return View(dto);

            // Check user existence on table  
            var user = _context.tbl_user.FirstOrDefault(u => u.Phone == dto.Phone);
             //the above run query on db
            //SELECT TOP(1) *FROM tbl_user WHERE Phone = @p0

            if (user == null)
            {
                TempData["Message"] = "User does not exist in database.";
                return View(dto);
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                TempData["Message"] = "Invalid password.";                
                return View(dto);
            }           
            HttpContext.Session.SetString("UserName", user?.Name ?? string.Empty);
            HttpContext.Session.SetString("UserPhone", user?.Phone ?? string.Empty);
            TempData["Message"] = "Login successful!";

            //var check = HttpContext.Session.GetString("UserName");
            //Console.WriteLine($"Session test;{check}");
                return RedirectToAction("UserDashboard", "User");       
        }
    }
}

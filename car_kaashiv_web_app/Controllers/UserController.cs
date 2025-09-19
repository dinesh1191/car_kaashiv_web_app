using BCrypt.Net;
using System.Security.Claims;
using car_kaashiv_web_app.Data;
using car_kaashiv_web_app.Models.DTOs;
using car_kaashiv_web_app.Models.Entities;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Numerics;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using car_kaashiv_web_app.Services;
using car_kaashiv_web_app.Extensions;


namespace car_kaashiv_web_app.Controllers
{
    public class UserController : Controller
    {
        private readonly string? _connectionString;
        private readonly AppDbContext _context;
        private AuthenticationProperties? authProperties;
        private readonly ILogger<HomeController> _logger;    

        // inject IConfiguration +AppContext      
        public UserController(IConfiguration configuration, ILogger<HomeController> logger, AppDbContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult UnAuthorized()//<--Marking an action with [AllowAnonymous] explicitly overrides this rule and skips the authentication checks.
        {
            return View();
        }
        //Show Register page
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()//<--Marking an action with [AllowAnonymous] explicitly overrides this rule and skips the authentication checks.
        {
            return View();
        }
        //show Login Page
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()//<--Marking an action with [AllowAnonymous] explicitly overrides this rule and skips the authentication checks.
        {
            
            return View();
                    
        }
        
        public IActionResult Logout()
        {
            //clear all session cookies 
            //Response.Cookies.Delete("UserName");
            //Response.Cookies.Delete("UserPhone");
            //clearing all cookies 
            foreach(var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }               
            TempData["Message"] = "You have been logged out.";
            TempData["MessageType"] = "warning"; // for css
            return RedirectToAction("Index", "Home");
        }


        public IActionResult UserDashboard()
        {          
            return View();
        }

        //Handle Register Post
        [HttpPost]
        [AllowAnonymous]    //<--Marking an action with [AllowAnonymous] explicitly overrides this rule and skips the authentication checks. 
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {               

            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            //Map DTO -> Entity-> User.cs;
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
            await _context.SaveChangesAsync(); // wait till db execution completes
            // Build claims for cookie authentication
            var claims = ClaimsHelper.BuildUserClaims(user);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
            TempData["Message"] = "User registered successfully!";
            var isAuth = User.Identity?.IsAuthenticated ?? false;           
            _logger.LogInformation("User {UserName} with phone {Phone} logged in at {Time}", user?.Name, user?.Phone, DateTime.UtcNow.ToIST());
            return RedirectToAction("UserDashboard", "User");     
        }
       
        [HttpPost]
        [AllowAnonymous]   //<--Marking an action with [AllowAnonymous] explicitly overrides this rule and skips the authentication checks. 
        [ValidateAntiForgeryToken] // <--- Anti-forgery check here avoid cross-site requests tokens     
        public async Task<IActionResult> Login(LoginDto dto)
        {          
            if (!ModelState.IsValid) return View(dto);
            // Check user existence on table by unique phone number  
            var user = await _context.tbl_user.FirstOrDefaultAsync(u => u.Phone == dto.Phone);   // Query will run SELECT TOP(1) *FROM tbl_user WHERE Phone = @p0     
            if (user == null)
            {
                TempData["Message"] = "User does not exist in database.";
                return View(dto);
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                TempData["Message"] = "Invalid password.";
                TempData["MessageType"] = "error";
                return View(dto);
            }
            // Build claims
            var claims = ClaimsHelper.BuildUserClaims(user);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };    
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),authProperties);           
            // Log user info instead of session
            _logger.LogInformation("User {UserName} with phone {Phone} logged in at {Time}",user?.Name,user?.Phone,DateTime.UtcNow.ToIST());         
            TempData["Message"] = "Login successful!";
            TempData["MessageType"] = "success";
            var name = User.Identity?.Name; // comes from ClaimTypes.Name
            var phone = User.FindFirst(ClaimTypes.MobilePhone)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            return RedirectToAction("UserDashboard", "User");       
        }
    }
}

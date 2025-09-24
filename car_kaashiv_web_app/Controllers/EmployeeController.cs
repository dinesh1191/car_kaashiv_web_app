using car_kaashiv_web_app.Data;
using car_kaashiv_web_app.Extensions;
using car_kaashiv_web_app.Models.DTOs;
using car_kaashiv_web_app.Models.Entities;
using car_kaashiv_web_app.Models.Enums;
using car_kaashiv_web_app.Services;
using car_kaashiv_web_app.Services.Extensions;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;// required for SelectListItem
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace car_kaashiv_web_app.Controllers


{
    public class EmployeeController : Controller
    {
        private readonly string? _connectionString;
        private readonly AppDbContext _context;
        private readonly ILogger<EmployeeController> _logger;
        public EmployeeController(IConfiguration configuration, ILogger<EmployeeController> logger, AppDbContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _logger = logger;

        }

        // inject IConfiguration +AppContext 
        [AllowAnonymous]
        public IActionResult EmployeeLogin()
        {
            //var model = new EmployeeLoginDto();
            //return View(model);
            return View();
        }
        [AllowAnonymous]
        public IActionResult EmployeeRegister()
        {
            var model = new EmployeeRegisterDto();
            return View("EmployeeRegister", model);
        }


        [Authorize(Roles = "admin")]
        public IActionResult EmployeeDashboard()
        {

            return View();
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail(string email)
        {
            bool exists = await _context.tbl_emp.AnyAsync(emp => emp.Email == email);
            if (exists)
            {
                return Json(false); //trigger the Remote ErrorMessage
            }
            return Json(true);      //passes validation
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] //<--Marking an action with [AllowAnonymous] explicitly overrides this rule and skips the authentication checks. 
        public async Task<IActionResult> RegisterEmp(EmployeeRegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View("EmployeeRegister", model);
            }
            if (_context.tbl_emp.Any(emp => emp.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email  already registered");
                return View("EmployeeRegister", model);
            }
            var employee = new TableEmployee
            {
                Name = model.Name,
                Phone = model.Phone,
                Email = model.Email,
                Role = model.Role,
                EmpPasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CreatedAt = DateTime.UtcNow.ToIST()
            };
            _context.tbl_emp.Add(employee);
            _context.SaveChanges();
            // Build claims for cookie authentication
            var claims = ClaimsHelper.BuildUserClaimsEmp(employee);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
            TempData.setAlert("Employee registered successfully!", AlertTypes.Success);          
            return RedirectToAction("EmployeeDashboard", "Employee");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]//<--Marking an action with [AllowAnonymous] explicitly overrides this rule and skips the authentication checks.
        public async Task<IActionResult> LoginEmp(EmployeeLoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View("EmployeeLogin", model);
            }
            var employee = await _context.tbl_emp.FirstOrDefaultAsync(emp => emp.Email == model.Email);
           
            if (employee == null)
            {
                TempData.setAlert("Employee not Registered!", AlertTypes.Error);                
                return View("EmployeeLogin", model);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Password, employee.EmpPasswordHash))
            {
           
                TempData.setAlert("Invalid Password!", AlertTypes.Error);                
                return View("EmployeeLogin", model);
            }
            // Build claims
            var claims = ClaimsHelper.BuildUserClaimsEmp(employee);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
            _logger.LogInformation("Employee {UserName} with email {Email} logged in at {Time}", employee?.Name, employee?.Email, DateTime.UtcNow.ToIST());
            TempData.setAlert("Login successful!", AlertTypes.Success);           
            var name = User.Identity?.Name; // comes from ClaimTypes.Name
            var phone = User.FindFirst(ClaimTypes.MobilePhone)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            return RedirectToAction("EmployeeDashboard", "Employee");

        }
    }
}

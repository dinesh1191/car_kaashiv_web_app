using car_kaashiv_web_app.Data;
using car_kaashiv_web_app.Models.DTOs;
using car_kaashiv_web_app.Models.Entities;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;// required for SelectListItem
using Microsoft.EntityFrameworkCore;

namespace car_kaashiv_web_app.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly string? _connectionString;
        private readonly AppDbContext _context;
        public EmployeeController(IConfiguration configuration, AppDbContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }
        // inject IConfiguration +AppContext 
        [AllowAnonymous]     
        public IActionResult EmployeeRegister()
        {
            var model = new EmployeeRegisterDto();
            return View(model);
        }
       
        public IActionResult EmployeeDashboard()
        {
          
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] //<--Marking an action with [AllowAnonymous] explicitly overrides this rule and skips the authentication checks. 
        public IActionResult RegisterEmp(EmployeeRegisterDto model)
        {
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (_context.tbl_emp.Any(emp => emp.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email  already registered");
                return View(model);
            }
            var employee = new TableEmployee
            {
                Name  = model.Name,
                Phone = model.Phone,
                Email = model.Email,
                Role  = model.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CreatedAt = DateTime.UtcNow
            };
            _context.tbl_emp.Add(employee);
            _context.SaveChanges();
            return RedirectToAction("UserDashboard", "User");
        }
        
    }
}

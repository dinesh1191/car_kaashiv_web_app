using car_kaashiv_web_app.Data;
using car_kaashiv_web_app.Models.DTOs;
using car_kaashiv_web_app.Models.Entities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
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
        public IActionResult Register()
        {
            return View();
        }
        //show Login Page
        [HttpGet]
        public IActionResult Login()
        {
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
            //Map DTO -> Entity

            var user = new User
            {
                U_Name = dto.Name,
                U_Phone = dto.Phone,
                U_Pass = dto.Password,
            };
            _context.tbl_user.Add(user);
            _context.SaveChanges();

            TempData["Message"] = "User registered successfully!";
            return RedirectToAction("Privacy","Home");
          
        }
        [HttpPost]
        public IActionResult Login(LoginDto dto)
        {
            
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            //Map DTO -> Entity
            using (var con = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@u_phone", dto.Phone);
                parameters.Add("@u_pass", dto.Password);
                parameters.Add("@statusCode", dbType: System.Data.DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@statusMessage", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

                con.Execute("sp_AuthenticateUser", parameters, commandType: CommandType.StoredProcedure);

                int code = parameters.Get<int>("@statusCode");
                string message = parameters.Get<string>("@statusMessage");

                TempData["Message"] = $"Code: {code} - {message}";

                if(code == 200)
                {
                    // Success → redirect to dashboard or home
                    return RedirectToAction("Privacy", "Home");

                }
                else
                {                    
                    ModelState.AddModelError(string.Empty, message);
                    return View(); //stays on Login page
                }
                    
           }
        }
    }
}

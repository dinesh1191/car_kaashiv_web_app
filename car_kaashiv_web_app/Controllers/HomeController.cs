using car_kaashiv_web_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace car_kaashiv_web_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [AllowAnonymous] //<--Marking an action with [AllowAnonymous] explicitly overrides this rule and skips the authentication checks.
        public IActionResult Privacy()
        {
            return View();
        }
        [AllowAnonymous]//<--Marking an action with [AllowAnonymous] explicitly overrides this rule and skips the authentication checks.
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

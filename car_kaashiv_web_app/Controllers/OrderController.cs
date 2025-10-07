using Microsoft.AspNetCore.Mvc;

namespace car_kaashiv_web_app.Controllers
{

   
    public class OrderController : Controller
    {
        [HttpGet]
        public IActionResult OrderConfirm() 
        {
            return RedirectToAction("CheckoutSuccess", "Order");
        }
    }
}

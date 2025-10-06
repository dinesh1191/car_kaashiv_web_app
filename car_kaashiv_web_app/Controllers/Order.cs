using car_kaashiv_web_app.Models.Entities;

namespace car_kaashiv_web_app.Controllers
{
    internal class Order : TableOrders
    {
        public int u_id { get; set; }
        public int total_amount { get; set; }
        public string? StatusCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
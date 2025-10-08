
namespace car_kaashiv_web_app.Models
{
    internal class OrderSummary 
    {
        public int u_id { get; set; }
        public int total_amount { get; set; }
        public string? StatusCode { get; set; }
        public DateTime OrderCreatedAt { get; set; }
    }
}
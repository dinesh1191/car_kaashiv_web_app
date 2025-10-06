namespace car_kaashiv_web_app.Models
{
    public class CartViewModel
    {
        
        public int CartId { get; set; }
        public string? PartName { get; set; }
        public int? Quantity { get; set; }    
        public decimal? UnitPrice { get; set; }
        public decimal? Total { get; set; }
    }

}

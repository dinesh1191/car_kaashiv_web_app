namespace car_kaashiv_web_app.Models
{
    public class InvoiceViewModel
    {
        public int? InvoiceNumber { get; set; }   
        public DateTime? OrderDate { get; set; } 
        public string? UserName { get; set; }       
        public string? UserEmail { get; set; }
        public decimal? TotalAmount { get; set; }
        public List<InvoiceItem>? Items { get; set; }


    }

    public class InvoiceItem
    {
        public string? PartName { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }        
        public decimal? Total { get; set; }
    }
}

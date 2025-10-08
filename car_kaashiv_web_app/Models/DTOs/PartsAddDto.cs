using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using car_kaashiv_web_app.Attributes;
using Microsoft.AspNetCore.Http;

namespace car_kaashiv_web_app.Models.DTOs
{
    public class PartsAddDto  

    {
        //[MinLength(3, ErrorMessage = "Part Id Required")]
        //[Required(ErrorMessage = "Part Id number required")]
        //public int? PartId { get; set; }

       [Required(ErrorMessage = "Emp id number required")]       
        public int PEmpId { get; set; }

        [MinLength(3, ErrorMessage = "Name must 3 characters minimum,maximum 25 characters")]
        [Required(ErrorMessage = "Part name number required")]       
        public string? PName { get; set; }

        
        [Required(ErrorMessage = "Part detail number required")]      
        public string? PDetail { get; set; }


        [Required(ErrorMessage = "Par price required")]
        public decimal? PPrice { get; set; }

        [Required(ErrorMessage = "Part stock required")]
        public int? PStock { get; set; }

        [Required(ErrorMessage = "Part image is required")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        
        [MaxFileSize(2 * 1024 * 1024)] // 2MB
        
        public IFormFile? ImagePath { get; set; }
                
    }
}

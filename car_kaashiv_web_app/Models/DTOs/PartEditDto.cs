using car_kaashiv_web_app.Attributes;
using Microsoft.CodeAnalysis.Scripting;
using System.ComponentModel.DataAnnotations;
using System.IO.Pipelines;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace car_kaashiv_web_app.Models.DTOs
{
    public class PartEditDto
    {

        [Required(ErrorMessage = "Emp id number required")]
        public int PartId { get; set; }
        public int PEmpId { get; set; }

        [MinLength(3, ErrorMessage = "Name must 3 characters minimum,maximum 25 characters")]
        [Required(ErrorMessage = "Part name number required")]
        public string? PName { get; set; }


        [Required(ErrorMessage = "Part detail number required")]
        public string? PDetail { get; set; }


        [Required(ErrorMessage = "Par price required")]
        public string? PPrice { get; set; }

        [Required(ErrorMessage = "Part stock required")]
        public string? PStock { get; set; }

        //[Required(ErrorMessage = "Part image is required")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]

        [MaxFileSize(2 * 1024 * 1024)] // 2MB

        // For displaying existing image
        // For uploading a new image
        public IFormFile? ImageUpload { get; set; }
        public string? ImageUrl { get; set; }
    }
}



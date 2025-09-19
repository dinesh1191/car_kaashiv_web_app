using System.ComponentModel.DataAnnotations;

namespace car_kaashiv_web_app.Models.DTOs
{
    public class EmployeeLoginDto
    {
        
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@kaashiv\.com$", ErrorMessage = "Only official kaashiv.com email IDs are allowed")]
        [Required(ErrorMessage = "Official Email Required")]
        public String? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Password must be 10 characters long")]
        [DataType(DataType.Password)]
        public String? Password { get; set; }
    }

}

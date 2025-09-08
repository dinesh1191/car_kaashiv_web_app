using System.ComponentModel.DataAnnotations;

namespace car_kaashiv_web_app.Models.DTOs
{
    public class LoginDto
    {
       
        [Required(ErrorMessage = "Phone number is required")]

        [MinLength(10, ErrorMessage ="Phone number must be at least 10 characters")]
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public String? Phone {  get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(10,MinimumLength =10,ErrorMessage = "Password must be 10 characters long")]
        [DataType(DataType.Password)]
        public String? Password { get; set; }

    }
}

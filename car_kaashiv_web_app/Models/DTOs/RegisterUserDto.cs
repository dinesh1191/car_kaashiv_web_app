using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace car_kaashiv_web_app.Models.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public DateTime createdAt { get; set; }

        [MinLength(3, ErrorMessage = "Name must 3 characters minimum,maximum 25 characters")]
        [Required(ErrorMessage = "Name number required")]
        public String? Name { get; set; }

        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must start with a digit from 6 to 9 and be 10 digits long")]
        [Required(ErrorMessage = "Phone number required")]
        public String? Phone { get; set; }
        
        [Required(ErrorMessage = "Email is required")] 
        public String? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+=\[{\]};:<>|./?,-]).{8,10}$",
         ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        [DataType(DataType.Password)]
        public String? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]        
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public String? CPassword { get; set;}

    }
}


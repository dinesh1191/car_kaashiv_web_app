using System.ComponentModel.DataAnnotations;

namespace car_kaashiv_web_app.Models.DTOs
{
    public class RegisterUserDto
    {
        [StringLength(3, ErrorMessage = "Name must 3 characters minimum,maximum 25 characters")]
        public String? Name { get; set; }

        [MinLength(10, ErrorMessage = "Phone number must be at least 10 characters")]
        public String? Phone { get; set; }
        
        
        [DataType(DataType.Password)]
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


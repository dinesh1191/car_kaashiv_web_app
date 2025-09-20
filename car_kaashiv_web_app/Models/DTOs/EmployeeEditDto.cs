using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace car_kaashiv_web_app.Models.DTOs
{
    public class EmployeeEditDto
    {
        public int Id { get; set; } // needed for Edit

        [MinLength(3, ErrorMessage = "Name must be at least 3 characters, max 25")]
        [Required(ErrorMessage = "Name required")]
        public string? Name { get; set; }

        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone must start 6-9 and be 10 digits")]
        [Required(ErrorMessage = "Phone required")]
        public string? Phone { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@kaashiv\.com$", ErrorMessage = "Only official kaashiv.com email IDs allowed")]
        [Required(ErrorMessage = "Official Email required")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Role required")]
        [RegularExpression("admin|staff", ErrorMessage = "Valid role required")]
        public string? Role { get; set; }
        public List<SelectListItem> RoleList { get; set; }

        public DateTime CreatedAt { get; set; }
        public EmployeeEditDto()
        {
            RoleList = new List<SelectListItem>
            {
                new SelectListItem { Value = "admin", Text = "Admin" },
                new SelectListItem { Value = "staff", Text = "Staff" }
            };
        }

    }
}

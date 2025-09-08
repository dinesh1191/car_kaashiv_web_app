using System.ComponentModel.DataAnnotations;

namespace car_kaashiv_web_app.Models.Entities
{
    public class User
    {
        [Key]
        public int U_Id {  get; set; }  //Auto-increment Primary key
        
        [Required, StringLength(50)]
        public string? U_Name { get; set; }
        [Required,StringLength(50)]
        public string? U_Phone { get; set; }
        [Required,StringLength(255)]
        public string? U_Pass { get; set; }
   
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace car_kaashiv_web_app.Models.Entities
{
    public class TableUser 
    {
        [Key]        
        
        
        [Column("u_id")] // Db Column name
        public int Id {  get; set; }  // Primary key Auto-increment 

        [Required, StringLength(50)]
        [Column("u_name")]
        public string? Name { get; set; }    // non-nullable with default
        [Required,StringLength(50)]
        [Column("u_phone")]
        public string?Phone { get; set; }


        [Required,StringLength(50)]
        [Column("u_email")]
        public String? Email { get; set; }
       
        [Required,StringLength(255)]
        [Column("u_pass")]
        public string? PasswordHash { get; set; } 

        [Column("idt")]  // date time on idt column  db 
        public  DateTime CreatedAt { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace Social_Media_Website.Models
{
    public class Register
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string Email { get; set; }
        
    }
}

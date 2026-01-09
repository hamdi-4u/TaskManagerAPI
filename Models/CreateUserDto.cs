using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models
{
   
    public class CreateUserDto
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "username must be between 3 and 50 characters")]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "password must be at least 6 characters")]
        public string? Password { get; set; }

        public string? Role { get; set; }
    }
}
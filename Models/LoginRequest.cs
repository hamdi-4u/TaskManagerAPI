using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models
{
    
    public class LoginRequest
    {
        [Required(ErrorMessage = "username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
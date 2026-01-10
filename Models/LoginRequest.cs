using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models
{
   
    /// Data transfer object for user authentication.
    /// Contains the credentials needed to log into the system.

    public class LoginRequest
    {
        
        /// The username for authentication
       
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

       
        /// The user's password for authentication.
        
        /// This will be verified against the stored password hash.
        
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
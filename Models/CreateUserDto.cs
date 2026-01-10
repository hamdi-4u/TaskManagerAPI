using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models
{
    
    ////// Data transfer object for creating or updating a user.
    //// Contains user credentials, profile information, and role assignment.
   
    public class CreateUserDto
    {
       
        /// Unique username for login and identification.
        /// Must be between 3 and 50 characters.
       
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; } = string.Empty;

      
        /// User's email address for notifications and account recovery.
        
        /// Must be a valid email format.
  
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

       
        /// User's password (will be hashed before storage).
        
        /// Must be at least 6 characters long for security.
       
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

    
        /// User's role in the system (e.g., "Admin" or "User").
        
        /// Determines access permissions and capabilities.
        
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
    }
}
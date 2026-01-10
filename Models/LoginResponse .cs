namespace TaskManagerAPI.Models
{
    
    /// Response object returned after successful user authentication.
    /// Contains the authentication token and user information.
   
    public class LoginResponse
    {
       
       
        public string Token { get; set; } = string.Empty;

     
        public string TokenType { get; set; } = "Bearer";

      
        public DateTime ExpiresAt { get; set; }

       
        /// Basic information about the authenticated user.
        /// Includes user ID, username, email, and role.
       
        public UserDto User { get; set; } = null!;
    }
}
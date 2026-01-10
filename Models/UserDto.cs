namespace TaskManagerAPI.Models
{
    /// <summary>
    /// Data transfer object for user information in API responses.
    /// </summary>
    public class UserDto
    {


        ///// Unique identifier for the user
        public int Id { get; set; }

      
        /// The user's unique username for login and identification
        public string Username { get; set; } = string.Empty;

        
        /// The user's email address for notifications and account recovery
        public string Email { get; set; } = string.Empty;

        
        //// The user's role in the system ("Admin" or "User").
        ///// Determines access permissions and capabilities.
       
        public string Role { get; set; } = string.Empty;

       
        /// Timestamp when the user account was created (in UTC)
        public DateTime CreatedAt { get; set; }
    }
}
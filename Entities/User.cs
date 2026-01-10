namespace TaskManagerAPI.Entities
{

    /// Represents a user in the task management system
    public class User
    {

        //// Unique identifier for the user
        public int Id { get; set; }

        /// The user's chosen username for login
        public string Username { get; set; } = string.Empty;

        //// Hashed password for security  never store plain text passwords!
        public string PasswordHash { get; set; } = string.Empty;

        /// User's email address for notifications and account recovery
        public string Email { get; set; } = string.Empty;

        //// The user's role in the system (e.g., Admin, User, Manager)
        public Role Role { get; set; }

        /// When this user account was created (stored in UTC)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // All tasks assigned to or created by this user
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        //// Creates a new user with default values 
        
     
      /// Constructor can be used for additional initialization if needed
        public User() {  }
    }
}

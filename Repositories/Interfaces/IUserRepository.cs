
using TaskManagerAPI.Entities;

namespace TaskManagerAPI.Repositories.Interfaces
{

    // User repository interface - handles all user data operations
    public interface IUserRepository
    {
        // Fetch all users (might need to add filters later for admin panel)
        Task<IEnumerable<User>> GetAllAsync();

        // Find user by username - mainly for login purposes
        Task<User?> GetByUsernameAsync(string username);

        Task<User?> GetByIdAsync(int id); // Get single user by ID
        Task AddAsync(User user); // Creates new user in system

        // Updates user info
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);  // Remove user from database
    }
}

using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing user-related business logic.
    /// Provides methods for CRUD operations and user queries.
    /// </summary>
    public interface IUserService
    {
       
        /// Retrieves all users in the system
      
        /// returns Collection of all users as DTOs
        Task<IEnumerable<UserDto>> GetAllUsersAsync();


        /// Retrieves a specific user by their unique identifier

     
        /// returns User DTO if found, null otherwise
        Task<UserDto?> GetUserByIdAsync(int id);


        /// Retrieves a user by their username

        /// returns  User DTO if found, null otherwise
        Task<UserDto?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Creates a new user in the system
       
        Task<UserDto> CreateUserAsync(CreateUserDto dto);

    
        /// Updates an existing user's information
    
        Task<UserDto?> UpdateUserAsync(int id, CreateUserDto dto);


        /// Deletes a user from the system

      
        Task<bool> DeleteUserAsync(int id);
    }
}